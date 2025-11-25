namespace EIA.S0.WebApi.Settings;

/// <summary>
/// oauth.
/// </summary>
public class OAuthSettings
{
    /// <summary>
    /// 服务端地址.
    /// </summary>
    public required string Authority { get; init; }
    
    /// <summary>
    /// 服务资源名称.
    /// </summary>
    public required string Audience { get; init; }

    /// <summary>
    /// 是否验证api.
    /// </summary>
    public bool ValidateAudience { get; init; }
}