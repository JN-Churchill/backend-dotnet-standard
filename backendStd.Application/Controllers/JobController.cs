using backendStd.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backendStd.Application.Controllers;

/// <summary>
/// 任务管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobController : ControllerBase
{
    private readonly JobService _jobService;

    public JobController(JobService jobService)
    {
        _jobService = jobService;
    }

    /// <summary>
    /// 获取所有任务
    /// </summary>
    [HttpGet("list")]
    public async Task<List<object>> GetAllJobs()
    {
        return await _jobService.GetAllJobsAsync();
    }

    /// <summary>
    /// 暂停任务
    /// </summary>
    [HttpPost("{jobName}/pause")]
    public async Task<bool> PauseJob(string jobName, [FromQuery] string jobGroup = "DEFAULT")
    {
        return await _jobService.PauseJobAsync(jobName, jobGroup);
    }

    /// <summary>
    /// 恢复任务
    /// </summary>
    [HttpPost("{jobName}/resume")]
    public async Task<bool> ResumeJob(string jobName, [FromQuery] string jobGroup = "DEFAULT")
    {
        return await _jobService.ResumeJobAsync(jobName, jobGroup);
    }

    /// <summary>
    /// 立即执行任务
    /// </summary>
    [HttpPost("{jobName}/trigger")]
    public async Task<bool> TriggerJob(string jobName, [FromQuery] string jobGroup = "DEFAULT")
    {
        return await _jobService.TriggerJobAsync(jobName, jobGroup);
    }

    /// <summary>
    /// 删除任务
    /// </summary>
    [HttpDelete("{jobName}")]
    public async Task<bool> DeleteJob(string jobName, [FromQuery] string jobGroup = "DEFAULT")
    {
        return await _jobService.DeleteJobAsync(jobName, jobGroup);
    }
}
