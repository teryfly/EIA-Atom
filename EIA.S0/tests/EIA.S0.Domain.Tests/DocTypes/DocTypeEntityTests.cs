using System.Text.Json;
using EIA.S0.Domain.Governance.Entities;

namespace EIA.S0.Domain.Tests.DocTypes;

public class DocTypeEntityTests
{
    [Fact]
    public void CanCreateAndSerializeDocType()
    {
        var now = DateTime.UtcNow;
        var id = Guid.NewGuid();
        var allowed = new[] { "P1", "P2" };

        var docType = new DocType(
            id,
            "CODE",
            "Name",
            "Desc",
            allowed,
            "P1",
            null,
            null,
            "{\"k\":\"v\"}",
            "{\"f\":\"v\"}",
            now,
            now);

        Assert.Equal(id.ToString("D"), docType.Id);
        Assert.Equal("CODE", docType.Code);
        Assert.Equal("Name", docType.Name);
        Assert.Equal("P1", docType.DefaultPhaseCode);
        Assert.Equal(2, docType.AllowedPhaseCodes.Count);

        var json = JsonSerializer.Serialize(new
        {
            id = docType.Id,
            code = docType.Code,
            name = docType.Name,
            description = docType.Description
        });

        Assert.Contains("\"code\":\"CODE\"", json);
    }
}