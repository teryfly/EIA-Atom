using EIA.S0.Application.Governance.Cache;
using EIA.S0.Application.Governance.Phases;
using EIA.S0.Domain.Core.Exceptions;
using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Governance.Entities;
using Moq;

namespace EIA.S0.Application.Tests.Phases;

public class PhaseServiceTests
{
    private PhaseService CreateService(
        IEnumerable<PhaseDefinition>? existingPhases = null)
    {
        existingPhases ??= Array.Empty<PhaseDefinition>();

        var phaseRepo = new Mock<IRepository<PhaseDefinition>>();

        phaseRepo.Setup(r => r.GetListAsync(It.IsAny<EIA.S0.Domain.Core.Specifications.Queries.IQuerySpecification<PhaseDefinition>>()))
            .ReturnsAsync(existingPhases);

        phaseRepo.Setup(r => r.AnyAsync(It.IsAny<EIA.S0.Domain.Core.Specifications.Queries.IQuerySpecification<PhaseDefinition>>()))
            .ReturnsAsync((EIA.S0.Domain.Core.Specifications.Queries.IQuerySpecification<PhaseDefinition> spec) =>
            {
                return existingPhases.Any(p => spec.IsSatisfiedBy(p));
            });

        phaseRepo.Setup(r => r.Add(It.IsAny<PhaseDefinition>()))
            .Returns<PhaseDefinition>(p => p);

        phaseRepo.Setup(r => r.Update(It.IsAny<PhaseDefinition>()))
            .Returns<PhaseDefinition>(p => p);

        var docTypeRepo = new Mock<IRepository<DocType>>();

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var cache = new PhaseCache();
        Func<string, object, CancellationToken, Task> publishFunc =
            (topic, payload, token) => Task.CompletedTask;

        return new PhaseService(
            phaseRepo.Object,
            docTypeRepo.Object,
            uow.Object,
            TimeProvider.System,
            cache,
            publishFunc);
    }

    [Fact]
    public async Task CreateAsync_Succeeds_WithUniquePhaseCode_AndValidTransitions()
    {
        // existing phase that allowedTransitions can point to
        var existing = new PhaseDefinition(
            Guid.NewGuid(),
            "REVIEW",
            "Review",
            2,
            Array.Empty<string>(),
            null,
            DateTime.UtcNow,
            DateTime.UtcNow);

        var service = CreateService(new[] { existing });

        var phase = await service.CreateAsync(
            "DRAFT",
            "Draft",
            1,
            new[] { "REVIEW" },
            null,
            CancellationToken.None);

        Assert.NotNull(phase);
        Assert.Equal("DRAFT", phase.PhaseCode);
        Assert.Contains("REVIEW", phase.AllowedTransitionPhaseCodes);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenDuplicatePhaseCode()
    {
        var existing = new PhaseDefinition(
            Guid.NewGuid(),
            "DRAFT",
            "Draft",
            1,
            Array.Empty<string>(),
            null,
            DateTime.UtcNow,
            DateTime.UtcNow);

        var service = CreateService(new[] { existing });

        await Assert.ThrowsAsync<DomainException>(() =>
            service.CreateAsync(
                "DRAFT",
                "Draft2",
                2,
                null,
                null,
                CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenAllowedTransitionsContainUnknownPhase()
    {
        var service = CreateService(); // no existing phases

        await Assert.ThrowsAsync<DomainException>(() =>
            service.CreateAsync(
                "DRAFT",
                "Draft",
                1,
                new[] { "REVIEW" }, // REVIEW not existing
                null,
                CancellationToken.None));
    }
}