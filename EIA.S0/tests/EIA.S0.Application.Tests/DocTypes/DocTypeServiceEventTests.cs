using EIA.S0.Application.Governance.Cache;
using EIA.S0.Application.Governance.DocTypes;
using EIA.S0.Application.Governance.Events;
using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Governance.Entities;
using Moq;

namespace EIA.S0.Application.Tests.DocTypes;

public class DocTypeServiceEventTests
{
    private (DocTypeService Service, List<(string Topic, object Payload)> Published) CreateServiceWithPublisher()
    {
        var docTypeRepo = new Mock<IRepository<DocType>>();
        docTypeRepo.Setup(r => r.Add(It.IsAny<DocType>()))
            .Returns<DocType>(d => d);
        docTypeRepo.Setup(r => r.Update(It.IsAny<DocType>()))
            .Returns<DocType>(d => d);
        docTypeRepo.Setup(r => r.Delete(It.IsAny<DocType>()));
        docTypeRepo.Setup(r => r.GetAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) =>
                new DocType(Guid.Parse(id), "CODE", "Name", null, new[] { "P1" }, "P1",
                    null, null, null, null, DateTime.UtcNow, DateTime.UtcNow));

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
        return (service, published);
    }

    [Fact]
    public async Task CreateDocType_PublishesDocTypeChangedEvent()
    {
        var (service, published) = CreateServiceWithPublisher();

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

        var evt = published.FirstOrDefault(p => p.Topic == "governance.doctype.changed.v1");
        Assert.NotNull(evt.Payload);

        var envelope = Assert.IsType<GovernanceEventEnvelope<DocTypeChangedEventPayload>>(evt.Payload);
        Assert.Equal("governance.doctype.changed.v1", envelope.EventType);
        Assert.Equal(entity.Id, envelope.Payload.DocTypeId);
        Assert.Equal(entity.Code, envelope.Payload.DocTypeCode);
        Assert.Equal("CREATED", envelope.Payload.OperationType);
    }

    [Fact]
    public async Task DeleteDocType_PublishesDocTypeChangedEvent_WithDeletedOperationType()
    {
        var (service, published) = CreateServiceWithPublisher();

        var id = Guid.NewGuid().ToString("D");
        var deleted = await service.DeleteDocTypeAsync(id, CancellationToken.None);

        Assert.True(deleted);

        var evt = published.FirstOrDefault(p => p.Topic == "governance.doctype.changed.v1");
        var envelope = Assert.IsType<GovernanceEventEnvelope<DocTypeChangedEventPayload>>(evt.Payload);
        Assert.Equal("DELETED", envelope.Payload.OperationType);
    }
}