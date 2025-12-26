using backendStd.Application.Dtos;
using backendStd.Application.Dtos.Department;
using backendStd.Application.Dtos.User;
using backendStd.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace backendStd.Application.Controllers;

/// <summary>
/// 部门管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly DepartmentService _departmentService;

    public DepartmentController(DepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    /// <summary>
    /// 获取部门分页列表
    /// </summary>
    [HttpGet("page")]
    public async Task<PagedResult<DepartmentDto>> GetPaged([FromQuery] PageInput input)
    {
        return await _departmentService.GetPagedAsync(input);
    }

    /// <summary>
    /// 获取部门树形结构
    /// </summary>
    [HttpGet("tree")]
    public async Task<List<DepartmentDto>> GetTree()
    {
        return await _departmentService.GetTreeAsync();
    }

    /// <summary>
    /// 根据ID获取部门
    /// </summary>
    [HttpGet("{id}")]
    public async Task<DepartmentDto> Get(long id)
    {
        return await _departmentService.GetByIdAsync(id);
    }

    /// <summary>
    /// 新增部门
    /// </summary>
    [HttpPost]
    public async Task<long> Add([FromBody] DepartmentInput input)
    {
        return await _departmentService.AddAsync(input);
    }

    /// <summary>
    /// 更新部门
    /// </summary>
    [HttpPut("{id}")]
    public async Task<bool> Update(long id, [FromBody] DepartmentUpdateInput input)
    {
        return await _departmentService.UpdateAsync(id, input);
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<bool> Delete(long id)
    {
        return await _departmentService.DeleteAsync(id);
    }

    /// <summary>
    /// 获取部门下的用户列表
    /// </summary>
    [HttpGet("{id}/users")]
    public async Task<List<UserDto>> GetDepartmentUsers(long id)
    {
        return await _departmentService.GetDepartmentUsersAsync(id);
    }
}
