namespace EIA.S0.Domain.Core.DomainEvents;

/// <summary>
/// 领域事件处理器.
/// </summary>
public interface IDomainEventHandler
{
    /// <summary>
    /// 事件名称.
    /// </summary>
    string EventName { get; }

    /// <summary>
    /// 优先级.
    /// </summary>
    short Priority { get; }

    /// <summary>
    /// 处理函数.
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    Task HandleAsync(DomainEvent @event, CancellationToken token = default);

    /// <summary>
    /// container.
    /// </summary>
    IGroupHandlerContainer? Container { get; set; }
}