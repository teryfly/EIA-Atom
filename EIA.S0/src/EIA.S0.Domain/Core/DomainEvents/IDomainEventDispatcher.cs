namespace EIA.S0.Domain.Core.DomainEvents;

public interface IDomainEventDispatcher
{
    /// <summary>
    /// pre dispatch.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    ValueTask PreDispatchAsync(DomainEvent @event, object context);

    /// <summary>
    /// post dispatch.
    /// </summary>
    /// <param name="event"></param>
    ValueTask PostDispatchAsync(DomainEvent @event);
}