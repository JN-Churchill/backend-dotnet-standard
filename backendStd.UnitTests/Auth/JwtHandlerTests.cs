using backendStd.Core.Auth;
using backendStd.Core.Entity;
using backendStd.Core.Options;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace backendStd.UnitTests.Auth;

/// <summary>
/// JWT处理器单元测试
/// </summary>
public class JwtHandlerTests
{
    private readonly JwtHandler _jwtHandler;
    private readonly JWTSettingsOptions _jwtOptions;

    public JwtHandlerTests()
    {
        _jwtOptions = new JWTSettingsOptions
        {
            IssuerSigningKey = "this-is-a-test-key-for-jwt-token-generation-minimum-32-characters",
            ValidIssuer = "test-issuer",
            ValidAudience = "test-audience",
            ExpiredTime = 120,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = 5
        };

        var optionsMock = new Mock<IOptions<JWTSettingsOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_jwtOptions);

        _jwtHandler = new JwtHandler(optionsMock.Object);
    }

    [Fact(DisplayName = "生成AccessToken - 包含用户基本信息")]
    public void GenerateAccessToken_ValidUser_ReturnsToken()
    {
        // Arrange
        var user = new User
        {
            Id = 123456789,
            UserName = "testuser",
            RealName = "测试用户",
            Phone = "13800138000",
            Email = "test@example.com",
            Password = "",
            Avatar = ""
        };

        // Act
        var token = _jwtHandler.GenerateAccessToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        
        // 验证Token格式
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        jwtToken.Should().NotBeNull();
        jwtToken.Claims.Should().Contain(c => c.Type == "UserId" && c.Value == user.Id.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == "UserName" && c.Value == user.UserName);
        jwtToken.Claims.Should().Contain(c => c.Type == "RealName" && c.Value == user.RealName);
        jwtToken.Claims.Should().Contain(c => c.Type == "Phone" && c.Value == user.Phone);
        jwtToken.Claims.Should().Contain(c => c.Type == "Email" && c.Value == user.Email);
    }

    [Fact(DisplayName = "生成AccessToken - 处理可选字段为空")]
    public void GenerateAccessToken_UserWithNullableFields_ReturnsToken()
    {
        // Arrange
        var user = new User
        {
            Id = 123456789,
            UserName = "testuser",
            RealName = "",
            Phone = "",
            Email = "",
            Password = "",
            Avatar = ""
        };

        // Act
        var token = _jwtHandler.GenerateAccessToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        jwtToken.Claims.Should().Contain(c => c.Type == "UserId");
        jwtToken.Claims.Should().Contain(c => c.Type == "UserName");
    }

    [Fact(DisplayName = "生成RefreshToken - 返回GUID格式")]
    public void GenerateRefreshToken_ReturnsValidGuid()
    {
        // Act
        var refreshToken = _jwtHandler.GenerateRefreshToken();

        // Assert
        refreshToken.Should().NotBeNullOrEmpty();
        refreshToken.Should().HaveLength(32); // GUID.ToString("N") 格式长度为32
        refreshToken.Should().MatchRegex("^[a-f0-9]{32}$"); // 只包含小写字母和数字
    }

    [Fact(DisplayName = "生成多个RefreshToken - 每次都不同")]
    public void GenerateRefreshToken_MultipleCalls_ReturnsDifferentTokens()
    {
        // Act
        var token1 = _jwtHandler.GenerateRefreshToken();
        var token2 = _jwtHandler.GenerateRefreshToken();
        var token3 = _jwtHandler.GenerateRefreshToken();

        // Assert
        token1.Should().NotBe(token2);
        token2.Should().NotBe(token3);
        token1.Should().NotBe(token3);
    }

    [Fact(DisplayName = "验证Token - 有效Token返回Claims")]
    public void ValidateToken_ValidToken_ReturnsClaimsPrincipal()
    {
        // Arrange
        var user = new User
        {
            Id = 123456789,
            UserName = "testuser",
            RealName = "测试用户",
            Phone = "13800138000",
            Email = "test@example.com",
            Password = "",
            Avatar = ""
        };
        var token = _jwtHandler.GenerateAccessToken(user);

        // Act
        var principal = _jwtHandler.ValidateToken(token);

        // Assert
        principal.Should().NotBeNull();
        principal!.Claims.Should().Contain(c => c.Type == "UserId" && c.Value == user.Id.ToString());
        principal.Claims.Should().Contain(c => c.Type == "UserName" && c.Value == user.UserName);
    }

    [Fact(DisplayName = "验证Token - 无效Token返回null")]
    public void ValidateToken_InvalidToken_ReturnsNull()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var principal = _jwtHandler.ValidateToken(invalidToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact(DisplayName = "验证Token - 空Token返回null")]
    public void ValidateToken_EmptyToken_ReturnsNull()
    {
        // Arrange
        var emptyToken = "";

        // Act
        var principal = _jwtHandler.ValidateToken(emptyToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact(DisplayName = "从Claims获取用户ID - 返回正确ID")]
    public void GetUserIdFromClaims_ValidPrincipal_ReturnsUserId()
    {
        // Arrange
        var user = new User
        {
            Id = 123456789,
            UserName = "testuser",
            RealName = "",
            Phone = "",
            Email = "",
            Password = "",
            Avatar = ""
        };
        var token = _jwtHandler.GenerateAccessToken(user);
        var principal = _jwtHandler.ValidateToken(token);

        // Act
        var userIdClaim = principal?.Claims.FirstOrDefault(c => c.Type == "UserId");

        // Assert
        userIdClaim.Should().NotBeNull();
        userIdClaim!.Value.Should().Be(user.Id.ToString());
    }
}
