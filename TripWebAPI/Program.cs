using Microsoft.EntityFrameworkCore;
using TripWebData;
using Serilog;
using Serilog.Events;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using TripWebAPI;
using Microsoft.OpenApi.Models;
using System.Reflection;
using IGeekFan.AspNetCore.Knife4jUI;
using TripWebAPI.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using TripWebData.Options;
using TripWebAPI.Hubs;
using DotNetCore.CAP;
using System.Text.Json.Serialization;
using TripWebAPI.Models;
using TripWebService;

var builder = WebApplication.CreateBuilder(args);

//注入序列化服务
builder.Services.AddControllers(p =>
{
    p.Filters.Add<TokenActionFilter>();
    p.Filters.Add<ExceptionFilter>();
}).AddJsonOptions(
options =>
{
    // 忽略循环序列化
    options.JsonSerializerOptions.ReferenceHandler =
    ReferenceHandler.IgnoreCycles;
    // 时间转换
    options.JsonSerializerOptions.Converters.Add(new DateConverter());
}
);

builder.Services.AddDbContextPool<TripWebContext>(p =>
{
    p.UseMySql(builder.Configuration.GetConnectionString("mysql"), new MySqlServerVersion("9.0.1"));
    p.LogTo(Console.WriteLine, LogLevel.Information); // 在控制台打印SQL
    p.EnableSensitiveDataLogging(true); // 打出SQL中的参数值
}, 150);

builder.Host.UseSerilog((context, config) =>
    config
        .WriteTo.Console() // 输入到控制台
        // 同时输出到文件中，并且按天输出(每天都会创建一个新的文件)
        .WriteTo.File($"{AppContext.BaseDirectory}/logs/renwoxing.log",
            rollingInterval: RollingInterval.Day
            // fff：毫秒 zzz:时区
            ,outputTemplate:"{Timestamp:HH:mm:ss fff zzz} " +
                            "|| {Level} " + // Level:日志级别
                            "|| {SourceContext:1} " + //SourceContext：日志上下文
                            "|| {Message} " + // Message：日志内容
                            "|| {Exception} " + // Exception：异常信息
                            "||end {NewLine}" //end:结束标志 NewLine：换行
                )
                .MinimumLevel.Information() // 设置最小级别
                .MinimumLevel.Override("Default", LogEventLevel.Information) // 默认设置
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error) // 只输出微软的错误日志
                // 生命周期日志
                .MinimumLevel.Override("Default.Hosting.Lifetime",LogEventLevel.Information)
                .Enrich.FromLogContext() // 将日志上下文也记录到日志中
);

// 将ServiceProvider 更换为Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // 注册刚刚自定义的Module类
    containerBuilder.RegisterModule<AutofacModuleRegister>();
    // 如果有多个Module，还可以继续添加
});

// 批量注册Autofac配置文件
builder.Services.AddAutoMapper(p =>
{
    p.AddMaps("TripWebData");
});

var jwtOption = builder.Configuration.GetSection("JwtTokenOption");
builder.Services.Configure<JwtTokenOption>(jwtOption);
JwtTokenOption jwtTokenOption = jwtOption.Get<JwtTokenOption>();

//创建密钥
var rsa = RSA.Create();
rsa.ImportRSAPrivateKey(Convert.FromBase64String(jwtTokenOption.SecurityKey), out _);
SecurityKey securityKey = new RsaSecurityKey(rsa);

// 添加认证服务
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(p =>
{
    // 校验JWT是否合法
    p.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidAlgorithms = new string[] { "RS256" },
        ValidateIssuer = true,//是否验证Issuer
        ValidateAudience = true,//是否验证Audience
        ValidateLifetime = true,//是否验证失效时间
        ClockSkew = TimeSpan.Zero,//时钟脉冲相位差
        ValidateIssuerSigningKey = true,//是否验证SecurityKey
        ValidAudience = jwtTokenOption.Audience,//Audience
        ValidIssuer = jwtTokenOption.Issuer,//Issuer，这两项和前面签发jwt的设置一致
        IssuerSigningKey = securityKey,//拿到SecurityKey
    };

    // SignalR通讯中的Token认证
    p.Events = new JwtBearerEvents()
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Query["access_token"].ToString();

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrWhiteSpace(token) && path.StartsWithSegments("/TripWebHub"))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

// 设置三种授权策略
builder.Services.AddAuthorization(p =>
{
    // 管理员策略
    p.AddPolicy(AuthorizeRoleName.Administrator, policy =>
    {
        policy.RequireClaim("RoleName", AuthorizeRoleName.Administrator);
    });
    // 商家策略
    p.AddPolicy(AuthorizeRoleName.SellerAdministrator, policy =>
    {
        policy.RequireClaim("RoleName", AuthorizeRoleName.SellerAdministrator);
    });
    // 普通用户策略
    p.AddPolicy(AuthorizeRoleName.TravelUser, policy =>
    {
        policy.RequireClaim("RoleName", AuthorizeRoleName.TravelUser);
    });
    // 管理员或者商家都可操作
    p.AddPolicy(AuthorizeRoleName.AdminOrSeller, policy =>
    {
        policy.RequireClaim("RoleName",
        AuthorizeRoleName.SellerAdministrator, AuthorizeRoleName.Administrator);
    });
});

// 配置允许跨域
builder.Services.AddCors(p =>
{
    p.AddPolicy("Travel.Client", policy =>
    {
        policy.AllowAnyHeader();//允许所有客户端
        policy.SetIsOriginAllowed(p => true);
        policy.AllowAnyMethod();
        policy.AllowCredentials(); // 主要是为了允许signalR跨域通讯
    });
});

// 配置即时通讯
builder.Services.AddSignalR();

// 配置事件总线以及分布式事务
var rabbitConfig = builder.Configuration.GetSection("RabbitMQ");
builder.Services.Configure<RabbitMQOptions>(rabbitConfig);
var rabbitOptions = rabbitConfig.Get<RabbitMQOptions>();

builder.Services.AddCap(p =>
{
    p.UseMySql(builder.Configuration.GetConnectionString("mysql") ?? string.Empty);
    p.UseEntityFramework<TripWebContext>();
    p.UseRabbitMQ(mq =>
    {
        mq.HostName = rabbitOptions.HostName;
        mq.VirtualHost = rabbitOptions.VirtualHost;
        mq.UserName = rabbitOptions.UserName;
        mq.Password = rabbitOptions.Password;
        mq.Port = rabbitOptions.Port;
    });
    p.UseDashboard(); // 注册仪表盘
    // 仪表盘默认的访问地址是：http://localhost:xxx/cap，你可以在d.MatchPath配置项中修改cap路径后缀为其他的名字。
});

builder.Services.AddSingleton<TokenService>();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();//静态文件中间件

app.UseCors("Travel.Client");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<TripWebHub>("/TripWebHub"); // 映射signalR通讯中心

app.Run();
