using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using EIA.S0.Contracts.Dtos;
using EIA.S0.Domain.Core.Exceptions;

namespace EIA.S0.WebApi.Middlewares;

/// <summary>
/// 全局异常处理.
/// </summary>
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptions<JsonOptions> _jsonOptions;
    private readonly ILogger _logger;

    /// <summary>
    /// 构造.
    /// </summary>
    /// <param name="next"></param>
    /// <param name="jsonOptions"></param>
    /// <param name="logger"></param>
    public ExceptionHandlerMiddleware(RequestDelegate next, IOptions<JsonOptions> jsonOptions,
        ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _jsonOptions = jsonOptions;
        _logger = logger;
    }

    /// <summary>
    /// invoke.
    /// </summary>
    /// <param name="context"></param>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "服务器出错.");
            bool isClientError = typeof(DomainException).IsAssignableFrom(ex.GetType());
            context.Response.ContentType = "application/json";
            if (isClientError)
            {
                context.Response.StatusCode = 400;
                await JsonSerializer.SerializeAsync(context.Response.Body, ResultDto.Fail(ex.Message),
                    _jsonOptions.Value.SerializerOptions);
            }
            else
            {
                context.Response.StatusCode = 500;
                await JsonSerializer.SerializeAsync(context.Response.Body, ResultDto.Error(),
                    _jsonOptions.Value.SerializerOptions);
            }
        }
    }
}