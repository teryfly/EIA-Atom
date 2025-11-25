using System.Collections.Concurrent;
using EIA.S0.Domain.Governance.Entities;

namespace EIA.S0.Application.Governance.Cache;

/// <summary>
/// DocType 本地缓存.
/// </summary>
public class DocTypeCache
{
    private readonly ConcurrentDictionary<string, DocType> _byId = new();
    private readonly ConcurrentDictionary<string, DocType> _byCode = new();

    /// <summary>
    /// 根据 Id 获取 DocType.
    /// </summary>
    public DocType? GetById(string id)
    {
        return _byId.TryGetValue(id, out var value) ? value : null;
    }

    /// <summary>
    /// 根据编码获取 DocType.
    /// </summary>
    public DocType? GetByCode(string code)
    {
        return _byCode.TryGetValue(code, out var value) ? value : null;
    }

    /// <summary>
    /// 设置或更新缓存.
    /// </summary>
    public void Set(DocType docType)
    {
        _byId[docType.Id] = docType;
        _byCode[docType.Code] = docType;
    }

    /// <summary>
    /// 从缓存移除.
    /// </summary>
    public void Remove(DocType docType)
    {
        _byId.TryRemove(docType.Id, out _);
        _byCode.TryRemove(docType.Code, out _);
    }

    /// <summary>
    /// 清空缓存.
    /// </summary>
    public void Clear()
    {
        _byId.Clear();
        _byCode.Clear();
    }
}