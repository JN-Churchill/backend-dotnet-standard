using Furion;
using Furion.DataValidation;
using Furion.FriendlyException;
using Furion.JsonSerialization;
using Furion.UnifyResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace backendStd.Core.Util;

/// <summary>
/// 统一返回结果提供器
/// 说明：实现Furion的IUnifyResultProvider接口，统一处理API返回格式
/// </summary>
[UnifyModel(typeof(TdivsResult<>))]
public class TdivsResultProvider : IUnifyResultProvider
{
    /// <summary>
    /// 异常返回处理
    /// </summary>
    public IActionResult OnException(ExceptionContext context, ExceptionMetadata metadata)
    {
        return new JsonResult(
            RestfulResult(
                metadata.StatusCode,
                data: metadata.Data,
                errors: metadata.Errors
            )
        );
    }

    /// <summary>
    /// 授权异常处理
    /// </summary>
    public IActionResult OnAuthorizeException(DefaultHttpContext context, ExceptionMetadata metadata)
    {
        return new JsonResult(
            RestfulResult(
                metadata.StatusCode,
                data: metadata.Data,
                errors: metadata.Errors
            )
        );
    }

    /// <summary>
    /// 成功返回处理
    /// </summary>
    public IActionResult OnSucceeded(ActionExecutedContext context, object data)
    {
        return new JsonResult(RestfulResult(StatusCodes.Status200OK, true, data));
    }

    /// <summary>
    /// 验证失败返回处理
    /// </summary>
    public IActionResult OnValidateFailed(ActionExecutingContext context, ValidationMetadata metadata)
    {
        return new JsonResult(
            RestfulResult(
                metadata.StatusCode ?? StatusCodes.Status400BadRequest,
                data: metadata.Data,
                errors: metadata.ValidationResult
            )
        );
    }

    /// <summary>
    /// 特殊状态码处理（401、403等）
    /// </summary>
    public async Task OnResponseStatusCodes(HttpContext context, int statusCode, UnifyResultSettingsOptions unifyResultSettings)
    {
        UnifyContext.SetResponseStatusCodes(context, statusCode, unifyResultSettings);

        switch (statusCode)
        {
            case StatusCodes.Status401Unauthorized:
                await context.Response.WriteAsJsonAsync(
                    RestfulResult(statusCode, errors: "401登录已过期，请重新登录"),
                    App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                break;
            case StatusCodes.Status403Forbidden:
                await context.Response.WriteAsJsonAsync(
                    RestfulResult(statusCode, errors: "403禁止访问，没有权限"),
                    App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 构建统一返回结果
    /// </summary>
    private static TdivsResult<object> RestfulResult(int statusCode, bool succeeded = default, object data = default, object errors = default)
    {
        return new TdivsResult<object>
        {
            Code = statusCode,
            Message = errors is string str ? str : JSON.Serialize(errors),
            Result = data,
            Type = succeeded ? "success" : "error",
            Extras = UnifyContext.Take(),
            Time = DateTime.Now
        };
    }
}

/// <summary>
/// 全局统一返回结果模型
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class TdivsResult<T>
{
    /// <summary>
    /// 状态码
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 类型：success/warning/error
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 提示信息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 返回数据
    /// </summary>
    public T Result { get; set; }

    /// <summary>
    /// 附加数据（如追踪信息等）
    /// </summary>
    public object Extras { get; set; }

    /// <summary>
    /// 响应时间
    /// </summary>
    public DateTime Time { get; set; }
}
