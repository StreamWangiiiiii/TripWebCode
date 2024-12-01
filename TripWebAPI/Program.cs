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

//注册swagger服务
builder.Services.AddSwaggerGen(p => {
    p.SwaggerDoc("v1", new OpenApiInfo()
    {
        Contact = new()
        {
            Email = "718324225@qq.com",
        },
        Description = "旅途网项目实战",
        Title = "旅途网"
    });


    //Bearer 的scheme定义
    var securityScheme = new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization 头使用Bearer 体系方案. 例:Authorization: Bearer 你的token\"",
        Name = "Authorization",
        //参数添加在头部
        In = ParameterLocation.Header,
        //使用Authorize头部
        Type = SecuritySchemeType.Http,
        //内容为以 bearer开头
        Scheme = "Bearer",
        BearerFormat = "JWT"
    };

    //把所有方法配置为增加bearer头部信息
    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "JWT认证"
                }
            },
            new string[] {}
        }
    };

    //注册到swagger中
    p.AddSecurityDefinition("JWT认证", securityScheme);
    p.AddSecurityRequirement(securityRequirement);

    //添加swagger过滤器---隐藏某些不想暴露出来的接口
    p.DocumentFilter<HiddenApiFilter>();

    // 加载xml文档注释
    p.IncludeXmlComments(AppContext.BaseDirectory +
    Assembly.GetExecutingAssembly().GetName().Name + ".xml", true);
    // 实体层的注释也需要加上
    p.IncludeXmlComments(AppContext.BaseDirectory + "TripWebData.xml");

});

var jwtOption = builder.Configuration.GetSection("JwtTokenOption");
builder.Services.Configure<JwtTokenOption>(jwtOption);
JwtTokenOption jwtTokenOption = jwtOption.Get<JwtTokenOption>();

// 添加认证服务
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(p =>
{
    var rsa = RSA.Create();
    rsa.ImportRSAPrivateKey(Convert.FromBase64String(jwtTokenOption.SecurityKey), out _);
    SecurityKey securityKey = new RsaSecurityKey(rsa);
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
/*var rabbitconfig = builder.configuration.getsection("rabbitmq");
builder.services.configure<rabbitmqoptions>(rabbitconfig);
var rabbitoptions = rabbitconfig.get<rabbitmqoptions>();
builder.services.addcap(p =>
{
    p.usemysql(builder.configuration.getconnectionstring("mysql"));
    p.useentityframework<tripwebcontext>();
    p.userabbitmq(mq =>
    {
        mq.hostname = rabbitoptions.hostname;
        mq.virtualhost = rabbitoptions.virtualhost;
        mq.username = rabbitoptions.username;
        mq.password = rabbitoptions.password;
        mq.port = rabbitoptions.port;
    });
    p.usedashboard(); // 注册仪表盘
                      // 仪表盘默认的访问地址是：http://localhost:xxx/cap，你可以在d.matchpath配置项中修改cap路径后缀为其他的名字。
});*/

builder.Services.AddSingleton<TokenService>();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.UseSwagger(p =>
{
    // 这里设置为v2版本，则UseSwaggerUI 中的 SwaggerEndpoint它的swagger.json的路径需要设置为 / swagger / v2 / swagger.json
    // p.SerializeAsV2 = true; // 如果设置为V2，则Swagger默认UI不支持Bearer认证了，所以不要设置
});

// 原有的UI
app.UseSwaggerUI(p =>
{
    p.SwaggerEndpoint("/swagger/v1/swagger.json", "旅途网");
    p.RoutePrefix = "swagger";// 默认值：swagger
});

// 自定义的UI
app.UseKnife4UI(p =>
{
    p.SwaggerEndpoint("/swagger/v1/swagger.json", "旅途网");
    p.RoutePrefix = "";// 默认值：swagger
});

app.UseHttpsRedirection();

app.UseStaticFiles();//静态文件中间件

app.UseCors("Travel.Client");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<TripWebHub>("/TripWebHub"); // 映射signalR通讯中心

app.Run();
