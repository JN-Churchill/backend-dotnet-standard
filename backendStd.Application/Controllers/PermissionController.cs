using backendStd.Application.Services;
using backendStd.Core.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backendStd.Application.Controllers;

/// <summary>
/// 权限管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionController : ControllerBase
{
    private readonly PermissionService _permissionService;

    public PermissionController(PermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    /// <summary>
    /// 获取权限列表
    /// </summary>
    [HttpGet("list")]
    public async Task<List<Permission>> GetList()
    {
        return await _permissionService.GetListAsync();
    }

    /// <summary>
    /// 获取权限树形结构
    /// </summary>
    [HttpGet("tree")]
    public async Task<List<Permission>> GetTree()
    {
        return await _permissionService.GetTreeAsync();
    }

    /// <summary>
    /// 根据ID获取权限
    /// </summary>
    [HttpGet("{id}")]
    public async Task<Permission> Get(long id)
    {
        return await _permissionService.GetByIdAsync(id);
    }

    /// <summary>
    /// 新增权限
    /// </summary>
    [HttpPost]
    public async Task<long> Add([FromBody] Permission input)
    {
        return await _permissionService.AddAsync(input);
    }

    /// <summary>
    /// 更新权限
    /// </summary>
    [HttpPut("{id}")]
    public async Task<bool> Update(long id, [FromBody] Permission input)
    {
        return await _permissionService.UpdateAsync(id, input);
    }

    /// <summary>
    /// 删除权限
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<bool> Delete(long id)
    {
        return await _permissionService.DeleteAsync(id);
    }
}
