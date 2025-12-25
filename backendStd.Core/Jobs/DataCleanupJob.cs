using Microsoft.Extensions.Logging;
using Quartz;
using SqlSugar;

namespace backendStd.Core.Jobs;

/// <summary>
/// 数据清理任务
/// 定期清理过期的临时数据、日志等
/// </summary>
[DisallowConcurrentExecution] // 不允许并发执行
public class DataCleanupJob : BaseJob
{
    private readonly ISqlSugarClient _db;

    public DataCleanupJob(ILogger<DataCleanupJob> logger, ISqlSugarClient db) : base(logger)
    {
        _db = db;
    }

    protected override async Task ExecuteJob(IJobExecutionContext context)
    {
        var expiredDays = 30; // 清理30天前的数据
        var expiredDate = DateTime.Now.AddDays(-expiredDays);

        Logger.LogInformation("开始清理 {ExpiredDays} 天前的过期数据", expiredDays);

        try
        {
            // 示例：清理软删除超过30天的数据
            // 注意：这里只是示例，实际应该针对具体实体类型进行清理
            // 例如：清理Demo表中的软删除数据
            var deletedCount = 0;
            
            // 这里可以根据实际需求清理不同表的数据
            // deletedCount += await _db.Deleteable<Demo>()
            //     .Where(e => e.IsDeleted == true && e.DeleteTime != null && e.DeleteTime < expiredDate)
            //     .ExecuteCommandAsync();

            Logger.LogInformation("数据清理完成，共清理 {Count} 条记录", deletedCount);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "数据清理失败: {Message}", ex.Message);
            throw;
        }

        await Task.CompletedTask;
    }
}
