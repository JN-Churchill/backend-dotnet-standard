namespace backendStd.Core.Auth;

/// <summary>
/// 限流特性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RateLimitAttribute : Attribute
{
    /// <summary>
    /// 时间窗口（秒）
    /// </summary>
    public int Window { get; set; } = 60;

    /// <summary>
    /// 请求次数限制
    /// </summary>
    public int Limit { get; set; } = 100;

    /// <summary>
    /// 限流类型：ip, user, api
    /// </summary>
    public string Type { get; set; } = "ip";

    public RateLimitAttribute()
    {
    }

    public RateLimitAttribute(int limit, int window = 60)
    {
        Limit = limit;
        Window = window;
    }
}
