using MediatR;
using Microsoft.Extensions.Logging;
using EIA.S0.Application.Common.Extensions;

namespace EIA.S0.Application.Common.Behaviors;

/// <summary>
/// 日志.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// 构造.
    /// </summary>
    /// <param name="logger"></param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    /// <summary>
    /// handle.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Handling command {CommandName} ({@Command})", request.GetGenericTypeName(),
                request);
        }

        var response = await next(cancellationToken);

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Command {CommandName} handled - response: {@Response}",
                request.GetGenericTypeName(), response);
        }

        return response;
    }
}