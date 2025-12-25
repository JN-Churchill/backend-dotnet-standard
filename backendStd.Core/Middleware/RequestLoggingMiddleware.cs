using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using backendStd.Core.Options;
using backendStd.Core.Const;
using System.Diagnostics;
using System.Text;

namespace backendStd.Core.Middleware;

/// <summary>
/// 请求日志中间件
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly RequestLoggingOptions _options;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger,
        IOptions<RequestLoggingOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_options.Enabled)
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var requestTime = DateTime.Now;

        // 读取请求体
        string? requestBody = null;
        if (_options.LogRequestBody && context.Request.ContentLength > 0)
        {
            context.Request.EnableBuffering();
            requestBody = await ReadRequestBodyAsync(context.Request);
            context.Request.Body.Position = 0;
        }

        // 获取用户信息
        var userId = context.User.FindFirst(ClaimConst.USER_ID)?.Value;
        var userName = context.User.FindFirst(ClaimConst.USER_NAME)?.Value;

        // 保存原始响应流
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            // 执行下一个中间件
            await _next(context);

            stopwatch.Stop();

            // 读取响应体
            string? responseBodyText = null;
            if (_options.LogResponseBody && responseBody.Length > 0)
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                responseBodyText = await ReadResponseBodyAsync(responseBody);
                responseBody.Seek(0, SeekOrigin.Begin);
            }

            // 记录日志
            _logger.LogInformation(
                "请求: {Method} {Path} | 状态码: {StatusCode} | 用户: {UserId}/{UserName} | 耗时: {ElapsedMs}ms | 请求体: {RequestBody} | 响应体: {ResponseBody}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                userId ?? "匿名",
                userName ?? "匿名",
                stopwatch.ElapsedMilliseconds,
                requestBody ?? "无",
                responseBodyText ?? "无"
            );

            // 复制响应流到原始流
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex,
                "请求异常: {Method} {Path} | 用户: {UserId}/{UserName} | 耗时: {ElapsedMs}ms",
                context.Request.Method,
                context.Request.Path,
                userId ?? "匿名",
                userName ?? "匿名",
                stopwatch.ElapsedMilliseconds
            );

            // 确保响应流被复制
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);

            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    /// <summary>
    /// 读取请求体
    /// </summary>
    private async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        try
        {
            var buffer = new byte[Math.Min(request.ContentLength ?? 0, _options.MaxRequestBodyLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }
        catch
        {
            return "无法读取";
        }
    }

    /// <summary>
    /// 读取响应体
    /// </summary>
    private async Task<string> ReadResponseBodyAsync(Stream stream)
    {
        try
        {
            var buffer = new byte[Math.Min(stream.Length, _options.MaxResponseBodyLength)];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }
        catch
        {
            return "无法读取";
        }
    }
}
