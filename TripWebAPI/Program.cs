using Microsoft.EntityFrameworkCore;
using TripWebData;
using Serilog;
using Serilog.Events;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using TripWebAPI;
using TripWebAPI.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using TripWebData.Options;
using TripWebAPI.Hubs;
using DotNetCore.CAP;
using System.Text.Json.Serialization;
using TripWebData.Consts;
using TripWebService;
using TripWebUtils.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(p =>
{
    p.Filters.Add<TokenActionFilter>();
    p.Filters.Add<ExceptionFilter>();
}).AddJsonOptions(
options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
    ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.Converters.Add(new DateConverter());
}
);

builder.Services.AddDbContextPool<TripWebContext>(p =>
{
    p.UseMySql(builder.Configuration.GetConnectionString("mysql"), new MySqlServerVersion("9.0.1"));
    p.LogTo(Console.WriteLine, LogLevel.Information);
    p.EnableSensitiveDataLogging(true); 
}, 150);

builder.Host.UseSerilog((context, config) => config
        .WriteTo.Console() 
        .WriteTo.File($"{AppContext.BaseDirectory}/logs/TripWeb.log",
            rollingInterval: RollingInterval.Day
            ,outputTemplate:"{Timestamp:HH:mm:ss fff zzz} " +
                            "|| {Level} " + 
                            "|| {SourceContext:1} " + 
                            "|| {Message} " + 
                            "|| {Exception} " + 
                            "||end {NewLine}" 
                )
                .MinimumLevel.Information() 
                .MinimumLevel.Override("Default", LogEventLevel.Information) 
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("Default.Hosting.Lifetime",LogEventLevel.Information)
                .Enrich.FromLogContext() 
);

//注入AutoFac服务
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule<AutofacModuleRegister>();
});

//注入Automap服务
builder.Services.AddAutoMapper(p =>
{
    p.AddMaps("TripWebData");
});

//使用JWT认证
var jwtOption = builder.Configuration.GetSection("JwtTokenOption");
builder.Services.Configure<JwtTokenOption>(jwtOption);
JwtTokenOption jwtTokenOption = jwtOption.Get<JwtTokenOption>();

var rsa = RSA.Create();
rsa.ImportRSAPrivateKey(Convert.FromBase64String(jwtTokenOption.SecurityKey), out _);
SecurityKey securityKey = new RsaSecurityKey(rsa);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(p =>
{
    p.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidAlgorithms = new string[] { "RS256" },
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        ValidAudience = jwtTokenOption.Audience,
        ValidIssuer = jwtTokenOption.Issuer,
        IssuerSigningKey = securityKey,
    };
    
    //在signalR中增加认证
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

// 新增授权策略
builder.Services.AddAuthorization(p =>
{
    p.AddPolicy(AuthorizeRoleName.Administrator, policy =>
    {
        policy.RequireClaim("RoleName", AuthorizeRoleName.Administrator);
    });

    p.AddPolicy(AuthorizeRoleName.SellerAdministrator, policy =>
    {
        policy.RequireClaim("RoleName", AuthorizeRoleName.SellerAdministrator);
    });

    p.AddPolicy(AuthorizeRoleName.TravelUser, policy =>
    {
        policy.RequireClaim("RoleName", AuthorizeRoleName.TravelUser);
    });

    p.AddPolicy(AuthorizeRoleName.AdminOrSeller, policy =>
    {
        policy.RequireClaim("RoleName",
        AuthorizeRoleName.SellerAdministrator, AuthorizeRoleName.Administrator);
    });
});

// 跨域设置
builder.Services.AddCors(p =>
{
    p.AddPolicy("Travel.Client", policy =>
    {
        policy.AllowAnyHeader();
        policy.SetIsOriginAllowed(p => true);
        policy.AllowAnyMethod();
        policy.AllowCredentials();
    });
});

// 注入SignalR服务
builder.Services.AddSignalR();

// 注入RabbitMQ服务
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
    p.UseDashboard(); 
});

builder.Services.AddSingleton<TokenService>();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
LocalServiceProvider.Instance = app.Services;

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("Travel.Client");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<TripWebHub>("/TripWebHub"); 

app.Run();
