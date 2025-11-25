namespace EIA.S0.Infrastructure.HttpClientHandlers;

/// <summary>
/// oauth token handler.
/// </summary>
public class OAuthTokenHandler : DelegatingHandler
{
    private readonly InternalClientTokenExecutor _executor;

    /// <summary>
    /// handler.
    /// </summary>
    /// <param name="executor"></param>
    public OAuthTokenHandler(InternalClientTokenExecutor executor)
    {
        _executor = executor;
    }

    private void SetToken(HttpRequestMessage request)
    {
        if (!string.IsNullOrWhiteSpace(_executor.Token) && !request.Headers.Contains(nameof(request.Headers.Authorization)))
        {
            request.Headers.Add(nameof(request.Headers.Authorization), _executor.Token);
        }
    }

    /// <summary>
    /// sendasync.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        SetToken(request);

        return base.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// send.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        SetToken(request);

        return base.Send(request, cancellationToken);
    }
}
