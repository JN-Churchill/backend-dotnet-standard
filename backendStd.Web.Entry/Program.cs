using backendStd.Core.SqlSugarConfig;
using backendStd.Application.Services;
using backendStd.Core.Repository;
using backendStd.Core.Cache;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;

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
    builder.Services.AddControllers();

    // 添加FluentValidation
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<backendStd.Application.Validators.UserInputValidator>();

    // 添加SqlSugar配置
    builder.Services.AddSqlSugarSetup();

    // 注册仓储
    builder.Services.AddScoped(typeof(IRepository<>), typeof(SqlSugarRepository<>));

    // 注册缓存服务（默认使用内存缓存）
    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

    // 注册业务服务
    builder.Services.AddScoped<UserService>();
    builder.Services.AddScoped<DemoService>();
    builder.Services.AddScoped<FileService>();

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

