using Microsoft.Extensions.DependencyInjection;
using System.Runtime.ExceptionServices;
using EIA.S0.Domain.Core.DomainEvents;

namespace EIA.S0.Infrastructure.DomainEventHandlers;

/// <summary>
/// 同步处理事件.
/// </summary>
public class SyncDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// 构造.
    /// </summary>
    /// <param name="provider"></param>
    public SyncDomainEventDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public ValueTask PreDispatchAsync(DomainEvent @event, object context)
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask PostDispatchAsync(DomainEvent @event)
    {
        var eventName = @event.GetType().Name;
        await using var scope = _provider.CreateAsyncScope();
        var handlers = scope.ServiceProvider.GetServices<IDomainEventHandler>().Where(h => h.EventName == eventName);
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
    }
}