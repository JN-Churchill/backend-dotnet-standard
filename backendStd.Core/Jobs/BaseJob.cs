using Microsoft.Extensions.Logging;
using Quartz;

namespace backendStd.Core.Jobs;

/// <summary>
/// 定时任务基类
/// </summary>
public abstract class BaseJob : IJob
{
    protected readonly ILogger Logger;

    protected BaseJob(ILogger logger)
    {
        Logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = context.JobDetail.Key.Name;
        Logger.LogInformation("定时任务 [{JobName}] 开始执行", jobName);

        try
        {
            await ExecuteJob(context);
            Logger.LogInformation("定时任务 [{JobName}] 执行成功", jobName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "定时任务 [{JobName}] 执行失败: {Message}", jobName, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// 子类实现具体的任务逻辑
    /// </summary>
    protected abstract Task ExecuteJob(IJobExecutionContext context);
}
