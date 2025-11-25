using EIA.S0.Application.Governance.Cache;
using EIA.S0.Application.Governance.DocTypes;
using EIA.S0.Domain.Core.Exceptions;
using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Governance.Entities;
using Moq;

namespace EIA.S0.Application.Tests.DocTypes;

public class DocTypeServiceTests
{
    private DocTypeService CreateService(DocType? existing = null)
    {
        var repo = new Mock<IRepository<DocType>>();
        repo.Setup(r => r.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(existing);

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var timeProvider = TimeProvider.System;
        var cache = new DocTypeCache();

        Func<string, object, CancellationToken, Task> publishFunc =
            (topic, payload, token) => Task.CompletedTask;

        return new DocTypeService(repo.Object, uow.Object, timeProvider, cache, publishFunc);
    }

    [Fact]
    public async Task CreateDocType_UpdatesCache()
    {
        var service = CreateService();

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

        Assert.NotNull(entity);
        Assert.Equal("CODE", entity.Code);
    }

    [Fact]
    public async Task CreateDocType_Throws_WhenDefaultPhaseNotInAllowed()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<DomainException>(async () =>
        {
            await service.CreateDocTypeAsync(
                "CODE",
                "Name",
                "Desc",
                new[] { "P1", "P2" },
                "P3",
                null,
                null,
                null,
                null,
                CancellationToken.None);
        });
    }

    [Fact]
    public async Task UpdateDocType_Throws_WhenDefaultPhaseNotInAllowed()
    {
        var existing = new DocType(
            Guid.NewGuid(),
            "CODE",
            "Name",
            null,
            new[] { "P1" },
            "P1",
            null,
            null,
            null,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow);

        var service = CreateService(existing);

        await Assert.ThrowsAsync<DomainException>(async () =>
        {
            await service.UpdateDocTypeAsync(
                existing.Id,
                "Name2",
                null,
                new[] { "P1" },
                "P2",
                null,
                null,
                null,
                null,
                CancellationToken.None);
        });
    }
}