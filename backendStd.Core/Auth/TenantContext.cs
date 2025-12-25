using Microsoft.AspNetCore.Http;
using backendStd.Core.Const;

namespace backendStd.Core.Auth;

/// <summary>
/// 租户上下文
/// </summary>
public class TenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// 获取当前租户ID
    /// </summary>
    public long? GetCurrentTenantId()
    {
        var tenantIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimConst.TENANT_ID);
        
        if (tenantIdClaim != null && long.TryParse(tenantIdClaim.Value, out var tenantId))
        {
            return tenantId;
        }

        // 也可以从Header中获取
        var tenantIdHeader = _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (!string.IsNullOrEmpty(tenantIdHeader) && long.TryParse(tenantIdHeader, out var headerTenantId))
        {
            return headerTenantId;
        }

        return null;
    }

    /// <summary>
    /// 设置租户ID到HttpContext
    /// </summary>
    public void SetTenantId(long tenantId)
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            _httpContextAccessor.HttpContext.Items["TenantId"] = tenantId;
        }
    }
}
