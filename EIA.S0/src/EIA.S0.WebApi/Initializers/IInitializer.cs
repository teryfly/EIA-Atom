namespace EIA.S0.WebApi.Initializers;

/// <summary>
/// 初始化.
/// </summary>
public interface IInitializer
{
    /// <summary>
    /// 初始化.
    /// </summary>
    /// <returns></returns>
    Task InitializeAsync();
}
