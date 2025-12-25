using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using backendStd.Core.Options;
using backendStd.Core.Cache;
using backendStd.Core.Const;
using backendStd.Core.Auth;
using System.Collections.Concurrent;

namespace backendStd.Core.Middleware;

/// <summary>
/// 限流中间件
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitOptions _options;
    private readonly ICacheService _cacheService;

    // 内存存储（滑动窗口）
    private static readonly ConcurrentDictionary<string, Queue<DateTime>> _requestHistory = new();

    public RateLimitingMiddleware(
        RequestDelegate next,
        ILogger<RateLimitingMiddleware> logger,
        IOptions<RateLimitOptions> options,
        ICacheService cacheService)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
        _cacheService = cacheService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_options.Enabled)
        {
            await _next(context);
            return;
        }

        // 检查是否有自定义限流特性
        var endpoint = context.GetEndpoint();
        var rateLimitAttr = endpoint?.Metadata.GetMetadata<RateLimitAttribute>();

        // 获取限流参数
        int limit = rateLimitAttr?.Limit ?? _options.DefaultLimit;
        int window = rateLimitAttr?.Window ?? _options.DefaultWindow;
        string type = rateLimitAttr?.Type ?? "ip";

        // 生成限流Key
        string rateLimitKey = GenerateRateLimitKey(context, type);

        // 检查限流
        if (!await CheckRateLimitAsync(rateLimitKey, limit, window))
        {
            _logger.LogWarning("触发限流: {Key} | 限制: {Limit}/{Window}秒", rateLimitKey, limit, window);

            context.Response.StatusCode = 429; // Too Many Requests
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                Code = 429,
                Type = "error",
                Message = $"请求过于频繁，请{window}秒后再试",
                Result = (object?)null,
                Extras = (object?)null,
                Time = DateTime.Now
            });
            return;
        }

        await _next(context);
    }

    /// <summary>
    /// 生成限流Key
    /// </summary>
    private string GenerateRateLimitKey(HttpContext context, string type)
    {
        string key = type.ToLower() switch
        {
            "user" => GetUserId(context),
            "api" => $"{context.Request.Method}:{context.Request.Path}",
            _ => GetClientIp(context) // default: ip
        };

        return $"{CacheConst.RATE_LIMIT_PREFIX}{type}:{key}";
    }

    /// <summary>
    /// 检查限流（滑动窗口算法）
    /// </summary>
    private Task<bool> CheckRateLimitAsync(string key, int limit, int window)
    {
        var now = DateTime.Now;
        var windowStart = now.AddSeconds(-window);

        // 获取或创建请求历史
        var history = _requestHistory.GetOrAdd(key, _ => new Queue<DateTime>());

        lock (history)
        {
            // 移除窗口外的请求
            while (history.Count > 0 && history.Peek() < windowStart)
            {
                history.Dequeue();
            }

            // 检查是否超过限制
            if (history.Count >= limit)
            {
                return Task.FromResult(false);
            }

            // 记录当前请求
            history.Enqueue(now);
        }

        // 定期清理旧数据
        if (_requestHistory.Count > 10000)
        {
            Task.Run(() => CleanupOldEntries(windowStart));
        }

        return Task.FromResult(true);
    }

    /// <summary>
    /// 清理过期数据
    /// </summary>
    private void CleanupOldEntries(DateTime before)
    {
        var keysToRemove = new List<string>();

        foreach (var kvp in _requestHistory)
        {
            lock (kvp.Value)
            {
                if (kvp.Value.Count == 0 || kvp.Value.All(dt => dt < before))
                {
                    keysToRemove.Add(kvp.Key);
                }
            }
        }

        foreach (var key in keysToRemove)
        {
            _requestHistory.TryRemove(key, out _);
        }
    }

    /// <summary>
    /// 获取客户端IP
    /// </summary>
    private string GetClientIp(HttpContext context)
    {
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                 ?? context.Request.Headers["X-Real-IP"].FirstOrDefault()
                 ?? context.Connection.RemoteIpAddress?.ToString()
                 ?? "unknown";

        return ip.Split(',')[0].Trim();
    }

    /// <summary>
    /// 获取用户ID
    /// </summary>
    private string GetUserId(HttpContext context)
    {
        var userId = context.User.FindFirst(ClaimConst.USER_ID)?.Value;
        return userId ?? GetClientIp(context); // 未登录则使用IP
    }
}
