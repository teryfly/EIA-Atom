using System.Text.Json;
using EIA.S0.Domain.Governance.Entities;

namespace EIA.S0.Domain.Tests.Phases;

public class PhaseDefinitionEntityTests
{
    [Fact]
    public void CanCreateAndSerializePhaseDefinition()
    {
        var now = DateTime.UtcNow;
        var id = Guid.NewGuid();
        var allowed = new[] { "DRAFT", "REVIEW" };
        var propertiesJson = "{\"key\":\"value\"}";

        var phase = new PhaseDefinition(
            id,
            "DRAFT",
            "Draft",
            1,
            allowed,
            propertiesJson,
            now,
            now);

        Assert.Equal(id.ToString("D"), phase.Id);
        Assert.Equal("DRAFT", phase.PhaseCode);
        Assert.Equal("Draft", phase.DisplayName);
        Assert.Equal(1, phase.Order);
        Assert.Equal(2, phase.AllowedTransitionPhaseCodes.Count);
        Assert.Equal(propertiesJson, phase.PropertiesJson);

        var json = JsonSerializer.Serialize(new
        {
            id = phase.Id,
            phaseCode = phase.PhaseCode,
            displayName = phase.DisplayName
        });

        Assert.Contains("\"phaseCode\":\"DRAFT\"", json);
    }
}