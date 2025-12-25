namespace backendStd.Application.Dtos.RefreshToken;

/// <summary>
/// RefreshToken输入
/// </summary>
public class RefreshTokenInput
{
    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// 访问令牌（可选，用于解析用户ID）
    /// </summary>
    public string? AccessToken { get; set; }
}
