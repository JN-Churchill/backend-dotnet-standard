using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace backendStd.Application.Controllers;

/// <summary>
/// 健康检查控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    /// <summary>
    /// 获取健康状态（公开访问）
    /// </summary>
    /// <returns>健康状态</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetHealth()
    {
        var report = await _healthCheckService.CheckHealthAsync();
        
        var response = new
        {
            Status = report.Status.ToString(),
            TotalDuration = report.TotalDuration.TotalMilliseconds,
            Timestamp = DateTime.Now
        };

        return report.Status == HealthStatus.Healthy 
            ? Ok(response) 
            : StatusCode(503, response);
    }

    /// <summary>
    /// 获取详细健康报告（需要认证）
    /// </summary>
    /// <returns>详细健康报告</returns>
    [HttpGet("details")]
    [Authorize]
    public async Task<IActionResult> GetHealthDetails()
    {
        var report = await _healthCheckService.CheckHealthAsync();
        
        var response = new
        {
            Status = report.Status.ToString(),
            TotalDuration = report.TotalDuration.TotalMilliseconds,
            Entries = report.Entries.Select(e => new
            {
                Name = e.Key,
                Status = e.Value.Status.ToString(),
                Description = e.Value.Description,
                Duration = e.Value.Duration.TotalMilliseconds,
                Data = e.Value.Data,
                Exception = e.Value.Exception?.Message
            }),
            Timestamp = DateTime.Now
        };

        return report.Status == HealthStatus.Healthy 
            ? Ok(response) 
            : StatusCode(503, response);
    }
}
