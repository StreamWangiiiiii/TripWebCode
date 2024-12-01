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

//ע�����л�����
builder.Services.AddControllers(p =>
{
    p.Filters.Add<TokenActionFilter>();
    p.Filters.Add<ExceptionFilter>();
}).AddJsonOptions(
options =>
{
    // ����ѭ�����л�
    options.JsonSerializerOptions.ReferenceHandler =
    ReferenceHandler.IgnoreCycles;
    // ʱ��ת��
    options.JsonSerializerOptions.Converters.Add(new DateConverter());
}
);

builder.Services.AddDbContextPool<TripWebContext>(p =>
{
    p.UseMySql(builder.Configuration.GetConnectionString("mysql"), new MySqlServerVersion("9.0.1"));
    p.LogTo(Console.WriteLine, LogLevel.Information); // �ڿ���̨��ӡSQL
    p.EnableSensitiveDataLogging(true); // ���SQL�еĲ���ֵ
}, 150);

builder.Host.UseSerilog((context, config) =>
    config
        .WriteTo.Console() // ���뵽����̨
        // ͬʱ������ļ��У����Ұ������(ÿ�춼�ᴴ��һ���µ��ļ�)
        .WriteTo.File($"{AppContext.BaseDirectory}/logs/renwoxing.log",
            rollingInterval: RollingInterval.Day
            // fff������ zzz:ʱ��
            ,outputTemplate:"{Timestamp:HH:mm:ss fff zzz} " +
                            "|| {Level} " + // Level:��־����
                            "|| {SourceContext:1} " + //SourceContext����־������
                            "|| {Message} " + // Message����־����
                            "|| {Exception} " + // Exception���쳣��Ϣ
                            "||end {NewLine}" //end:������־ NewLine������
                )
                .MinimumLevel.Information() // ������С����
                .MinimumLevel.Override("Default", LogEventLevel.Information) // Ĭ������
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error) // ֻ���΢��Ĵ�����־
                // ����������־
                .MinimumLevel.Override("Default.Hosting.Lifetime",LogEventLevel.Information)
                .Enrich.FromLogContext() // ����־������Ҳ��¼����־��
);

// ��ServiceProvider ����ΪAutofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // ע��ո��Զ����Module��
    containerBuilder.RegisterModule<AutofacModuleRegister>();
    // ����ж��Module�������Լ������
});

// ����ע��Autofac�����ļ�
builder.Services.AddAutoMapper(p =>
{
    p.AddMaps("TripWebData");
});

//ע��swagger����
builder.Services.AddSwaggerGen(p => {
    p.SwaggerDoc("v1", new OpenApiInfo()
    {
        Contact = new()
        {
            Email = "718324225@qq.com",
        },
        Description = "��;����Ŀʵս",
        Title = "��;��"
    });


    //Bearer ��scheme����
    var securityScheme = new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization ͷʹ��Bearer ��ϵ����. ��:Authorization: Bearer ���token\"",
        Name = "Authorization",
        //���������ͷ��
        In = ParameterLocation.Header,
        //ʹ��Authorizeͷ��
        Type = SecuritySchemeType.Http,
        //����Ϊ�� bearer��ͷ
        Scheme = "Bearer",
        BearerFormat = "JWT"
    };

    //�����з�������Ϊ����bearerͷ����Ϣ
    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "JWT��֤"
                }
            },
            new string[] {}
        }
    };

    //ע�ᵽswagger��
    p.AddSecurityDefinition("JWT��֤", securityScheme);
    p.AddSecurityRequirement(securityRequirement);

    //���swagger������---����ĳЩ���뱩¶�����Ľӿ�
    p.DocumentFilter<HiddenApiFilter>();

    // ����xml�ĵ�ע��
    p.IncludeXmlComments(AppContext.BaseDirectory +
    Assembly.GetExecutingAssembly().GetName().Name + ".xml", true);
    // ʵ����ע��Ҳ��Ҫ����
    p.IncludeXmlComments(AppContext.BaseDirectory + "TripWebData.xml");

});

var jwtOption = builder.Configuration.GetSection("JwtTokenOption");
builder.Services.Configure<JwtTokenOption>(jwtOption);
JwtTokenOption jwtTokenOption = jwtOption.Get<JwtTokenOption>();

// �����֤����
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(p =>
{
    var rsa = RSA.Create();
    rsa.ImportRSAPrivateKey(Convert.FromBase64String(jwtTokenOption.SecurityKey), out _);
    SecurityKey securityKey = new RsaSecurityKey(rsa);
    // У��JWT�Ƿ�Ϸ�
    p.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidAlgorithms = new string[] { "RS256" },
        ValidateIssuer = true,//�Ƿ���֤Issuer
        ValidateAudience = true,//�Ƿ���֤Audience
        ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
        ClockSkew = TimeSpan.Zero,//ʱ��������λ��
        ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
        ValidAudience = jwtTokenOption.Audience,//Audience
        ValidIssuer = jwtTokenOption.Issuer,//Issuer���������ǰ��ǩ��jwt������һ��
        IssuerSigningKey = securityKey,//�õ�SecurityKey
    };

    // SignalRͨѶ�е�Token��֤
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

// ����������Ȩ����
builder.Services.AddAuthorization(p =>
{
    // ����Ա����
    p.AddPolicy(AuthorizeRoleName.Administrator, policy =>
    {
        policy.RequireClaim("RoleName", AuthorizeRoleName.Administrator);
    });
    // �̼Ҳ���
    p.AddPolicy(AuthorizeRoleName.SellerAdministrator, policy =>
    {
        policy.RequireClaim("RoleName", AuthorizeRoleName.SellerAdministrator);
    });
    // ��ͨ�û�����
    p.AddPolicy(AuthorizeRoleName.TravelUser, policy =>
    {
        policy.RequireClaim("RoleName", AuthorizeRoleName.TravelUser);
    });
    // ����Ա�����̼Ҷ��ɲ���
    p.AddPolicy(AuthorizeRoleName.AdminOrSeller, policy =>
    {
        policy.RequireClaim("RoleName",
        AuthorizeRoleName.SellerAdministrator, AuthorizeRoleName.Administrator);
    });
});

// �����������
builder.Services.AddCors(p =>
{
    p.AddPolicy("Travel.Client", policy =>
    {
        policy.AllowAnyHeader();//�������пͻ���
        policy.SetIsOriginAllowed(p => true);
        policy.AllowAnyMethod();
        policy.AllowCredentials(); // ��Ҫ��Ϊ������signalR����ͨѶ
    });
});

// ���ü�ʱͨѶ
builder.Services.AddSignalR();

// �����¼������Լ��ֲ�ʽ����
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
    p.usedashboard(); // ע���Ǳ���
                      // �Ǳ���Ĭ�ϵķ��ʵ�ַ�ǣ�http://localhost:xxx/cap���������d.matchpath���������޸�cap·����׺Ϊ���������֡�
});*/

builder.Services.AddSingleton<TokenService>();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.UseSwagger(p =>
{
    // ��������Ϊv2�汾����UseSwaggerUI �е� SwaggerEndpoint����swagger.json��·����Ҫ����Ϊ / swagger / v2 / swagger.json
    // p.SerializeAsV2 = true; // �������ΪV2����SwaggerĬ��UI��֧��Bearer��֤�ˣ����Բ�Ҫ����
});

// ԭ�е�UI
app.UseSwaggerUI(p =>
{
    p.SwaggerEndpoint("/swagger/v1/swagger.json", "��;��");
    p.RoutePrefix = "swagger";// Ĭ��ֵ��swagger
});

// �Զ����UI
app.UseKnife4UI(p =>
{
    p.SwaggerEndpoint("/swagger/v1/swagger.json", "��;��");
    p.RoutePrefix = "";// Ĭ��ֵ��swagger
});

app.UseHttpsRedirection();

app.UseStaticFiles();//��̬�ļ��м��

app.UseCors("Travel.Client");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<TripWebHub>("/TripWebHub"); // ӳ��signalRͨѶ����

app.Run();
