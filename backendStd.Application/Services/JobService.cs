using Quartz;
using Quartz.Impl.Matchers;

namespace backendStd.Application.Services;

/// <summary>
/// 任务管理服务
/// </summary>
public class JobService
{
    private readonly ISchedulerFactory _schedulerFactory;

    public JobService(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    /// <summary>
    /// 获取所有任务
    /// </summary>
    public async Task<List<object>> GetAllJobsAsync()
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobGroups = await scheduler.GetJobGroupNames();
        var jobs = new List<object>();

        foreach (var group in jobGroups)
        {
            var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group));
            
            foreach (var jobKey in jobKeys)
            {
                var triggers = await scheduler.GetTriggersOfJob(jobKey);
                var jobDetail = await scheduler.GetJobDetail(jobKey);

                foreach (var trigger in triggers)
                {
                    var triggerState = await scheduler.GetTriggerState(trigger.Key);
                    
                    jobs.Add(new
                    {
                        JobName = jobKey.Name,
                        JobGroup = jobKey.Group,
                        TriggerName = trigger.Key.Name,
                        TriggerGroup = trigger.Key.Group,
                        NextFireTime = trigger.GetNextFireTimeUtc()?.LocalDateTime,
                        PreviousFireTime = trigger.GetPreviousFireTimeUtc()?.LocalDateTime,
                        State = triggerState.ToString(),
                        Description = jobDetail?.Description
                    });
                }
            }
        }

        return jobs;
    }

    /// <summary>
    /// 暂停任务
    /// </summary>
    public async Task<bool> PauseJobAsync(string jobName, string jobGroup = "DEFAULT")
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey(jobName, jobGroup);
        
        if (await scheduler.CheckExists(jobKey))
        {
            await scheduler.PauseJob(jobKey);
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// 恢复任务
    /// </summary>
    public async Task<bool> ResumeJobAsync(string jobName, string jobGroup = "DEFAULT")
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey(jobName, jobGroup);
        
        if (await scheduler.CheckExists(jobKey))
        {
            await scheduler.ResumeJob(jobKey);
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// 立即执行任务
    /// </summary>
    public async Task<bool> TriggerJobAsync(string jobName, string jobGroup = "DEFAULT")
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey(jobName, jobGroup);
        
        if (await scheduler.CheckExists(jobKey))
        {
            await scheduler.TriggerJob(jobKey);
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// 删除任务
    /// </summary>
    public async Task<bool> DeleteJobAsync(string jobName, string jobGroup = "DEFAULT")
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey(jobName, jobGroup);
        
        if (await scheduler.CheckExists(jobKey))
        {
            return await scheduler.DeleteJob(jobKey);
        }
        
        return false;
    }
}
