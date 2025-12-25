using backendStd.Application.Dtos;
using backendStd.Core.Entity;
using backendStd.Core.Repository;
using Mapster;

namespace backendStd.Application.Services;

/// <summary>
/// 租户服务
/// </summary>
public class TenantService
{
    private readonly IRepository<Tenant> _tenantRepository;

    public TenantService(IRepository<Tenant> tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    /// <summary>
    /// 获取租户分页列表
    /// </summary>
    public async Task<PagedResult<Tenant>> GetPagedAsync(PageInput input)
    {
        var (items, total) = await _tenantRepository.GetPagedAsync(input.Page, input.PageSize);
        
        return new PagedResult<Tenant>
        {
            Items = items,
            Total = total,
            Page = input.Page,
            PageSize = input.PageSize
        };
    }

    /// <summary>
    /// 根据ID获取租户
    /// </summary>
    public async Task<Tenant> GetByIdAsync(long id)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id);
        if (tenant == null)
            throw new Common.Exceptions.BusinessException("租户不存在");

        return tenant;
    }

    /// <summary>
    /// 新增租户
    /// </summary>
    public async Task<long> AddAsync(Tenant input)
    {
        // 检查租户编码是否存在
        var existTenants = await _tenantRepository.GetListAsync(t => t.TenantCode == input.TenantCode);
        if (existTenants.Any())
            throw new Common.Exceptions.BusinessException("租户编码已存在");

        return await _tenantRepository.InsertAsync(input);
    }

    /// <summary>
    /// 更新租户
    /// </summary>
    public async Task<bool> UpdateAsync(long id, Tenant input)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id);
        if (tenant == null)
            throw new Common.Exceptions.BusinessException("租户不存在");

        input.Adapt(tenant);
        return await _tenantRepository.UpdateAsync(tenant);
    }

    /// <summary>
    /// 删除租户
    /// </summary>
    public async Task<bool> DeleteAsync(long id)
    {
        return await _tenantRepository.SoftDeleteAsync(id);
    }

    /// <summary>
    /// 根据租户编码获取租户
    /// </summary>
    public async Task<Tenant?> GetByCodeAsync(string tenantCode)
    {
        var tenants = await _tenantRepository.GetListAsync(t => t.TenantCode == tenantCode);
        return tenants.FirstOrDefault();
    }
}
