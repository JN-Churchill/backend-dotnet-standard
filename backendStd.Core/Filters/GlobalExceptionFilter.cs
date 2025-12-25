using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using backendStd.Common.Exceptions;
using System.Text.Json;

namespace backendStd.Core.Filters;

/// <summary>
/// 全局异常过滤器
/// </summary>
public class GlobalExceptionFilter : IAsyncExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public Task OnExceptionAsync(ExceptionContext context)
    {
        // 判断异常类型
        var exception = context.Exception;
        var response = new
        {
            Code = 500,
            Type = "error",
            Message = "系统异常",
            Result = (object?)null,
            Extras = (object?)null,
            Time = DateTime.Now
        };

        int statusCode = 500;

        if (exception is BusinessException businessException)
        {
            // 业务异常
            response = new
            {
                Code = 400,
                Type = "error",
                Message = businessException.Message,
                Result = (object?)null,
                Extras = (object?)null,
                Time = DateTime.Now
            };
            statusCode = 400;

            _logger.LogWarning(exception, "业务异常: {Message}", businessException.Message);
        }
        else if (exception is ValidationException validationException)
        {
            // 验证异常
            response = new
            {
                Code = 400,
                Type = "error",
                Message = validationException.Message,
                Result = (object?)null,
                Extras = (object?)null,
                Time = DateTime.Now
            };
            statusCode = 400;

            _logger.LogWarning(exception, "验证异常: {Message}", validationException.Message);
        }
        else if (exception is UnauthorizedAccessException)
        {
            // 未授权异常
            response = new
            {
                Code = 401,
                Type = "error",
                Message = "未授权访问",
                Result = (object?)null,
                Extras = (object?)null,
                Time = DateTime.Now
            };
            statusCode = 401;

            _logger.LogWarning(exception, "未授权访问");
        }
        else
        {
            // 系统异常
            _logger.LogError(exception, "系统异常: {Message}", exception.Message);
        }

        // 设置响应
        context.Result = new ObjectResult(response)
        {
            StatusCode = statusCode
        };

        // 标记异常已处理
        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }
}
