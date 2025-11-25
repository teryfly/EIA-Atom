using EIA.S0.Domain.Core.Aggregates;

namespace EIA.S0.Domain.Governance.Entities;

/// <summary>
/// DocType 聚合根.
/// </summary>
public class DocType : AggregateRoot
{
    /// <summary>
    /// 文档类型编码.
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// 名称.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 描述.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// 允许的阶段编码列表.
    /// </summary>
    public List<string> AllowedPhaseCodes { get; private set; } = new();

    /// <summary>
    /// 默认阶段编码.
    /// </summary>
    public string DefaultPhaseCode { get; private set; }

    /// <summary>
    /// 分类 Id.
    /// </summary>
    public Guid? CategoryId { get; private set; }

    /// <summary>
    /// 默认 AI 草稿 PromptTemplate Id.
    /// </summary>
    public Guid? AiDraftPromptTemplateId { get; private set; }

    /// <summary>
    /// 元数据.
    /// </summary>
    public string? MetadataJson { get; private set; }

    /// <summary>
    /// 自定义字段.
    /// </summary>
    public string? CustomFieldsJson { get; private set; }

    /// <summary>
    /// 创建时间 (UTC).
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 最后更新时间 (UTC).
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// EF Core 使用的空构造函数.
    /// </summary>
#pragma warning disable CS8618
    private DocType()
    {
    }
#pragma warning restore CS8618

    /// <summary>
    /// 构造一个新的 DocType.
    /// </summary>
    public DocType(
        Guid id,
        string code,
        string name,
        string? description,
        IEnumerable<string> allowedPhaseCodes,
        string defaultPhaseCode,
        Guid? categoryId,
        Guid? aiDraftPromptTemplateId,
        string? metadataJson,
        string? customFieldsJson,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        Id = id.ToString("D");
        Code = code;
        Name = name;
        Description = description;
        AllowedPhaseCodes = allowedPhaseCodes?.Distinct().ToList() ?? new List<string>();
        DefaultPhaseCode = defaultPhaseCode;
        CategoryId = categoryId;
        AiDraftPromptTemplateId = aiDraftPromptTemplateId;
        MetadataJson = metadataJson;
        CustomFieldsJson = customFieldsJson;
        CreatedAt = DateTime.SpecifyKind(createdAtUtc, DateTimeKind.Utc);
        UpdatedAt = DateTime.SpecifyKind(updatedAtUtc, DateTimeKind.Utc);
    }

    /// <summary>
    /// 更新基础信息.
    /// </summary>
    public void UpdateBasicInfo(
        string name,
        string? description,
        IEnumerable<string> allowedPhaseCodes,
        string defaultPhaseCode,
        Guid? categoryId,
        Guid? aiDraftPromptTemplateId,
        string? metadataJson,
        string? customFieldsJson,
        DateTime updatedAtUtc)
    {
        Name = name;
        Description = description;
        AllowedPhaseCodes = allowedPhaseCodes?.Distinct().ToList() ?? new List<string>();
        DefaultPhaseCode = defaultPhaseCode;
        CategoryId = categoryId;
        AiDraftPromptTemplateId = aiDraftPromptTemplateId;
        MetadataJson = metadataJson;
        CustomFieldsJson = customFieldsJson;
        UpdatedAt = DateTime.SpecifyKind(updatedAtUtc, DateTimeKind.Utc);
    }
}