namespace EIA.S0.Contracts.Dtos;

/// <summary>
/// DocType DTO.
/// </summary>
public class DocTypeDto
{
    public string Id { get; init; } = string.Empty;

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public IReadOnlyCollection<string> AllowedPhases { get; init; } = Array.Empty<string>();

    public string DefaultPhase { get; init; } = string.Empty;

    public Guid? CategoryId { get; init; }

    public Guid? AiDraftPromptTemplateId { get; init; }

    public object? Metadata { get; init; }

    public object? CustomFields { get; init; }
}

/// <summary>
/// 创建 DocType 请求.
/// </summary>
public class CreateDocTypeRequest
{
    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public IReadOnlyCollection<string> AllowedPhases { get; init; } = Array.Empty<string>();

    public string DefaultPhase { get; init; } = string.Empty;

    public Guid? CategoryId { get; init; }

    public Guid? AiDraftPromptTemplateId { get; init; }

    public object? Metadata { get; init; }

    public object? CustomFields { get; init; }
}

/// <summary>
/// 创建 DocType 响应.
/// </summary>
public class CreateDocTypeResponse
{
    public string Id { get; init; } = string.Empty;
}

/// <summary>
/// 更新 DocType 请求.
/// </summary>
public class UpdateDocTypeRequest
{
    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public IReadOnlyCollection<string> AllowedPhases { get; init; } = Array.Empty<string>();

    public string DefaultPhase { get; init; } = string.Empty;

    public Guid? CategoryId { get; init; }

    public Guid? AiDraftPromptTemplateId { get; init; }

    public object? Metadata { get; init; }

    public object? CustomFields { get; init; }
}

/// <summary>
/// 更新 DocType 响应.
/// </summary>
public class UpdateDocTypeResponse
{
    public bool Updated { get; init; }
}

/// <summary>
/// 删除 DocType 响应.
/// </summary>
public class DeleteDocTypeResponse
{
    public bool Deleted { get; init; }
}