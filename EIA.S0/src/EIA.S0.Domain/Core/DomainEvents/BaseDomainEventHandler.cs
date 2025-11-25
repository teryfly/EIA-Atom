namespace EIA.S0.Domain.Core.DomainEvents;

/// <summary>
/// 领域事件处理基类.
/// </summary>
/// <typeparam name="TDomainEvent"></typeparam>
public abstract class BaseDomainEventHandler<TDomainEvent> : IDomainEventHandler
    where TDomainEvent : DomainEvent
{
    /// <summary>
    /// container.
    /// </summary>
    private IGroupHandlerContainer? _groupHandlerContainer;

    /// <summary>
    /// 事件名称就是泛型的typeName.
    /// </summary>
    public string EventName { get; } = typeof(TDomainEvent).Name;

    /// <summary>
    /// 默认无优先级.
    /// </summary>
    public virtual short Priority { get; } = 0;

    /// <summary>
    /// container.
    /// </summary>
    public IGroupHandlerContainer? Container { get => _groupHandlerContainer; set => _groupHandlerContainer = value; }

    /// <summary>
    /// 基类handle.
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    public async Task HandleAsync(DomainEvent @event, CancellationToken token = default)
    {
        await ExecuteAsync((TDomainEvent)@event);
    }
    
    /// <summary>
    /// 处理具体的领域事件.
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    protected abstract Task ExecuteAsync(TDomainEvent @event);
}