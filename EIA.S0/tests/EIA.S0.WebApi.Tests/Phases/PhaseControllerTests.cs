using System.Net;
using System.Net.Http.Json;
using EIA.S0.Contracts.Dtos;
using EIA.S0.WebApi.Tests.Factories;

namespace EIA.S0.WebApi.Tests.Phases;

public class PhaseControllerTests : IClassFixture<CustomApplicationFactory>
{
    private readonly HttpClient _client;

    public PhaseControllerTests(CustomApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Phase_E2E_Create_Get_Update_Delete_Flow()
    {
        // Create
        var createRequest = new CreatePhaseRequest
        {
            PhaseCode = "UNIT_TEST_PHASE",
            DisplayName = "Unit Test Phase",
            Order = 10,
            AllowedTransitions = Array.Empty<string>(),
            Properties = new { level = "test" }
        };

        var createResponse = await _client.PostAsJsonAsync("api/governance/phase", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createBody = await createResponse.Content.ReadFromJsonAsync<CreatePhaseResponse>();
        Assert.NotNull(createBody);
        Assert.False(string.IsNullOrWhiteSpace(createBody!.Id));

        var id = createBody.Id;

        // Get
        var getResponse = await _client.GetAsync($"api/governance/phase/{id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var phase = await getResponse.Content.ReadFromJsonAsync<PhaseDto>();
        Assert.NotNull(phase);
        Assert.Equal("UNIT_TEST_PHASE", phase!.PhaseCode);

        // Update
        var updateRequest = new UpdatePhaseRequest
        {
            DisplayName = "Unit Test Phase Updated",
            Order = 20,
            AllowedTransitions = Array.Empty<string>(),
            Properties = new { level = "test-updated" }
        };

        var updateResponse = await _client.PutAsJsonAsync($"api/governance/phase/{id}", updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updateBody = await updateResponse.Content.ReadFromJsonAsync<UpdatePhaseResponse>();
        Assert.NotNull(updateBody);
        Assert.True(updateBody!.Updated);

        // Get again
        var getResponse2 = await _client.GetAsync($"api/governance/phase/{id}");
        Assert.Equal(HttpStatusCode.OK, getResponse2.StatusCode);
        var phase2 = await getResponse2.Content.ReadFromJsonAsync<PhaseDto>();
        Assert.NotNull(phase2);
        Assert.Equal("Unit Test Phase Updated", phase2!.DisplayName);

        // Delete
        var deleteResponse = await _client.DeleteAsync($"api/governance/phase/{id}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        var deleteBody = await deleteResponse.Content.ReadFromJsonAsync<DeletePhaseResponse>();
        Assert.NotNull(deleteBody);
        Assert.True(deleteBody!.Deleted);

        // Get after delete
        var getResponse3 = await _client.GetAsync($"api/governance/phase/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse3.StatusCode);
    }
}