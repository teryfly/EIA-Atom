namespace EIA.S0.Application.Governance.Events;

/// <summary>
/// DocType 变更事件负载.
/// </summary>
public class DocTypeChangedEventPayload
{
    public string DocTypeId { get; init; } = string.Empty;
    public string DocTypeCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Status { get; init; } = "ENABLED";
    public string OperationType { get; init; } = string.Empty;
}

/// <summary>
/// 统一事件 Envelope.
/// </summary>
/// <typeparam name="TPayload"></typeparam>
public class GovernanceEventEnvelope<TPayload>
    where TPayload : class, new()
{
    public string EventId { get; init; } = string.Empty;
    public string EventType { get; init; } = string.Empty;
    public string Source { get; init; } = "governance-bc";
    public DateTime OccurredAt { get; init; }
    public int Version { get; init; } = 1;
    public TPayload Payload { get; init; } = new();
}