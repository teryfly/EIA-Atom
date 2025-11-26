using EIA.S0.Application.Governance.Cache;
using EIA.S0.Application.Governance.Events;
using EIA.S0.Domain.Governance.Entities;

namespace EIA.S0.Application.Governance.Phases;

/// <summary>
/// PhaseService 事件与缓存集成部分.
/// </summary>
public partial class PhaseService
{
    private const string PhaseChangedTopic = "governance.phase.changed.v1";
    private const string CacheRefreshTopic = "governance.cache.refresh.v1";

    private readonly PhaseCache _cache = default!;
    private readonly Func<string, object, CancellationToken, Task> _eventPublishFunc = default!;

    /// <summary>
    /// 扩展构造函数，注入缓存与事件发布器.
    /// </summary>
    public PhaseService(
        EIA.S0.Domain.Core.Repositories.IRepository<PhaseDefinition> phaseRepository,
        EIA.S0.Domain.Core.Repositories.IRepository<DocType> docTypeRepository,
        EIA.S0.Domain.Core.Repositories.IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        PhaseCache cache,
        Func<string, object, CancellationToken, Task> eventPublishFunc)
        : this(phaseRepository, docTypeRepository, unitOfWork, timeProvider)
    {
        _cache = cache;
        _eventPublishFunc = eventPublishFunc;
    }

    /// <summary>
    /// 发布 PhaseChanged 事件并更新缓存.
    /// </summary>
    private async Task PublishPhaseChangedAsync(
        PhaseDefinition entity,
        string operationType,
        CancellationToken cancellationToken)
    {
        var payload = new PhaseChangedEventPayload
        {
            PhaseId = entity.Id,
            PhaseCode = entity.PhaseCode,
            DisplayName = entity.DisplayName,
            IsActive = true,
            OperationType = operationType
        };

        var envelope = new GovernanceEventEnvelope<PhaseChangedEventPayload>
        {
            EventId = Guid.NewGuid().ToString("N"),
            EventType = PhaseChangedTopic,
            Source = "governance-bc",
            OccurredAt = _timeProvider.GetUtcNow().UtcDateTime,
            Version = 1,
            Payload = payload
        };

        await _eventPublishFunc(PhaseChangedTopic, envelope, cancellationToken);

        if (operationType == "DELETED")
        {
            _cache.Remove(entity);
        }
        else
        {
            _cache.Set(entity);
        }

        var cacheEnvelope = new GovernanceEventEnvelope<object>
        {
            EventId = Guid.NewGuid().ToString("N"),
            EventType = CacheRefreshTopic,
            Source = "governance-bc",
            OccurredAt = _timeProvider.GetUtcNow().UtcDateTime,
            Version = 1,
            Payload = new { reason = "PhaseChanged", originInstance = Environment.MachineName }
        };

        await _eventPublishFunc(CacheRefreshTopic, cacheEnvelope, cancellationToken);
    }
}