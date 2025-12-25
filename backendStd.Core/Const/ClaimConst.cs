namespace backendStd.Core.Const;

/// <summary>
/// JWT Claims常量
/// </summary>
public static class ClaimConst
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public const string USER_ID = "UserId";

    /// <summary>
    /// 用户名
    /// </summary>
    public const string USER_NAME = "UserName";

    /// <summary>
    /// 真实姓名
    /// </summary>
    public const string REAL_NAME = "RealName";

    /// <summary>
    /// 手机号
    /// </summary>
    public const string PHONE = "Phone";

    /// <summary>
    /// 邮箱
    /// </summary>
    public const string EMAIL = "Email";

    /// <summary>
    /// 租户ID
    /// </summary>
    public const string TENANT_ID = "TenantId";
}

/// <summary>
/// 缓存Key常量
/// </summary>
public static class CacheConst
{
    /// <summary>
    /// 用户缓存Key前缀
    /// </summary>
    public const string USER_PREFIX = "user:";

    /// <summary>
    /// RefreshToken缓存Key前缀
    /// </summary>
    public const string REFRESH_TOKEN_PREFIX = "refresh_token:";

    /// <summary>
    /// 验证码缓存Key前缀
    /// </summary>
    public const string CAPTCHA_PREFIX = "captcha:";

    /// <summary>
    /// 限流缓存Key前缀
    /// </summary>
    public const string RATE_LIMIT_PREFIX = "rate_limit:";
}
