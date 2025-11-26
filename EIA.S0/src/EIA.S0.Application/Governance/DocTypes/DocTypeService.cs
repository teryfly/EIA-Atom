using EIA.S0.Domain.Core.Exceptions;
using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Core.Specifications.Queries;
using EIA.S0.Domain.Governance.Entities;

namespace EIA.S0.Application.Governance.DocTypes;

/// <summary>
/// DocType 应用服务.
/// </summary>
public partial class DocTypeService
{
    private readonly IRepository<DocType> _docTypeRepository;
    private readonly IRepository<PhaseDefinition> _phaseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// 核心构造函数（供 Events 部分调用）.
    /// </summary>
    public DocTypeService(
        IRepository<DocType> docTypeRepository,
        IRepository<PhaseDefinition> phaseRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
    {
        _docTypeRepository = docTypeRepository;
        _phaseRepository = phaseRepository;
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
    /// 校验 allowedPhases 中的每个阶段码在 PhaseDefinition 中存在.
    /// </summary>
    private async Task EnsureAllAllowedPhasesExistAsync(IEnumerable<string> allowedPhases, CancellationToken cancellationToken)
    {
        var codes = (allowedPhases ?? Array.Empty<string>())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (!codes.Any())
        {
            return;
        }

        foreach (var code in codes)
        {
            var spec = new PhaseByCodeSpecification(code);
            var exists = await _phaseRepository.AnyAsync(spec);
            if (!exists)
            {
                throw new DomainException($"allowedPhases 中的阶段编码[{code}]不存在.");
            }
        }
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

        var allowedList = (allowedPhases ?? Array.Empty<string>()).ToList();

        await EnsureAllAllowedPhasesExistAsync(allowedList, cancellationToken);
        EnsureDefaultPhaseInAllowedPhases(allowedList, defaultPhase);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var entity = new DocType(
            Guid.NewGuid(),
            code,
            name,
            description,
            allowedList,
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

        var allowedList = (allowedPhases ?? Array.Empty<string>()).ToList();

        await EnsureAllAllowedPhasesExistAsync(allowedList, cancellationToken);
        EnsureDefaultPhaseInAllowedPhases(allowedList, defaultPhase);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        entity.UpdateBasicInfo(
            name,
            description,
            allowedList,
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

    /// <summary>
    /// 通过 phaseCode 的规约（供 allowedPhases 校验使用）.
    /// </summary>
    private sealed class PhaseByCodeSpecification : BaseQuerySpecification<PhaseDefinition>
    {
        private readonly string _code;

        public PhaseByCodeSpecification(string code)
        {
            _code = code;
        }

        public override System.Linq.Expressions.Expression<Func<PhaseDefinition, bool>> ToExpression()
        {
            return x => x.PhaseCode == _code;
        }
    }
}