namespace EIA.S0.Contracts.Dtos;

/// <summary>
/// PhaseDefinition DTO.
/// </summary>
public class PhaseDto
{
    public string Id { get; init; } = string.Empty;

    public string PhaseCode { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public int Order { get; init; }

    public IReadOnlyCollection<string> AllowedTransitions { get; init; } = Array.Empty<string>();

    public object? Properties { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}

/// <summary>
/// 创建 Phase 请求.
/// </summary>
public class CreatePhaseRequest
{
    public string PhaseCode { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public int Order { get; init; }

    public IReadOnlyCollection<string>? AllowedTransitions { get; init; }

    public object? Properties { get; init; }
}

/// <summary>
/// 创建 Phase 响应.
/// </summary>
public class CreatePhaseResponse
{
    public string Id { get; init; } = string.Empty;
}

/// <summary>
/// 更新 Phase 请求.
/// </summary>
public class UpdatePhaseRequest
{
    public string DisplayName { get; init; } = string.Empty;

    public int Order { get; init; }

    public IReadOnlyCollection<string>? AllowedTransitions { get; init; }

    public object? Properties { get; init; }
}

/// <summary>
/// 更新 Phase 响应.
/// </summary>
public class UpdatePhaseResponse
{
    public bool Updated { get; init; }
}

/// <summary>
/// 删除 Phase 响应.
/// </summary>
public class DeletePhaseResponse
{
    public bool Deleted { get; init; }
}