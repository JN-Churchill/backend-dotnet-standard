namespace backendStd.Common.Exceptions;

/// <summary>
/// 验证异常
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// 验证错误集合
    /// </summary>
    public Dictionary<string, string[]> Errors { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    /// <param name="errors">验证错误集合</param>
    public ValidationException(string message, Dictionary<string, string[]> errors) : base(message)
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }
}
