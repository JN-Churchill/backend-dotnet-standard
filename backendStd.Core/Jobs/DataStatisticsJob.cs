using Microsoft.Extensions.Logging;
using Quartz;
using SqlSugar;
using backendStd.Core.Entity;

namespace backendStd.Core.Jobs;

/// <summary>
/// 数据统计任务
/// 定期统计系统数据，生成报表等
/// </summary>
[DisallowConcurrentExecution]
public class DataStatisticsJob : BaseJob
{
    private readonly ISqlSugarClient _db;

    public DataStatisticsJob(ILogger<DataStatisticsJob> logger, ISqlSugarClient db) : base(logger)
    {
        _db = db;
    }

    protected override async Task ExecuteJob(IJobExecutionContext context)
    {
        Logger.LogInformation("开始执行数据统计任务");

        try
        {
            // 示例：统计用户数量
            var totalUsers = await _db.Queryable<User>().CountAsync();
            var activeUsers = await _db.Queryable<User>().Where(u => u.Status == 1).CountAsync();

            // 示例：统计今日新增用户
            var today = DateTime.Today;
            var todayNewUsers = await _db.Queryable<User>()
                .Where(u => u.CreateTime >= today)
                .CountAsync();

            Logger.LogInformation("数据统计完成 - 总用户数: {TotalUsers}, 活跃用户: {ActiveUsers}, 今日新增: {TodayNewUsers}",
                totalUsers, activeUsers, todayNewUsers);

            // 这里可以将统计结果保存到统计表中
            // await _db.Insertable(new StatisticsRecord { ... }).ExecuteCommandAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "数据统计失败: {Message}", ex.Message);
            throw;
        }

        await Task.CompletedTask;
    }
}
