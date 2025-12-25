namespace backendStd.Core.Options;

/// <summary>
/// JWT配置选项
/// </summary>
public class JWTSettingsOptions
{
    /// <summary>
    /// 是否验证签名密钥
    /// </summary>
    public bool ValidateIssuerSigningKey { get; set; } = true;

    /// <summary>
    /// 签名密钥（至少16个字符）
    /// </summary>
    public string IssuerSigningKey { get; set; }

    /// <summary>
    /// 是否验证发行者
    /// </summary>
    public bool ValidateIssuer { get; set; } = true;

    /// <summary>
    /// 发行者
    /// </summary>
    public string ValidIssuer { get; set; }

    /// <summary>
    /// 是否验证受众
    /// </summary>
    public bool ValidateAudience { get; set; } = true;

    /// <summary>
    /// 受众
    /// </summary>
    public string ValidAudience { get; set; }

    /// <summary>
    /// 是否验证生命周期
    /// </summary>
    public bool ValidateLifetime { get; set; } = true;

    /// <summary>
    /// 过期时间（分钟）
    /// </summary>
    public long ExpiredTime { get; set; } = 120;

    /// <summary>
    /// 时钟偏差（分钟）
    /// </summary>
    public long ClockSkew { get; set; } = 5;
}

/// <summary>
/// RefreshToken配置选项
/// </summary>
public class RefreshTokenOptions
{
    /// <summary>
    /// 过期时间（分钟）
    /// </summary>
    public long ExpiredTime { get; set; } = 43200; // 30天
}
