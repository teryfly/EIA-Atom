using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace EIA.S0.Infrastructure.HttpClientHandlers;

/// <summary>
/// token 获取器.
/// </summary>
public class InternalClientTokenExecutor
{
    private DateTime _expire;
    private readonly InternalClientTokenSettings _settings;
    private readonly IHttpClientFactory _factory;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<InternalClientTokenExecutor> _logger;

    /// <summary>
    /// handler.
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="factory"></param>
    /// <param name="timeProvider"></param>
    /// <param name="logger"></param>
    public InternalClientTokenExecutor(
        IOptions<InternalClientTokenSettings> settings
        , IHttpClientFactory factory
        , TimeProvider timeProvider
        , ILogger<InternalClientTokenExecutor> logger)
    {
        _settings = settings.Value;
        _factory = factory;
        _timeProvider = timeProvider;
        _logger = logger;
        _expire = _timeProvider.GetLocalNow().LocalDateTime.AddSeconds(-1);

        StartWoker();
    }

    /// <summary>
    /// token.
    /// </summary>
    public string? Token { get; set; }

    private void StartWoker()
    {
        if (!_settings.Enable || string.IsNullOrWhiteSpace(_settings.Authority))
        {
            _logger.LogInformation("未启用 Token Handler.");
            return;
        }

        Task.Run(async () =>
        {
            while (true)
            {
                SpinWait.SpinUntil(() => _timeProvider.GetLocalNow().LocalDateTime >= _expire, -1);

                await GetTokenAsync();
            }
        });
    }

    private async Task GetTokenAsync()
    {
        // 构建 client
        var client = _factory.CreateClient();
        client.BaseAddress = new Uri(_settings.Authority!);

        // 构建表单
        var list = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("client_id", _settings.ClientId),
            new KeyValuePair<string, string>("client_secret", _settings.ClientSecret)
        };
        if (!string.IsNullOrWhiteSpace(_settings.UserName) && !string.IsNullOrWhiteSpace(_settings.Password))
        {
            list.Add(new KeyValuePair<string, string>("grant_type", "password"));
            list.Add(new KeyValuePair<string, string>("username", _settings.UserName));
            list.Add(new KeyValuePair<string, string>("password", _settings.Password));
        }
        else
        {
            list.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
        }

        try
        {
            var form = new FormUrlEncodedContent(list);
            var result = await client.PostAsync("connect/token", form);
            if (result.IsSuccessStatusCode)
            {
                var tokenResponse = (await result.Content.ReadFromJsonAsync<TokenResponse>())!;
                Token = tokenResponse.GetToken();
                _expire = _timeProvider.GetLocalNow().LocalDateTime.AddSeconds(tokenResponse.ExpiresIn * 3 / 4);
            }
            else
            {
                // 如果获取失败,30秒后再试.
                _expire = _timeProvider.GetLocalNow().LocalDateTime.AddSeconds(30);
                _logger.LogError($"获取Token失败:{(int)result.StatusCode}-{await result.Content.ReadAsStringAsync()}");
            }
        }
        catch (Exception ex)
        {
            // 如果获取失败,30秒后再试.
            _expire = _timeProvider.GetLocalNow().LocalDateTime.AddSeconds(30);
            _logger.LogError(ex, $"获取Token失败.");
        }
    }

    /// <summary>
    /// token response.
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// token.
        /// </summary>
        /// <value></value>
        [System.Text.Json.Serialization.JsonPropertyName("access_token")]
        public required string AccessToken { get; set; }

        /// <summary>
        /// expire.
        /// </summary>
        /// <value></value>
        [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
        public required int ExpiresIn { get; set; }

        /// <summary>
        /// token type.
        /// </summary>
        /// <value></value>
        [System.Text.Json.Serialization.JsonPropertyName("token_type")]
        public required string TokenType { get; set; }

        /// <summary>
        /// scope.
        /// </summary>
        /// <value></value>

        [System.Text.Json.Serialization.JsonPropertyName("scope")]
        public required string Scope { get; set; }

        /// <summary>
        /// 获取令牌.
        /// </summary>
        /// <returns></returns>
        public string GetToken()
        {
            return $"{this.TokenType} {this.AccessToken}";
        }
    }
}
