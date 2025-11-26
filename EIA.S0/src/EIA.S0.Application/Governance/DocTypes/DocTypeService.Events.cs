using EIA.S0.Application.Governance.Cache;
using EIA.S0.Application.Governance.Events;
using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Governance.Entities;

namespace EIA.S0.Application.Governance.DocTypes;

/// <summary>
/// DocTypeService 事件与缓存集成部分.
/// </summary>
public partial class DocTypeService
{
    private const string DocTypeChangedTopic = "governance.doctype.changed.v1";
    private const string CacheRefreshTopic = "governance.cache.refresh.v1";

    private readonly Func<string, object, CancellationToken, Task> _eventPublishFunc = default!;
    private readonly DocTypeCache _cache = default!;

    public DocTypeService(
        IRepository<DocType> docTypeRepository,
        IRepository<PhaseDefinition> phaseRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        DocTypeCache cache,
        Func<string, object, CancellationToken, Task> eventPublishFunc)
        : this(docTypeRepository, phaseRepository, unitOfWork, timeProvider)
    {
        _cache = cache;
        _eventPublishFunc = eventPublishFunc;
    }

    private async Task PublishDocTypeChangedAsync(DocType entity, string operationType, CancellationToken cancellationToken)
    {
        var payload = new DocTypeChangedEventPayload
        {
            DocTypeId = entity.Id,
            DocTypeCode = entity.Code,
            Name = entity.Name,
            Status = "ENABLED",
            OperationType = operationType
        };

        var envelope = new GovernanceEventEnvelope<DocTypeChangedEventPayload>
        {
            EventId = Guid.NewGuid().ToString("N"),
            EventType = DocTypeChangedTopic,
            Source = "governance-bc",
            OccurredAt = _timeProvider.GetUtcNow().UtcDateTime,
            Version = 1,
            Payload = payload
        };

        await _eventPublishFunc(DocTypeChangedTopic, envelope, cancellationToken);

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
            Payload = new { reason = "DocTypeChanged", originInstance = Environment.MachineName }
        };

        await _eventPublishFunc(CacheRefreshTopic, cacheEnvelope, cancellationToken);
    }
}