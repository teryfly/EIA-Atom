namespace EIA.S0.Infrastructure.HttpClientHandlers;

/// <summary>
/// 配置.
/// </summary>
public class InternalClientTokenSettings
{
    /// <summary>
    /// 是否启用.
    /// </summary>
    /// <value></value>
    public bool Enable { get; set; }

    /// <summary>
    /// oauth server uri.
    /// </summary>
    /// <value></value>
    public string? Authority { get; set; }

    /// <summary>
    /// client id.
    /// </summary>
    public string ClientId { get; set; } = "internal-client";

    /// <summary>
    /// client secret.
    /// </summary>
    public string ClientSecret { get; set; } = "internal-client-secret";

    /// <summary>
    /// 用户名.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 密码.
    /// </summary>
    public string? Password { get; set; }
}