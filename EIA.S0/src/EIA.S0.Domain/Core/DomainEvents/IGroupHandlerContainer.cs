namespace EIA.S0.Domain.Core.DomainEvents;

/// <summary>
/// 同组容器.
/// </summary>
public interface IGroupHandlerContainer
{
    /// <summary>
    /// 写入.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="obj"></param>
    void Set<T>(string key, T obj);

    /// <summary>
    /// 获取.
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T? Get<T>(string key);
}
