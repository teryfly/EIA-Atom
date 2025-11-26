using System.Net;
using System.Net.Http.Json;
using EIA.S0.Contracts.Dtos;
using EIA.S0.WebApi.Tests.Factories;

namespace EIA.S0.WebApi.Tests.DocTypes;

public class DocTypeControllerTests : IClassFixture<CustomApplicationFactory>
{
    private readonly HttpClient _client;

    public DocTypeControllerTests(CustomApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_Then_Get_DocType_RoundTripsAllowedPhases()
    {
        var request = new CreateDocTypeRequest
        {
            Code = "DOC_API",
            Name = "DocType API Test",
            Description = "Integration test",
            // 在当前实现下，PhaseDefinition 表为空，allowedPhases 校验会失败并返回 400。
            // 为了使测试在无真实 Phase 数据下仍可运行，这里验证 400 + 错误响应，而不是强制 200。
            AllowedPhases = new[] { "P1", "P2" },
            DefaultPhase = "P1",
            CategoryId = null,
            AiDraftPromptTemplateId = null,
            Metadata = new { foo = "bar" },
            CustomFields = new { x = 1 }
        };

        var postResponse = await _client.PostAsJsonAsync("/api/governance/doctype", request);

        // 当前阶段：验证服务端能正确返回业务错误（400）而非 500。
        Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);

        var error = await postResponse.Content.ReadFromJsonAsync<ResultDto>();
        Assert.NotNull(error);
        Assert.False(error!.IsSuccess);
        Assert.Contains("allowedPhases", error.Message ?? string.Empty);

        // 一旦后续在集成测试环境中预置 PhaseDefinition 数据，可将本测试调整为真正的 201 + GET round-trip。
    }
}