namespace EIA.S0.Application.Governance.Events;

/// <summary>
/// PhaseDefinition 变更事件负载.
/// </summary>
public class PhaseChangedEventPayload
{
    /// <summary>
    /// 阶段 Id.
    /// </summary>
    public string PhaseId { get; init; } = string.Empty;

    /// <summary>
    /// 阶段编码.
    /// </summary>
    public string PhaseCode { get; init; } = string.Empty;

    /// <summary>
    /// 显示名称.
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>
    /// 是否可用（当前阶段模型默认 true，预留字段）.
    /// </summary>
    public bool IsActive { get; init; } = true;

    /// <summary>
    /// 操作类型：CREATED / UPDATED / DELETED / SYNC_FULL.
    /// </summary>
    public string OperationType { get; init; } = string.Empty;
}