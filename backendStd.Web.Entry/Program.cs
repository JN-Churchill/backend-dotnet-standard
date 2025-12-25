using backendStd.Core.SqlSugarConfig;
using backendStd.Application.Services;
using backendStd.Core.Repository;
using backendStd.Core.Cache;
using backendStd.Core.Auth;
using backendStd.Core.Options;
using backendStd.Core.Filters;
using backendStd.Core.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// 配置Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // 添加Serilog
    builder.Host.UseSerilog();

    // 配置服务
    builder.Services.AddControllers(options =>
    {
        // 添加全局异常过滤器
        options.Filters.Add<GlobalExceptionFilter>();
    });

    // 配置JWT选项
    builder.Services.Configure<JWTSettingsOptions>(builder.Configuration.GetSection("JWTSettings"));
    builder.Services.Configure<RefreshTokenOptions>(builder.Configuration.GetSection("RefreshTokenOptions"));
    builder.Services.Configure<RequestLoggingOptions>(builder.Configuration.GetSection("RequestLoggingOptions"));
    builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection("RateLimitOptions"));

    // 添加JWT认证
    var jwtSettings = builder.Configuration.GetSection("JWTSettings").Get<JWTSettingsOptions>();
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = jwtSettings!.ValidateIssuerSigningKey,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.IssuerSigningKey)),
            ValidateIssuer = jwtSettings.ValidateIssuer,
            ValidIssuer = jwtSettings.ValidIssuer,
            ValidateAudience = jwtSettings.ValidateAudience,
            ValidAudience = jwtSettings.ValidAudience,
            ValidateLifetime = jwtSettings.ValidateLifetime,
            ClockSkew = TimeSpan.FromMinutes(jwtSettings.ClockSkew)
        };
    });

    // 添加FluentValidation
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<backendStd.Application.Validators.UserInputValidator>();

    // 添加SqlSugar配置
    builder.Services.AddSqlSugarSetup();

    // 注册仓储
    builder.Services.AddScoped(typeof(IRepository<>), typeof(SqlSugarRepository<>));

    // 注册JWT处理器
    builder.Services.AddScoped<JwtHandler>();

    // 注册缓存服务（默认使用内存缓存）
    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

    // 注册业务服务
    builder.Services.AddScoped<UserService>();
    builder.Services.AddScoped<DemoService>();
    builder.Services.AddScoped<FileService>();
    builder.Services.AddScoped<RoleService>();
    builder.Services.AddScoped<PermissionService>();

    // 配置统一返回结果
    builder.Services.AddUnifyResult<backendStd.Core.Util.TdivsResultProvider>();

    // 配置Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Backend Standard API",
            Version = "v1",
            Description = "企业级.NET 8 Web API 模板"
        });
        
        // 添加XML注释
        var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
        foreach (var xmlFile in xmlFiles)
        {
            options.IncludeXmlComments(xmlFile, true);
        }
    });

    var app = builder.Build();

    // 配置HTTP请求管道
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend Standard API v1");
        });
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    // 请求日志中间件
    app.UseMiddleware<RequestLoggingMiddleware>();

    // 限流中间件
    app.UseMiddleware<RateLimitingMiddleware>();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("应用程序启动成功");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "应用程序启动失败");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

