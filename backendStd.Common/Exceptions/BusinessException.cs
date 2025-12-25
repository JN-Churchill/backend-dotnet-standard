namespace backendStd.Common.Exceptions;

/// <summary>
/// 业务异常
/// </summary>
public class BusinessException : Exception
{
    /// <summary>
    /// 错误码
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    /// <param name="errorCode">错误码，默认400</param>
    public BusinessException(string message, int errorCode = 400) : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    /// <param name="innerException">内部异常</param>
    /// <param name="errorCode">错误码，默认400</param>
    public BusinessException(string message, Exception innerException, int errorCode = 400) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
