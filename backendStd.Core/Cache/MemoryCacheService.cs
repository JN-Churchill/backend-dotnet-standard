using Microsoft.Extensions.Caching.Memory;

namespace backendStd.Core.Cache;

/// <summary>
/// 内存缓存服务实现
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    public Task<T> GetAsync<T>(string key)
    {
        _cache.TryGetValue(key, out T value);
        return Task.FromResult(value);
    }

    /// <summary>
    /// 获取缓存，如果不存在则设置
    /// </summary>
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
    {
        if (_cache.TryGetValue(key, out T value))
        {
            return value;
        }

        value = await factory();
        await SetAsync(key, value, expiry);
        return value;
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new MemoryCacheEntryOptions();
        if (expiry.HasValue)
        {
            options.SetAbsoluteExpiration(expiry.Value);
        }
        else
        {
            options.SetSlidingExpiration(TimeSpan.FromHours(1)); // 默认1小时滑动过期
        }

        _cache.Set(key, value, options);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    public Task<bool> RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.FromResult(true);
    }

    /// <summary>
    /// 判断缓存是否存在
    /// </summary>
    public Task<bool> ExistsAsync(string key)
    {
        return Task.FromResult(_cache.TryGetValue(key, out _));
    }

    /// <summary>
    /// 根据前缀删除缓存（内存缓存不支持，需要维护Key列表）
    /// </summary>
    public Task RemoveByPrefixAsync(string prefix)
    {
        // 内存缓存不支持按前缀删除，需要在应用层维护Key列表
        return Task.CompletedTask;
    }
}
