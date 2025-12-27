using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace backendStd.Core.Health;

/// <summary>
/// 种子数据初始化状态健康检查
/// </summary>
public class SeedDataHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // 检查种子数据标记文件是否存在
            var flagFile = "seed_data_initialized.flag";
            var flagPath = Path.Combine(AppContext.BaseDirectory, flagFile);
            var exists = File.Exists(flagPath);

            var data = new Dictionary<string, object>
            {
                { "SeedDataInitialized", exists },
                { "FlagFilePath", flagPath }
            };

            if (exists)
            {
                return Task.FromResult(HealthCheckResult.Healthy("种子数据已初始化", data));
            }

            return Task.FromResult(HealthCheckResult.Degraded("种子数据未初始化", data: data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("种子数据状态检查失败", ex));
        }
    }
}
