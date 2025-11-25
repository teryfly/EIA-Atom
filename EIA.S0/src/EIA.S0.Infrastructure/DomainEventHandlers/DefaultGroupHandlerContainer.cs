using EIA.S0.Domain.Core.DomainEvents;

namespace EIA.S0.Infrastructure.DomainEventHandlers;

/// <summary>
/// container.
/// </summary>
internal class DefaultGroupHandlerContainer : IGroupHandlerContainer
{
    private Dictionary<string, object?> contaienr = new Dictionary<string, object?>();

    /// <summary>
    /// 获取.
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T? Get<T>(string key)
    {
        if (contaienr.ContainsKey(key))
        {
            var v = contaienr[key];
            if (v == null)
            {
                return default(T);
            }

            return (T)v;
        }

        return default(T);
    }

    /// <summary>
    /// 写入.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="obj"></param>
    public void Set<T>(string key, T obj)
    {
        if (contaienr.ContainsKey(key))
        {
            contaienr[key] = obj;
        }
        else
        {
            contaienr.Add(key, obj);
        }
    }
}