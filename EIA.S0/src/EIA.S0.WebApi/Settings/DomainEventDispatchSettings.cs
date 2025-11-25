namespace EIA.S0.WebApi.Settings;

/// <summary>
/// 领域事件分发.
/// </summary>
public class DomainEventDispatchSettings
{
    /// <summary>
    /// 处理类型.
    /// </summary>
    public DomainEventDispatchHandleType HandleType { get; set; } = DomainEventDispatchHandleType.Async;

    /// <summary>
    /// 处理类型.
    /// </summary>
    public enum DomainEventDispatchHandleType
    {
        /// <summary>
        /// 异步.
        /// </summary>
        Async,
        /// <summary>
        /// 同步.
        /// </summary>
        Sync
    }
}
