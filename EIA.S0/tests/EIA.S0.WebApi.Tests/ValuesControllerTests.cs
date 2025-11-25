using EIA.S0.WebApi.Tests.Factories;

namespace EIA.S0.WebApi.Tests;

public class ValuesControllerTests : IClassFixture<CustomApplicationFactory>
{
    private readonly HttpClient _client;

    public ValuesControllerTests(CustomApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Test_GetAsync()
    {
        var response = await _client.GetAsync("api/values");
        Assert.True(response.IsSuccessStatusCode);
    }
}
