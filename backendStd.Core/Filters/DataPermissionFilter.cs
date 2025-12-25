using Microsoft.AspNetCore.Mvc.Filters;
using backendStd.Core.Const;
using backendStd.Core.Enum;

namespace backendStd.Core.Filters;

/// <summary>
/// 数据权限过滤器
/// </summary>
public class DataPermissionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 获取数据权限特性
        var dataPermissionAttr = context.ActionDescriptor.EndpointMetadata
            .OfType<backendStd.Core.Auth.DataPermissionAttribute>()
            .FirstOrDefault();

        if (dataPermissionAttr != null)
        {
            // 获取当前用户信息
            var userId = context.HttpContext.User.FindFirst(ClaimConst.USER_ID)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                // 这里可以根据用户的数据权限范围进行过滤
                // 实际应该通过SqlSugar的过滤器实现
                // 此处仅做示例标记，实际过滤在SqlSugarSetup中配置
                
                context.HttpContext.Items["DataScope"] = dataPermissionAttr.DataScope;
                context.HttpContext.Items["EntityField"] = dataPermissionAttr.EntityField;
                context.HttpContext.Items["UserId"] = userId;
            }
        }

        await next();
    }
}
