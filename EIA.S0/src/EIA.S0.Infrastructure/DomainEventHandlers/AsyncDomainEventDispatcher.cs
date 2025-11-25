using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.ExceptionServices;
using System.Threading.Channels;
using EIA.S0.Domain.Core.DomainEvents;
using EIA.S0.Domain.IntegrationEventLogs.Aggregates;
using EIA.S0.Infrastructure.EntityFrameworkCore;

namespace EIA.S0.Infrastructure.DomainEventHandlers;

/// <summary>
/// 异步执行.
/// </summary>
public class AsyncDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _provider;
    private readonly Channel<DomainEvent> _domainEventChannel = Channel.CreateUnbounded<DomainEvent>();
    private readonly ILogger<AsyncDomainEventDispatcher> _logger;
    private readonly TimeProvider _timeProvider;

    public AsyncDomainEventDispatcher(IServiceProvider provider, ILogger<AsyncDomainEventDispatcher> logger,
        TimeProvider timeProvider)
    {
        _provider = provider;
        _logger = logger;
        _timeProvider = timeProvider;

        StartHandleChannelEvent();
    }

    public ValueTask PreDispatchAsync(DomainEvent @event, object context)
    {
        if (context is EiaS0dbContext dbContext)
        {
            dbContext.IntegrationEventLogs.Add(new IntegrationEventLog(@event));
        }

        return ValueTask.CompletedTask;
    }

    public async ValueTask PostDispatchAsync(DomainEvent @event)
    {
        await _domainEventChannel.Writer.WriteAsync(@event);
    }

    public void StartHandleChannelEvent()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                var @event = await _domainEventChannel.Reader.ReadAsync();
                var eventName = @event.GetType().Name;
                await using var scope = _provider.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<EiaS0dbContext>();
                IntegrationEventLog? integrationEventLog = null;

                try
                {
                    integrationEventLog = await context.IntegrationEventLogs.FindAsync(@event.Id);
                    if (integrationEventLog != null)
                    {
                        integrationEventLog.InProgress(_timeProvider.GetLocalNow().LocalDateTime);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        _logger?.LogError($"事件[{eventName}-{@event.Id}]未持久化成功.");
                    }

                    // 处理事件
                    var handlers = GetHandler(scope.ServiceProvider, eventName);
                    if (!handlers.Any())
                    {
                        throw new Exception($"事件[{eventName}]未找到对应的处理器");
                    }

                    var exceptions = new List<Exception>();
                    var groupContainer = new DefaultGroupHandlerContainer();
                    foreach (var handler in handlers)
                    {
                        try
                        {
                            // 执行.
                            handler.Container = groupContainer;
                            await handler.HandleAsync(@event, CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            exceptions.Add(ex);
                        }
                    }

                    if (exceptions.Any())
                    {
                        if (exceptions.Count == 1)
                        {
                            ExceptionDispatchInfo.Capture(exceptions[0]).Throw();
                        }
                        else
                        {
                            throw new AggregateException(exceptions);
                        }
                    }

                    if (integrationEventLog != null)
                    {
                        integrationEventLog.Published(_timeProvider.GetLocalNow().LocalDateTime);
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    // 将事件置为发布失败.
                    _logger?.LogError(ex, $"事件[{eventName}-{@event.Identifier}]处理失败！");
                    try
                    {
                        if (integrationEventLog != null)
                        {
                            // 发布失败.
                            integrationEventLog.InError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}",
                                _timeProvider.GetLocalNow().LocalDateTime);
                            await context.SaveChangesAsync();
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        });
    }

    private IEnumerable<IDomainEventHandler> GetHandler(IServiceProvider provider, string eventName)
    {
        return provider.GetServices<IDomainEventHandler>()
            .Where(d => d.EventName == eventName)
            .OrderByDescending(d => d.Priority);
    }
}