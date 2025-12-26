using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace backendStd.Core.Middleware;

/// <summary>
/// 开发环境认证后门中间件
/// </summary>
public class DevBypassMiddleware
{
    private readonly RequestDelegate _next;

    public DevBypassMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        
        if (authHeader != null)
        {
            var token = authHeader.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();
            
            if (token == "driver")
            {
                // 创建开发后门用户身份
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "999999"),
                    new Claim(Const.ClaimConst.USER_ID, "999999"),
                    new Claim(Const.ClaimConst.USER_NAME, "driver"),
                    new Claim(ClaimTypes.Name, "driver"),
                    new Claim(Const.ClaimConst.REAL_NAME, "开发后门")
                };
                
                var identity = new ClaimsIdentity(claims, "DevBypass");
                context.User = new ClaimsPrincipal(identity);
            }
        }
        
        await _next(context);
    }
}
