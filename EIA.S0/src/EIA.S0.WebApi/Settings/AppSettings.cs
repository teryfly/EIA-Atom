namespace EIA.S0.WebApi.Settings;

/// <summary>
/// 系统配置.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// appId.
    /// </summary>
    public required string AppId { get; init; }

    /// <summary>
    /// app名称.
    /// </summary>
    public required string AppName { get; init; }

    /// <summary>
    /// path base.
    /// </summary>
    public string? PathBase { get; init; }

    /// <summary>
    /// swagger配置.
    /// </summary>
    public SwaggerSettings Swagger { get; init; } = new SwaggerSettings();

    /// <summary>
    /// 数据库配置.
    /// </summary>
    public required DatabaseSettings Database { get; init; }

    /// <summary>
    /// 认证授权配置.
    /// </summary>
    public OAuthSettings? OAuth { get; init; }

    /// <summary>
    /// 用户名claim.
    /// </summary>
    public string UserNameClaim { get; init; } = "workno";

    /// <summary>
    /// 领域事件分发配置.
    /// </summary>
    public DomainEventDispatchSettings DomainEventDispatch { get; init; } = new DomainEventDispatchSettings();
}