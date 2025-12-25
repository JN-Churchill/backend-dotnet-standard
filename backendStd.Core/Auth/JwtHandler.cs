using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backendStd.Core.Const;
using backendStd.Core.Entity;
using backendStd.Core.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace backendStd.Core.Auth;

/// <summary>
/// JWT处理器
/// </summary>
public class JwtHandler
{
    private readonly JWTSettingsOptions _jwtOptions;

    public JwtHandler(IOptions<JWTSettingsOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    /// <summary>
    /// 生成AccessToken
    /// </summary>
    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimConst.USER_ID, user.Id.ToString()),
            new Claim(ClaimConst.USER_NAME, user.UserName),
            new Claim(ClaimTypes.Name, user.UserName),
        };

        // 添加可选字段
        if (!string.IsNullOrEmpty(user.RealName))
            claims.Add(new Claim(ClaimConst.REAL_NAME, user.RealName));

        if (!string.IsNullOrEmpty(user.Phone))
            claims.Add(new Claim(ClaimConst.PHONE, user.Phone));

        if (!string.IsNullOrEmpty(user.Email))
            claims.Add(new Claim(ClaimConst.EMAIL, user.Email));

        return GenerateToken(claims, _jwtOptions.ExpiredTime);
    }

    /// <summary>
    /// 生成RefreshToken
    /// </summary>
    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// 验证Token
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.IssuerSigningKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = _jwtOptions.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = _jwtOptions.ValidateIssuer,
                ValidIssuer = _jwtOptions.ValidIssuer,
                ValidateAudience = _jwtOptions.ValidateAudience,
                ValidAudience = _jwtOptions.ValidAudience,
                ValidateLifetime = _jwtOptions.ValidateLifetime,
                ClockSkew = TimeSpan.FromMinutes(_jwtOptions.ClockSkew)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 从Token中获取用户ID
    /// </summary>
    public long? GetUserIdFromToken(string token)
    {
        var principal = ValidateToken(token);
        if (principal == null)
            return null;

        var userIdClaim = principal.FindFirst(ClaimConst.USER_ID);
        if (userIdClaim == null)
            return null;

        return long.TryParse(userIdClaim.Value, out var userId) ? userId : null;
    }

    /// <summary>
    /// 生成Token（内部方法）
    /// </summary>
    private string GenerateToken(List<Claim> claims, long expiredMinutes)
    {
        var key = Encoding.UTF8.GetBytes(_jwtOptions.IssuerSigningKey);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.ValidIssuer,
            audience: _jwtOptions.ValidAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiredMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
