namespace EIA.S0.WebApi.Settings;

/// <summary>
/// swagger配置.
/// </summary>
public class SwaggerSettings
{
    /// <summary>
    /// 是否启用.
    /// </summary>
    public bool Enable { get; init; }

    /// <summary>
    /// 是否启用授权.
    /// </summary>
    public bool Authorization { get; init; } = true;
}