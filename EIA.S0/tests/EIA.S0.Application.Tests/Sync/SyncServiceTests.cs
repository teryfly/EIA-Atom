using EIA.S0.Application.Governance.Sync;
using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Core.Specifications.Queries;
using EIA.S0.Domain.Governance.Entities;
using Moq;

namespace EIA.S0.Application.Tests.Sync;

public class SyncServiceTests
{
    [Fact]
    public async Task GetAllDocTypesAsync_ReturnsList()
    {
        var repo = new Mock<IRepository<DocType>>();
        var spec = new Mock<IQuerySpecification<DocType>>();

        var list = new List<DocType>
        {
            new DocType(Guid.NewGuid(), "CODE", "Name", null, new[] { "P1" }, "P1", null, null, null, null,
                DateTime.UtcNow, DateTime.UtcNow)
        };

        repo.Setup(r => r.GetListAsync(It.IsAny<IQuerySpecification<DocType>>()))
            .ReturnsAsync(list);

        var service = new SyncService(repo.Object);
        var result = await service.GetAllDocTypesAsync(spec.Object);

        Assert.Single(result);
        Assert.Equal("CODE", result.First().Code);
    }
}