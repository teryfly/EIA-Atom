using EIA.S0.Domain.Core.Aggregates;

namespace EIA.S0.Domain.Governance.Entities;

/// <summary>
/// PhaseDefinition 聚合根.
/// </summary>
public class PhaseDefinition : AggregateRoot
{
    /// <summary>
    /// 阶段编码（全局唯一）.
    /// </summary>
    public string PhaseCode { get; private set; }

    /// <summary>
    /// 显示名称.
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    /// 顺序号.
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    /// 允许跳转到的阶段编码集合.
    /// </summary>
    public List<string> AllowedTransitionPhaseCodes { get; private set; } = new();

    /// <summary>
    /// 扩展属性（JSON 文本）.
    /// </summary>
    public string? PropertiesJson { get; private set; }

    /// <summary>
    /// 创建时间 (UTC).
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 最后更新时间 (UTC).
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

#pragma warning disable CS8618
    /// <summary>
    /// EF Core 使用的空构造函数.
    /// </summary>
    private PhaseDefinition()
    {
    }
#pragma warning restore CS8618

    /// <summary>
    /// 构造函数.
    /// </summary>
    public PhaseDefinition(
        Guid id,
        string phaseCode,
        string displayName,
        int order,
        IEnumerable<string>? allowedTransitionPhaseCodes,
        string? propertiesJson,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        Id = id.ToString("D");
        PhaseCode = phaseCode;
        DisplayName = displayName;
        Order = order;
        AllowedTransitionPhaseCodes = allowedTransitionPhaseCodes?.Distinct().ToList() ?? new List<string>();
        PropertiesJson = propertiesJson;
        CreatedAt = DateTime.SpecifyKind(createdAtUtc, DateTimeKind.Utc);
        UpdatedAt = DateTime.SpecifyKind(updatedAtUtc, DateTimeKind.Utc);
    }

    /// <summary>
    /// 更新基础信息.
    /// </summary>
    public void UpdateBasicInfo(
        string displayName,
        int order,
        IEnumerable<string>? allowedTransitionPhaseCodes,
        string? propertiesJson,
        DateTime updatedAtUtc)
    {
        DisplayName = displayName;
        Order = order;
        AllowedTransitionPhaseCodes = allowedTransitionPhaseCodes?.Distinct().ToList() ?? new List<string>();
        PropertiesJson = propertiesJson;
        UpdatedAt = DateTime.SpecifyKind(updatedAtUtc, DateTimeKind.Utc);
    }
}