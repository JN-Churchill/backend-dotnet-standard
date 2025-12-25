using backendStd.Application.Dtos;
using backendStd.Application.Services;
using backendStd.Core.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backendStd.Application.Controllers;

/// <summary>
/// 租户管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TenantController : ControllerBase
{
    private readonly TenantService _tenantService;

    public TenantController(TenantService tenantService)
    {
        _tenantService = tenantService;
    }

    /// <summary>
    /// 获取租户分页列表
    /// </summary>
    [HttpGet("page")]
    public async Task<PagedResult<Tenant>> GetPaged([FromQuery] PageInput input)
    {
        return await _tenantService.GetPagedAsync(input);
    }

    /// <summary>
    /// 根据ID获取租户
    /// </summary>
    [HttpGet("{id}")]
    public async Task<Tenant> Get(long id)
    {
        return await _tenantService.GetByIdAsync(id);
    }

    /// <summary>
    /// 新增租户
    /// </summary>
    [HttpPost]
    public async Task<long> Add([FromBody] Tenant input)
    {
        return await _tenantService.AddAsync(input);
    }

    /// <summary>
    /// 更新租户
    /// </summary>
    [HttpPut("{id}")]
    public async Task<bool> Update(long id, [FromBody] Tenant input)
    {
        return await _tenantService.UpdateAsync(id, input);
    }

    /// <summary>
    /// 删除租户
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<bool> Delete(long id)
    {
        return await _tenantService.DeleteAsync(id);
    }

    /// <summary>
    /// 根据租户编码获取租户
    /// </summary>
    [HttpGet("code/{tenantCode}")]
    public async Task<Tenant?> GetByCode(string tenantCode)
    {
        return await _tenantService.GetByCodeAsync(tenantCode);
    }
}
