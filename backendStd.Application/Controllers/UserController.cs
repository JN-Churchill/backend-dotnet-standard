using backendStd.Application.Dtos;
using backendStd.Application.Dtos.User;
using backendStd.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace backendStd.Application.Controllers;

/// <summary>
/// 用户管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    [HttpPost("login")]
    public async Task<LoginOutput> Login([FromBody] LoginInput input)
    {
        return await _userService.LoginAsync(input);
    }

    /// <summary>
    /// 获取用户分页列表
    /// </summary>
    [HttpGet("page")]
    public async Task<PagedResult<UserDto>> GetPaged([FromQuery] PageInput input)
    {
        return await _userService.GetPagedAsync(input);
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    [HttpGet("{id}")]
    public async Task<UserDto> Get(long id)
    {
        return await _userService.GetByIdAsync(id);
    }

    /// <summary>
    /// 新增用户
    /// </summary>
    [HttpPost]
    public async Task<long> Add([FromBody] UserInput input)
    {
        return await _userService.AddAsync(input);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    [HttpPut("{id}")]
    public async Task<bool> Update(long id, [FromBody] UserUpdateInput input)
    {
        return await _userService.UpdateAsync(id, input);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<bool> Delete(long id)
    {
        return await _userService.DeleteAsync(id);
    }

    /// <summary>
    /// 批量删除用户
    /// </summary>
    [HttpDelete("batch")]
    public async Task<bool> BatchDelete([FromBody] List<long> ids)
    {
        return await _userService.BatchDeleteAsync(ids);
    }
}
