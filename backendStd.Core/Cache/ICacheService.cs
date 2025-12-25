namespace backendStd.Core.Cache;

/// <summary>
/// 缓存服务接口
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// 获取缓存
    /// </summary>
    Task<T> GetAsync<T>(string key);

    /// <summary>
    /// 获取缓存，如果不存在则设置
    /// </summary>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null);

    /// <summary>
    /// 设置缓存
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);

    /// <summary>
    /// 删除缓存
    /// </summary>
    Task<bool> RemoveAsync(string key);

    /// <summary>
    /// 判断缓存是否存在
    /// </summary>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// 根据前缀删除缓存
    /// </summary>
    Task RemoveByPrefixAsync(string prefix);
}
