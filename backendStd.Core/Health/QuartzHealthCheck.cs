using Microsoft.Extensions.Diagnostics.HealthChecks;
using Quartz;

namespace backendStd.Core.Health;

/// <summary>
/// Quartz任务调度器健康检查
/// </summary>
public class QuartzHealthCheck : IHealthCheck
{
    private readonly ISchedulerFactory _schedulerFactory;

    public QuartzHealthCheck(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            
            if (scheduler == null)
            {
                return HealthCheckResult.Unhealthy("Quartz调度器实例为空");
            }

            var isStarted = scheduler.IsStarted;
            var isShutdown = scheduler.IsShutdown;
            
            var jobKeys = await scheduler.GetJobKeys(Quartz.Impl.Matchers.GroupMatcher<JobKey>.AnyGroup(), cancellationToken);
            var jobCount = jobKeys.Count;

            var data = new Dictionary<string, object>
            {
                { "IsStarted", isStarted },
                { "IsShutdown", isShutdown },
                { "JobCount", jobCount }
            };

            if (isShutdown)
            {
                return HealthCheckResult.Unhealthy("Quartz调度器已关闭", data: data);
            }

            if (!isStarted)
            {
                return HealthCheckResult.Degraded("Quartz调度器未启动", data: data);
            }

            return HealthCheckResult.Healthy("Quartz调度器运行正常", data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Quartz调度器检查失败", ex);
        }
    }
}
