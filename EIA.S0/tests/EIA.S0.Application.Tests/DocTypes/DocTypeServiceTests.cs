using EIA.S0.Application.Governance.Cache;
using EIA.S0.Application.Governance.DocTypes;
using EIA.S0.Domain.Core.Exceptions;
using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Core.Specifications.Queries;
using EIA.S0.Domain.Governance.Entities;
using Moq;

namespace EIA.S0.Application.Tests.DocTypes;

public class DocTypeServiceTests
{
    private DocTypeService CreateService(
        DocType? existingDocType = null,
        IEnumerable<PhaseDefinition>? existingPhases = null)
    {
        existingPhases ??= Array.Empty<PhaseDefinition>();

        var docTypeRepo = new Mock<IRepository<DocType>>();
        docTypeRepo.Setup(r => r.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(existingDocType);

        docTypeRepo.Setup(r => r.Add(It.IsAny<DocType>()))
            .Returns<DocType>(d => d);

        docTypeRepo.Setup(r => r.Update(It.IsAny<DocType>()))
            .Returns<DocType>(d => d);

        var phaseRepo = new Mock<IRepository<PhaseDefinition>>();
        phaseRepo.Setup(r => r.AnyAsync(It.IsAny<IQuerySpecification<PhaseDefinition>>()))
            .ReturnsAsync((IQuerySpecification<PhaseDefinition> spec) =>
            {
                return existingPhases.Any(p => spec.IsSatisfiedBy(p));
            });

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var timeProvider = TimeProvider.System;
        var cache = new DocTypeCache();

        Func<string, object, CancellationToken, Task> publishFunc =
            (topic, payload, token) => Task.CompletedTask;

        return new DocTypeService(
            docTypeRepo.Object,
            phaseRepo.Object,
            uow.Object,
            timeProvider,
            cache,
            publishFunc);
    }

    [Fact]
    public async Task CreateDocType_UpdatesCache()
    {
        var service = CreateService(
            existingDocType: null,
            existingPhases: new[]
            {
                new PhaseDefinition(Guid.NewGuid(), "P1", "Phase 1", 1, Array.Empty<string>(), null,
                    DateTime.UtcNow, DateTime.UtcNow)
            });

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
        var service = CreateService(
            existingDocType: null,
            existingPhases: new[]
            {
                new PhaseDefinition(Guid.NewGuid(), "P1", "Phase 1", 1, Array.Empty<string>(), null,
                    DateTime.UtcNow, DateTime.UtcNow),
                new PhaseDefinition(Guid.NewGuid(), "P2", "Phase 2", 2, Array.Empty<string>(), null,
                    DateTime.UtcNow, DateTime.UtcNow)
            });

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

        var service = CreateService(
            existingDocType: existing,
            existingPhases: new[]
            {
                new PhaseDefinition(Guid.NewGuid(), "P1", "Phase 1", 1, Array.Empty<string>(), null,
                    DateTime.UtcNow, DateTime.UtcNow),
                new PhaseDefinition(Guid.NewGuid(), "P2", "Phase 2", 2, Array.Empty<string>(), null,
                    DateTime.UtcNow, DateTime.UtcNow)
            });

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

    [Fact]
    public async Task CreateDocType_Throws_WhenAllowedPhaseDoesNotExist()
    {
        // No existing phases; allowedPhases contains P1 which should be rejected.
        var service = CreateService(
            existingDocType: null,
            existingPhases: Array.Empty<PhaseDefinition>());

        var ex = await Assert.ThrowsAsync<DomainException>(async () =>
        {
            await service.CreateDocTypeAsync(
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
        });

        Assert.Contains("allowedPhases 中的阶段编码[P1]不存在", ex.Message);
    }

    [Fact]
    public async Task CreateDocType_Succeeds_WhenAllAllowedPhasesExist()
    {
        var phases = new[]
        {
            new PhaseDefinition(Guid.NewGuid(), "P1", "Phase 1", 1, Array.Empty<string>(), null,
                DateTime.UtcNow, DateTime.UtcNow),
            new PhaseDefinition(Guid.NewGuid(), "P2", "Phase 2", 2, Array.Empty<string>(), null,
                DateTime.UtcNow, DateTime.UtcNow)
        };

        var service = CreateService(
            existingDocType: null,
            existingPhases: phases);

        var entity = await service.CreateDocTypeAsync(
            "CODE",
            "Name",
            "Desc",
            new[] { "P1", "P2" },
            "P1",
            null,
            null,
            null,
            null,
            CancellationToken.None);

        Assert.NotNull(entity);
        Assert.Equal("CODE", entity.Code);
        Assert.Equal(2, entity.AllowedPhaseCodes.Count);
    }
}