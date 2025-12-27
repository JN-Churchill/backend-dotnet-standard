namespace backendStd.Core.Options;

/// <summary>
/// 健康检查配置选项
/// </summary>
public class HealthCheckOptions
{
    /// <summary>
    /// 是否启用健康检查
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 是否启用详细健康报告（仅管理员可访问）
    /// </summary>
    public bool EnableDetailedReport { get; set; } = true;

    /// <summary>
    /// 健康检查端点路径
    /// </summary>
    public string Endpoint { get; set; } = "/health";

    /// <summary>
    /// 详细健康报告端点路径
    /// </summary>
    public string DetailEndpoint { get; set; } = "/health/details";

    /// <summary>
    /// 是否检查数据库
    /// </summary>
    public bool CheckDatabase { get; set; } = true;

    /// <summary>
    /// 是否检查Redis
    /// </summary>
    public bool CheckRedis { get; set; } = false;

    /// <summary>
    /// 是否检查Quartz
    /// </summary>
    public bool CheckQuartz { get; set; } = true;

    /// <summary>
    /// 是否检查磁盘空间
    /// </summary>
    public bool CheckDiskSpace { get; set; } = true;

    /// <summary>
    /// 磁盘空间最小值（GB）
    /// </summary>
    public long MinimumFreeDiskSpaceGB { get; set; } = 1;

    /// <summary>
    /// 是否检查内存
    /// </summary>
    public bool CheckMemory { get; set; } = true;

    /// <summary>
    /// 最大内存使用率（百分比）
    /// </summary>
    public long MaxMemoryUsagePercentage { get; set; } = 90;
}
