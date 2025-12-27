using backendStd.Core.Cache;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;

namespace backendStd.UnitTests.Cache;

/// <summary>
/// 内存缓存服务单元测试
/// </summary>
public class MemoryCacheServiceTests
{
    private readonly MemoryCacheService _cacheService;

    public MemoryCacheServiceTests()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        _cacheService = new MemoryCacheService(memoryCache);
    }

    [Fact(DisplayName = "设置和获取缓存 - 返回正确值")]
    public async Task SetAndGetAsync_ValidKey_ReturnsCorrectValue()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";

        // Act
        await _cacheService.SetAsync(key, value);
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact(DisplayName = "设置和获取复杂对象 - 返回正确对象")]
    public async Task SetAndGetAsync_ComplexObject_ReturnsCorrectObject()
    {
        // Arrange
        var key = "user-key";
        var user = new TestUser
        {
            Id = 123,
            Name = "测试用户",
            Email = "test@example.com"
        };

        // Act
        await _cacheService.SetAsync(key, user);
        var result = await _cacheService.GetAsync<TestUser>(key);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Name.Should().Be(user.Name);
        result.Email.Should().Be(user.Email);
    }

    [Fact(DisplayName = "获取不存在的键 - 返回默认值")]
    public async Task GetAsync_NonExistentKey_ReturnsDefault()
    {
        // Arrange
        var key = "non-existent-key";

        // Act
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact(DisplayName = "删除缓存 - 缓存被移除")]
    public async Task RemoveAsync_ExistingKey_RemovesCache()
    {
        // Arrange
        var key = "remove-test";
        var value = "test-value";
        await _cacheService.SetAsync(key, value);

        // Act
        await _cacheService.RemoveAsync(key);
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact(DisplayName = "检查键是否存在 - 返回true")]
    public async Task ExistsAsync_ExistingKey_ReturnsTrue()
    {
        // Arrange
        var key = "exists-test";
        var value = "test-value";
        await _cacheService.SetAsync(key, value);

        // Act
        var exists = await _cacheService.ExistsAsync(key);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact(DisplayName = "检查键是否存在 - 返回false")]
    public async Task ExistsAsync_NonExistentKey_ReturnsFalse()
    {
        // Arrange
        var key = "non-existent-exists-test";

        // Act
        var exists = await _cacheService.ExistsAsync(key);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact(DisplayName = "设置带过期时间的缓存 - 过期后无法获取")]
    public async Task SetAsync_WithExpiry_ExpiresAfterTime()
    {
        // Arrange
        var key = "expiry-test";
        var value = "test-value";
        var expiry = TimeSpan.FromMilliseconds(100);

        // Act
        await _cacheService.SetAsync(key, value, expiry);
        await Task.Delay(150); // 等待过期
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact(DisplayName = "设置缓存不指定过期时间 - 永久存储")]
    public async Task SetAsync_NoExpiry_PersistsIndefinitely()
    {
        // Arrange
        var key = "no-expiry-test";
        var value = "test-value";

        // Act
        await _cacheService.SetAsync(key, value);
        await Task.Delay(100);
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact(DisplayName = "更新已存在的缓存 - 返回新值")]
    public async Task SetAsync_UpdateExistingKey_ReturnsNewValue()
    {
        // Arrange
        var key = "update-test";
        var oldValue = "old-value";
        var newValue = "new-value";

        // Act
        await _cacheService.SetAsync(key, oldValue);
        await _cacheService.SetAsync(key, newValue);
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        result.Should().Be(newValue);
    }

    // 测试用的辅助类
    private class TestUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
