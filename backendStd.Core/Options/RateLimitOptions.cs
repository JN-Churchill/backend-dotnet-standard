namespace backendStd.Core.Options;

/// <summary>
/// 限流配置选项
/// </summary>
public class RateLimitOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 默认时间窗口（秒）
    /// </summary>
    public int DefaultWindow { get; set; } = 60;

    /// <summary>
    /// 默认请求次数限制
    /// </summary>
    public int DefaultLimit { get; set; } = 100;

    /// <summary>
    /// IP级别限流配置
    /// </summary>
    public IpRateLimitConfig IpRateLimit { get; set; } = new IpRateLimitConfig();

    /// <summary>
    /// 用户级别限流配置
    /// </summary>
    public UserRateLimitConfig UserRateLimit { get; set; } = new UserRateLimitConfig();
}

/// <summary>
/// IP限流配置
/// </summary>
public class IpRateLimitConfig
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 时间窗口（秒）
    /// </summary>
    public int Window { get; set; } = 60;

    /// <summary>
    /// 请求次数限制
    /// </summary>
    public int Limit { get; set; } = 100;
}

/// <summary>
/// 用户限流配置
/// </summary>
public class UserRateLimitConfig
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 时间窗口（秒）
    /// </summary>
    public int Window { get; set; } = 60;

    /// <summary>
    /// 请求次数限制
    /// </summary>
    public int Limit { get; set; } = 200;
}
