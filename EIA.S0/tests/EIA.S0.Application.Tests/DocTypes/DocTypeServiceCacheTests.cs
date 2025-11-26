using EIA.S0.Application.Governance.Cache;
using EIA.S0.Application.Governance.DocTypes;
using EIA.S0.Application.Governance.Events;
using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Governance.Entities;
using Moq;

namespace EIA.S0.Application.Tests.DocTypes;

public class DocTypeServiceCacheTests
{
    [Fact]
    public async Task CreateDocType_UpdatesCache_AndPublishesCacheRefresh()
    {
        var docTypeRepo = new Mock<IRepository<DocType>>();
        docTypeRepo.Setup(r => r.Add(It.IsAny<DocType>()))
            .Returns<DocType>(d => d);

        var phaseRepo = new Mock<IRepository<PhaseDefinition>>();
        phaseRepo.Setup(r => r.AnyAsync(It.IsAny<EIA.S0.Domain.Core.Specifications.Queries.IQuerySpecification<PhaseDefinition>>()))
            .ReturnsAsync(true);

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var cache = new DocTypeCache();
        var timeProvider = TimeProvider.System;

        var published = new List<(string Topic, object Payload)>();
        Func<string, object, CancellationToken, Task> publishFunc =
            (topic, payload, token) =>
            {
                published.Add((topic, payload));
                return Task.CompletedTask;
            };

        var service = new DocTypeService(docTypeRepo.Object, phaseRepo.Object, uow.Object, timeProvider, cache, publishFunc);

        var entity = await service.CreateDocTypeAsync(
            "CODE",
            "Name",
            "Desc",
            new[] { "P1" },
            "P1",
            null,
            null,
            null,
            null,
            CancellationToken.None);

        // Cache should contain the new entity.
        var fromCacheById = cache.GetById(entity.Id);
        Assert.NotNull(fromCacheById);
        Assert.Equal("CODE", fromCacheById!.Code);

        var fromCacheByCode = cache.GetByCode("CODE");
        Assert.NotNull(fromCacheByCode);

        // Cache refresh event should be published.
        var cacheEvt = published.FirstOrDefault(p => p.Topic == "governance.cache.refresh.v1");
        var envelope = Assert.IsType<GovernanceEventEnvelope<object>>(cacheEvt.Payload);
        Assert.Equal("governance.cache.refresh.v1", envelope.EventType);
    }
}