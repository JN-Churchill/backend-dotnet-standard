namespace backendStd.Core.Options;

/// <summary>
/// 请求日志配置选项
/// </summary>
public class RequestLoggingOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 是否记录请求体
    /// </summary>
    public bool LogRequestBody { get; set; } = true;

    /// <summary>
    /// 是否记录响应体
    /// </summary>
    public bool LogResponseBody { get; set; } = true;

    /// <summary>
    /// 最大请求体长度（字节）
    /// </summary>
    public int MaxRequestBodyLength { get; set; } = 10240; // 10KB

    /// <summary>
    /// 最大响应体长度（字节）
    /// </summary>
    public int MaxResponseBodyLength { get; set; } = 10240; // 10KB
}
