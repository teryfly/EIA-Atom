using System.Collections.Concurrent;
using EIA.S0.Domain.Governance.Entities;

namespace EIA.S0.Application.Governance.Cache;

/// <summary>
/// PhaseDefinition 本地缓存.
/// </summary>
public class PhaseCache
{
    private readonly ConcurrentDictionary<string, PhaseDefinition> _byId = new();
    private readonly ConcurrentDictionary<string, PhaseDefinition> _byCode = new();

    /// <summary>
    /// 根据 Id 获取 PhaseDefinition.
    /// </summary>
    public PhaseDefinition? GetById(string id)
    {
        return _byId.TryGetValue(id, out var value) ? value : null;
    }

    /// <summary>
    /// 根据阶段编码获取 PhaseDefinition.
    /// </summary>
    public PhaseDefinition? GetByCode(string phaseCode)
    {
        return _byCode.TryGetValue(phaseCode, out var value) ? value : null;
    }

    /// <summary>
    /// 设置或更新缓存.
    /// </summary>
    public void Set(PhaseDefinition phase)
    {
        _byId[phase.Id] = phase;
        _byCode[phase.PhaseCode] = phase;
    }

    /// <summary>
    /// 从缓存移除.
    /// </summary>
    public void Remove(PhaseDefinition phase)
    {
        _byId.TryRemove(phase.Id, out _);
        _byCode.TryRemove(phase.PhaseCode, out _);
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