using Microsoft.EntityFrameworkCore;
using EIA.S0.Domain.Governance.Entities;
using EIA.S0.Infrastructure.EntityFrameworkCore;
using EIA.S0.Domain.Core.DomainEvents;
using Microsoft.Extensions.Logging;
using EIA.S0.Infrastructure.Governance.Repositories;
using Moq;

namespace EIA.S0.Infrastructure.Tests.DocTypes;

public class DocTypeRepositoryTests
{
    private EiaS0dbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<EiaS0dbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        var dispatcher = new Mock<IDomainEventDispatcher>().Object;
        var logger = new Mock<ILogger<EiaS0dbContext>>().Object;

        return new EiaS0dbContext(options, dispatcher, logger);
    }

    [Fact]
    public async Task DocTypeRepository_PersistsAllowedPhases_AsList()
    {
        using var context = CreateInMemoryContext();
        var repo = new DocTypeRepository(context);

        var now = DateTime.UtcNow;
        var id = Guid.NewGuid();
        var allowed = new[] { "P1", "P2" };

        var entity = new DocType(
            id,
            "CODE",
            "Name",
            "Desc",
            allowed,
            "P1",
            null,
            null,
            null,
            null,
            now,
            now);

        repo.Add(entity);
        await context.SaveChangesAsync();

        var loaded = await repo.GetAsync(entity.Id);
        Assert.NotNull(loaded);
        Assert.Equal(2, loaded!.AllowedPhaseCodes.Count);
        Assert.Contains("P1", loaded.AllowedPhaseCodes);
        Assert.Contains("P2", loaded.AllowedPhaseCodes);
    }
}