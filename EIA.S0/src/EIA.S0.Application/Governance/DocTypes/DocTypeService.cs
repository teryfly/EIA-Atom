using EIA.S0.Domain.Core.Exceptions;
using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Governance.Entities;

namespace EIA.S0.Application.Governance.DocTypes;

/// <summary>
/// DocType 应用服务.
/// </summary>
public partial class DocTypeService
{
    private readonly IRepository<DocType> _docTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// 核心构造函数（供 Events 部分调用）.
    /// </summary>
    public DocTypeService(
        IRepository<DocType> docTypeRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
    {
        _docTypeRepository = docTypeRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 校验 defaultPhase 必须在 allowedPhases 中.
    /// </summary>
    private static void EnsureDefaultPhaseInAllowedPhases(IEnumerable<string> allowedPhases, string defaultPhase)
    {
        var list = (allowedPhases ?? Array.Empty<string>()).ToList();
        if (!list.Any())
        {
            throw new DomainException("allowedPhases 不能为空。");
        }

        if (!list.Contains(defaultPhase))
        {
            throw new DomainException("defaultPhase 必须包含在 allowedPhases 中。");
        }
    }

    /// <summary>
    /// 校验 allowedPhases 中的每个阶段码在 PhaseDefinition 中存在（占位，后续接入 Phase 仓储）.
    /// </summary>
    private static Task EnsureAllAllowedPhasesExistAsync(IEnumerable<string> allowedPhases, CancellationToken cancellationToken)
    {
        // TODO: 接入 PhaseDefinition 仓储进行存在性校验
        return Task.CompletedTask;
    }

    /// <summary>
    /// 创建 DocType.
    /// </summary>
    public virtual async Task<DocType> CreateDocTypeAsync(
        string code,
        string name,
        string? description,
        IEnumerable<string> allowedPhases,
        string defaultPhase,
        Guid? categoryId,
        Guid? aiDraftPromptTemplateId,
        string? metadataJson,
        string? customFieldsJson,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("DocType code 不能为空.");
        }

        await EnsureAllAllowedPhasesExistAsync(allowedPhases, cancellationToken);
        EnsureDefaultPhaseInAllowedPhases(allowedPhases, defaultPhase);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var entity = new DocType(
            Guid.NewGuid(),
            code,
            name,
            description,
            allowedPhases,
            defaultPhase,
            categoryId,
            aiDraftPromptTemplateId,
            metadataJson,
            customFieldsJson,
            now,
            now);

        _docTypeRepository.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await PublishDocTypeChangedAsync(entity, "CREATED", cancellationToken);

        return entity;
    }

    /// <summary>
    /// 更新 DocType.
    /// </summary>
    public virtual async Task<DocType> UpdateDocTypeAsync(
        string id,
        string name,
        string? description,
        IEnumerable<string> allowedPhases,
        string defaultPhase,
        Guid? categoryId,
        Guid? aiDraftPromptTemplateId,
        string? metadataJson,
        string? customFieldsJson,
        CancellationToken cancellationToken = default)
    {
        var entity = await _docTypeRepository.GetAsync(id);
        if (entity == null)
        {
            throw new DomainException("DocType 不存在.");
        }

        await EnsureAllAllowedPhasesExistAsync(allowedPhases, cancellationToken);
        EnsureDefaultPhaseInAllowedPhases(allowedPhases, defaultPhase);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        entity.UpdateBasicInfo(
            name,
            description,
            allowedPhases,
            defaultPhase,
            categoryId,
            aiDraftPromptTemplateId,
            metadataJson,
            customFieldsJson,
            now);

        _docTypeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await PublishDocTypeChangedAsync(entity, "UPDATED", cancellationToken);

        return entity;
    }

    /// <summary>
    /// 获取 DocType.
    /// </summary>
    public virtual async Task<DocType?> GetDocTypeAsync(string id)
    {
        return await _docTypeRepository.GetAsync(id);
    }

    /// <summary>
    /// 删除 DocType (物理删除).
    /// </summary>
    public virtual async Task<bool> DeleteDocTypeAsync(string id, CancellationToken cancellationToken = default)
    {
        var entity = await _docTypeRepository.GetAsync(id);
        if (entity == null)
        {
            return false;
        }

        _docTypeRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await PublishDocTypeChangedAsync(entity, "DELETED", cancellationToken);

        return true;
    }
}