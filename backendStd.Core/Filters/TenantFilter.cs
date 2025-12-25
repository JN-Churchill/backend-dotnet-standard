using Microsoft.AspNetCore.Mvc.Filters;

namespace backendStd.Core.Filters;

/// <summary>
/// 租户过滤器
/// </summary>
public class TenantFilter : IAsyncActionFilter
{
    private readonly Auth.TenantContext _tenantContext;

    public TenantFilter(Auth.TenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 获取当前租户ID
        var tenantId = _tenantContext.GetCurrentTenantId();

        if (tenantId.HasValue)
        {
            // 将租户ID存储到HttpContext.Items中，供后续使用
            context.HttpContext.Items["TenantId"] = tenantId.Value;
        }

        await next();
    }
}
