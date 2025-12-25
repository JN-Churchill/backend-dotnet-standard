using backendStd.Application.Dtos;
using backendStd.Application.Services;
using backendStd.Core.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backendStd.Application.Controllers;

/// <summary>
/// 角色管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly RoleService _roleService;

    public RoleController(RoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// 获取角色分页列表
    /// </summary>
    [HttpGet("page")]
    public async Task<PagedResult<Role>> GetPaged([FromQuery] PageInput input)
    {
        return await _roleService.GetPagedAsync(input);
    }

    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    [HttpGet("{id}")]
    public async Task<Role> Get(long id)
    {
        return await _roleService.GetByIdAsync(id);
    }

    /// <summary>
    /// 新增角色
    /// </summary>
    [HttpPost]
    public async Task<long> Add([FromBody] Role input)
    {
        return await _roleService.AddAsync(input);
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    [HttpPut("{id}")]
    public async Task<bool> Update(long id, [FromBody] Role input)
    {
        return await _roleService.UpdateAsync(id, input);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<bool> Delete(long id)
    {
        return await _roleService.DeleteAsync(id);
    }

    /// <summary>
    /// 为角色分配权限
    /// </summary>
    [HttpPost("{id}/permissions")]
    public async Task<bool> AssignPermissions(long id, [FromBody] List<long> permissionIds)
    {
        return await _roleService.AssignPermissionsAsync(id, permissionIds);
    }

    /// <summary>
    /// 获取角色的权限列表
    /// </summary>
    [HttpGet("{id}/permissions")]
    public async Task<List<long>> GetRolePermissions(long id)
    {
        return await _roleService.GetRolePermissionsAsync(id);
    }
}
