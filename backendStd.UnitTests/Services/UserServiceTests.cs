using backendStd.Application.Dtos.User;
using backendStd.Application.Services;
using backendStd.Common.Exceptions;
using backendStd.Core.Auth;
using backendStd.Core.Cache;
using backendStd.Core.Entity;
using backendStd.Core.Options;
using backendStd.Core.Repository;
using backendStd.Core.Util;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq.Expressions;

namespace backendStd.UnitTests.Services;

/// <summary>
/// 用户服务单元测试
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IRepository<User>> _userRepositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly JwtHandler _jwtHandler; // Changed from Mock to actual instance
    private readonly Mock<IOptions<JWTSettingsOptions>> _jwtOptionsMock;
    private readonly Mock<IOptions<RefreshTokenOptions>> _refreshTokenOptionsMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IRepository<User>>();
        _cacheServiceMock = new Mock<ICacheService>();
        
        // 设置JWT选项
        var jwtOptions = new JWTSettingsOptions
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
        _jwtOptionsMock = new Mock<IOptions<JWTSettingsOptions>>();
        _jwtOptionsMock.Setup(x => x.Value).Returns(jwtOptions);

        // 设置RefreshToken选项
        var refreshTokenOptions = new RefreshTokenOptions { ExpiredTime = 43200 };
        _refreshTokenOptionsMock = new Mock<IOptions<RefreshTokenOptions>>();
        _refreshTokenOptionsMock.Setup(x => x.Value).Returns(refreshTokenOptions);

        // 创建真实的JwtHandler实例
        _jwtHandler = new JwtHandler(_jwtOptionsMock.Object);

        _userService = new UserService(
            _userRepositoryMock.Object,
            _cacheServiceMock.Object,
            _jwtOptionsMock.Object,
            _refreshTokenOptionsMock.Object,
            _jwtHandler
        );
    }

    [Fact(DisplayName = "登录成功 - 返回Token和用户信息")]
    public async Task LoginAsync_ValidCredentials_ReturnsTokenAndUserInfo()
    {
        // Arrange
        var input = new LoginInput
        {
            UserName = "testuser",
            Password = "123456"
        };

        var user = new User
        {
            Id = 123456789,
            UserName = "testuser",
            Password = MD5Helper.Encrypt("123456"),
            RealName = "测试用户",
            Status = 1,
            Phone = "13800138000",
            Email = "test@example.com",
            Avatar = ""
        };

        _userRepositoryMock.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });

        _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(true);

        _cacheServiceMock.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.LoginAsync(input);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.UserInfo.Should().NotBeNull();
        result.UserInfo.UserName.Should().Be("testuser");
        
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        _cacheServiceMock.Verify(x => x.SetAsync(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<TimeSpan?>()), Times.Once);
    }

    [Fact(DisplayName = "登录失败 - 用户不存在")]
    public async Task LoginAsync_UserNotFound_ThrowsBusinessException()
    {
        // Arrange
        var input = new LoginInput
        {
            UserName = "nonexistent",
            Password = "123456"
        };

        _userRepositoryMock.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>());

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _userService.LoginAsync(input);
        });
    }

    [Fact(DisplayName = "登录失败 - 密码错误")]
    public async Task LoginAsync_WrongPassword_ThrowsBusinessException()
    {
        // Arrange
        var input = new LoginInput
        {
            UserName = "testuser",
            Password = "wrongpassword"
        };

        var user = new User
        {
            Id = 123456789,
            UserName = "testuser",
            Password = MD5Helper.Encrypt("correctpassword"),
            Status = 1,
            RealName = "",
            Phone = "",
            Email = "",
            Avatar = ""
        };

        _userRepositoryMock.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _userService.LoginAsync(input);
        });
    }

    [Fact(DisplayName = "登录失败 - 用户已禁用")]
    public async Task LoginAsync_DisabledUser_ThrowsBusinessException()
    {
        // Arrange
        var input = new LoginInput
        {
            UserName = "testuser",
            Password = "123456"
        };

        var user = new User
        {
            Id = 123456789,
            UserName = "testuser",
            Password = MD5Helper.Encrypt("123456"),
            Status = 0, // 禁用状态
            RealName = "",
            Phone = "",
            Email = "",
            Avatar = ""
        };

        _userRepositoryMock.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _userService.LoginAsync(input);
        });
        
        exception.Message.Should().Contain("禁用");
    }

    [Fact(DisplayName = "添加用户 - 成功创建")]
    public async Task AddAsync_ValidUser_ReturnsUserId()
    {
        // Arrange
        var input = new UserInput
        {
            UserName = "newuser",
            Password = "123456",
            RealName = "新用户",
            Phone = "13800138000",
            Email = "newuser@example.com"
        };

        _userRepositoryMock.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>()); // 用户名不存在

        _userRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<User>()))
            .ReturnsAsync(123456789);

        // Act
        var result = await _userService.AddAsync(input);

        // Assert
        result.Should().BeGreaterThan(0);
        _userRepositoryMock.Verify(x => x.InsertAsync(It.Is<User>(u => 
            u.UserName == input.UserName && 
            u.Password == MD5Helper.Encrypt(input.Password))), Times.Once);
    }

    [Fact(DisplayName = "更新用户 - 成功更新")]
    public async Task UpdateAsync_ValidUser_ReturnsTrue()
    {
        // Arrange
        var userId = 123456789L;
        var input = new UserUpdateInput
        {
            RealName = "更新用户",
            Phone = "13900139000",
            Email = "updated@example.com"
        };

        var existingUser = new User
        {
            Id = userId,
            UserName = "olduser",
            Password = MD5Helper.Encrypt("123456"),
            RealName = "旧用户",
            Status = 1,
            Phone = "",
            Email = "",
            Avatar = ""
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateAsync(userId, input);

        // Assert
        result.Should().BeTrue();
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.Is<User>(u => 
            u.Id == userId && 
            u.RealName == input.RealName)), Times.Once);
    }

    [Fact(DisplayName = "删除用户 - 软删除成功")]
    public async Task DeleteAsync_ExistingUser_ReturnsTrue()
    {
        // Arrange
        var userId = 123456789L;

        _userRepositoryMock.Setup(x => x.SoftDeleteAsync(userId))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.DeleteAsync(userId);

        // Assert
        result.Should().BeTrue();
        _userRepositoryMock.Verify(x => x.SoftDeleteAsync(userId), Times.Once);
    }

    [Fact(DisplayName = "获取用户详情 - 返回用户信息")]
    public async Task GetByIdAsync_ExistingUser_ReturnsUserDto()
    {
        // Arrange
        var userId = 123456789L;
        var user = new User
        {
            Id = userId,
            UserName = "testuser",
            RealName = "测试用户",
            Phone = "13800138000",
            Email = "test@example.com",
            Status = 1,
            Password = "",
            Avatar = ""
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.UserName.Should().Be("testuser");
        result.RealName.Should().Be("测试用户");
    }
}
