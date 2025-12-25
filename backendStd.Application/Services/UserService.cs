using backendStd.Application.Dtos;
using backendStd.Application.Dtos.User;
using backendStd.Core.Cache;
using backendStd.Core.Const;
using backendStd.Core.Entity;
using backendStd.Core.Options;
using backendStd.Core.Repository;
using backendStd.Core.Util;
using Mapster;
using Microsoft.Extensions.Options;

namespace backendStd.Application.Services;

/// <summary>
/// 用户服务
/// </summary>
public class UserService
{
    private readonly IRepository<User> _userRepository;
    private readonly ICacheService _cacheService;
    private readonly JWTSettingsOptions _jwtOptions;

    public UserService(
        IRepository<User> userRepository,
        ICacheService cacheService,
        IOptions<JWTSettingsOptions> jwtOptions)
    {
        _userRepository = userRepository;
        _cacheService = cacheService;
        _jwtOptions = jwtOptions.Value;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    public async Task<LoginOutput> LoginAsync(LoginInput input)
    {
        // 查询用户
        var users = await _userRepository.GetListAsync(u => u.UserName == input.UserName);
        var user = users.FirstOrDefault();

        if (user == null)
            throw new Common.Exceptions.BusinessException("用户名或密码错误");

        // 验证密码（MD5）
        var passwordHash = MD5Helper.Encrypt(input.Password);
        if (user.Password != passwordHash)
            throw new Common.Exceptions.BusinessException("用户名或密码错误");

        // 检查状态
        if (user.Status == 0)
            throw new Common.Exceptions.BusinessException("用户已被禁用");

        // 更新登录信息
        user.LastLoginTime = DateTime.Now;
        // user.LastLoginIp = 从HttpContext获取;
        await _userRepository.UpdateAsync(user);

        // 生成Token（简化版，实际应该使用JWT库）
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken(user);

        // 缓存RefreshToken
        await _cacheService.SetAsync(
            $"{CacheConst.REFRESH_TOKEN_PREFIX}{user.Id}",
            refreshToken,
            TimeSpan.FromMinutes(_jwtOptions.ExpiredTime * 30) // RefreshToken有效期更长
        );

        return new LoginOutput
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiredTime = _jwtOptions.ExpiredTime,
            UserInfo = user.Adapt<UserDto>()
        };
    }

    /// <summary>
    /// 获取用户分页列表
    /// </summary>
    public async Task<PagedResult<UserDto>> GetPagedAsync(PageInput input)
    {
        var (items, total) = await _userRepository.GetPagedAsync(input.Page, input.PageSize);
        
        return new PagedResult<UserDto>
        {
            Items = items.Adapt<List<UserDto>>(),
            Total = total,
            Page = input.Page,
            PageSize = input.PageSize
        };
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    public async Task<UserDto> GetByIdAsync(long id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new Common.Exceptions.BusinessException("用户不存在");

        return user.Adapt<UserDto>();
    }

    /// <summary>
    /// 新增用户
    /// </summary>
    public async Task<long> AddAsync(UserInput input)
    {
        // 检查用户名是否存在
        var existUsers = await _userRepository.GetListAsync(u => u.UserName == input.UserName);
        if (existUsers.Any())
            throw new Common.Exceptions.BusinessException("用户名已存在");

        var user = input.Adapt<User>();
        user.Password = MD5Helper.Encrypt(input.Password); // 密码MD5加密

        return await _userRepository.InsertAsync(user);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    public async Task<bool> UpdateAsync(long id, UserUpdateInput input)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new Common.Exceptions.BusinessException("用户不存在");

        // 映射更新字段
        input.Adapt(user);

        return await _userRepository.UpdateAsync(user);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    public async Task<bool> DeleteAsync(long id)
    {
        return await _userRepository.SoftDeleteAsync(id);
    }

    /// <summary>
    /// 批量删除用户
    /// </summary>
    public async Task<bool> BatchDeleteAsync(List<long> ids)
    {
        return await _userRepository.SoftDeleteAsync(ids);
    }

    /// <summary>
    /// 生成AccessToken（简化版，实际应该使用JWT库）
    /// </summary>
    private string GenerateAccessToken(User user)
    {
        // TODO: 实际应该使用System.IdentityModel.Tokens.Jwt生成标准JWT Token
        return $"Bearer_{user.Id}_{Guid.NewGuid():N}";
    }

    /// <summary>
    /// 生成RefreshToken
    /// </summary>
    private string GenerateRefreshToken(User user)
    {
        return Guid.NewGuid().ToString("N");
    }
}
