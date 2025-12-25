using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using backendStd.Core.Const;
using backendStd.Core.Repository;
using backendStd.Core.Entity;
using Microsoft.Extensions.DependencyInjection;

namespace backendStd.Core.Auth;

/// <summary>
/// 权限验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    /// <summary>
    /// 权限编码
    /// </summary>
    public string PermissionCode { get; set; }

    public RequirePermissionAttribute(string permissionCode)
    {
        PermissionCode = permissionCode;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // 获取用户ID
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimConst.USER_ID);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            context.Result = new UnauthorizedObjectResult(new { Message = "未授权访问" });
            return;
        }

        // 获取用户的所有角色
        var userRoleRepository = context.HttpContext.RequestServices.GetRequiredService<IRepository<UserRole>>();
        var userRoles = await userRoleRepository.GetListAsync(ur => ur.UserId == userId);
        
        if (!userRoles.Any())
        {
            context.Result = new ForbidResult();
            return;
        }

        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

        // 获取角色的所有权限
        var rolePermissionRepository = context.HttpContext.RequestServices.GetRequiredService<IRepository<RolePermission>>();
        var rolePermissions = await rolePermissionRepository.GetListAsync(rp => roleIds.Contains(rp.RoleId));
        
        if (!rolePermissions.Any())
        {
            context.Result = new ForbidResult();
            return;
        }

        var permissionIds = rolePermissions.Select(rp => rp.PermissionId).ToList();

        // 检查是否有指定权限
        var permissionRepository = context.HttpContext.RequestServices.GetRequiredService<IRepository<Permission>>();
        var hasPermission = (await permissionRepository.GetListAsync(p => 
            permissionIds.Contains(p.Id) && p.PermissionCode == PermissionCode && p.Status == 1
        )).Any();

        if (!hasPermission)
        {
            context.Result = new ObjectResult(new { Message = "无权限访问" })
            {
                StatusCode = 403
            };
        }
    }
}
