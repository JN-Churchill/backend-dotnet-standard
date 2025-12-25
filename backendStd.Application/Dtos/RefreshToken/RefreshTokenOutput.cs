namespace backendStd.Application.Dtos.RefreshToken;

/// <summary>
/// RefreshToken输出
/// </summary>
public class RefreshTokenOutput
{
    /// <summary>
    /// 新的访问令牌
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 新的刷新令牌
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// 过期时间（分钟）
    /// </summary>
    public long ExpiredTime { get; set; }
}
