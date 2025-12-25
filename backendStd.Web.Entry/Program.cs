using backendStd.Core.SqlSugarConfig;
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

    // 添加SqlSugar配置
    builder.Services.AddSqlSugarSetup();

    // 添加内存缓存
    builder.Services.AddMemoryCache();

    // 配置统一返回结果
    builder.Services.AddUnifyResult<backendStd.Core.Util.TdivsResultProvider>();

    // 配置Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // 配置HTTP请求管道
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
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

