using backendStd.Core.Util;
using StackExchange.Redis;

namespace backendStd.Core.Cache;

/// <summary>
/// Redis缓存服务实现
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = redis.GetDatabase();
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    public async Task<T> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (!value.HasValue)
            return default;

        return JsonHelper.Deserialize<T>(value);
    }

    /// <summary>
    /// 获取缓存，如果不存在则设置
    /// </summary>
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
    {
        var value = await GetAsync<T>(key);
        if (value != null)
            return value;

        value = await factory();
        await SetAsync(key, value, expiry);
        return value;
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonHelper.Serialize(value);
        await _db.StringSetAsync(key, json, expiry);
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    public async Task<bool> RemoveAsync(string key)
    {
        return await _db.KeyDeleteAsync(key);
    }

    /// <summary>
    /// 判断缓存是否存在
    /// </summary>
    public async Task<bool> ExistsAsync(string key)
    {
        return await _db.KeyExistsAsync(key);
    }

    /// <summary>
    /// 根据前缀删除缓存
    /// </summary>
    public async Task RemoveByPrefixAsync(string prefix)
    {
        var endpoints = _redis.GetEndPoints();
        var server = _redis.GetServer(endpoints.First());
        
        var keys = server.Keys(pattern: $"{prefix}*").ToArray();
        if (keys.Length > 0)
        {
            await _db.KeyDeleteAsync(keys);
        }
    }
}
