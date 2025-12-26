using backendStd.Core.SqlSugarConfig;
using backendStd.Application.Services;
using backendStd.Core.Repository;
using backendStd.Core.Cache;
using backendStd.Core.Auth;
using backendStd.Core.Options;
using backendStd.Core.Filters;
using backendStd.Core.Middleware;
using backendStd.Core.Jobs;
using backendStd.Core.Const;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Claims;
using Quartz;

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
    
    // 配置模型验证错误响应格式（开发环境显示详细错误）
    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .Select(e => new
                {
                    Field = e.Key,
                    Errors = e.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                })
                .ToList();

            var result = new
            {
                Code = 400,
                Type = "validation_error",
                Message = "模型验证失败",
                Result = (object?)null,
                Extras = errors,
                Time = DateTime.Now
            };

            return new BadRequestObjectResult(result);
        };
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
    builder.Services.AddSqlSugarSetup(builder.Configuration);

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
    builder.Services.AddScoped<JobService>();
    builder.Services.AddScoped<DepartmentService>();

    // 配置Quartz定时任务
    builder.Services.AddQuartz(q =>
    {
        q.UseMicrosoftDependencyInjectionJobFactory();

        // 配置DataCleanupJob - 每天凌晨2点执行
        var dataCleanupJobKey = new JobKey("DataCleanupJob");
        q.AddJob<DataCleanupJob>(opts => opts.WithIdentity(dataCleanupJobKey));
        q.AddTrigger(opts => opts
            .ForJob(dataCleanupJobKey)
            .WithIdentity("DataCleanupJob-trigger")
            .WithCronSchedule("0 0 2 * * ?") // 每天凌晨2点
            .WithDescription("数据清理任务 - 每天凌晨2点执行"));

        // 配置DataStatisticsJob - 每天凌晨1点执行
        var dataStatisticsJobKey = new JobKey("DataStatisticsJob");
        q.AddJob<DataStatisticsJob>(opts => opts.WithIdentity(dataStatisticsJobKey));
        q.AddTrigger(opts => opts
            .ForJob(dataStatisticsJobKey)
            .WithIdentity("DataStatisticsJob-trigger")
            .WithCronSchedule("0 0 1 * * ?") // 每天凌晨1点
            .WithDescription("数据统计任务 - 每天凌晨1点执行"));
    });

    // 添加Quartz托管服务
    builder.Services.AddQuartzHostedService(options =>
    {
        options.WaitForJobsToComplete = true;
    });

    // 配置统一返回结果（已注释，使用GlobalExceptionFilter代替）
    // builder.Services.AddUnifyResult<backendStd.Core.Util.TdivsResultProvider>();

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
        
        // 添加JWT认证支持
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description = "JWT授权 - 直接输入token即可(开发环境输入driver即可通过)，系统会自动添加Bearer前缀",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });
        
        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
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
            options.RoutePrefix = string.Empty; // 设置Swagger UI为根路径
        });
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    // 开发环境认证后门（必须在UseAuthentication之前）
    if (app.Environment.IsDevelopment())
    {
        app.UseMiddleware<DevBypassMiddleware>();
    }

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

