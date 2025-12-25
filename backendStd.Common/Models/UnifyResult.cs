namespace backendStd.Common.Models;

/// <summary>
/// 统一返回结果模型（已弃用，使用 TdivsResult 替代）
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
[Obsolete("请使用 Core.Util.TdivsResult<T> 替代")]
public class UnifyResult<T>
{
    /// <summary>
    /// 状态码
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 数据
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}
