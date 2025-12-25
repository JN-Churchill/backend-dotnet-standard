namespace backendStd.Core.Options;

/// <summary>
/// Redis配置选项
/// </summary>
public class RedisOptions
{
    /// <summary>
    /// Redis连接配置字符串
    /// 格式: localhost:6379,password=,defaultDatabase=0
    /// </summary>
    public string Configuration { get; set; }

    /// <summary>
    /// 实例名称（用作Key前缀）
    /// </summary>
    public string InstanceName { get; set; }
}
