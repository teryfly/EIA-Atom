using EIA.S0.Domain.Core.Exceptions;
using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Core.Specifications.Queries;
using EIA.S0.Domain.Governance.Entities;

namespace EIA.S0.Application.Governance.Phases;

/// <summary>
/// PhaseDefinition 应用服务（核心逻辑部分，不含事件/缓存集成）。
/// </summary>
public partial class PhaseService
{
    private readonly IRepository<PhaseDefinition> _phaseRepository;
    private readonly IRepository<DocType> _docTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    protected readonly TimeProvider _timeProvider;

    /// <summary>
    /// 核心构造函数（供 Events 部分调用）.
    /// </summary>
    public PhaseService(
        IRepository<PhaseDefinition> phaseRepository,
        IRepository<DocType> docTypeRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
    {
        _phaseRepository = phaseRepository;
        _docTypeRepository = docTypeRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 创建 PhaseDefinition.
    /// </summary>
    public virtual async Task<PhaseDefinition> CreateAsync(
        string phaseCode,
        string displayName,
        int order,
        IEnumerable<string>? allowedTransitions,
        string? propertiesJson,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phaseCode))
        {
            throw new DomainException("phaseCode 不能为空.");
        }

        // 简单唯一性校验（并发下仍依赖 DB 约束防护）
        await EnsurePhaseCodeUniqueAsync(phaseCode, cancellationToken);

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var phase = new PhaseDefinition(
            Guid.NewGuid(),
            phaseCode,
            displayName,
            order,
            allowedTransitions,
            propertiesJson,
            now,
            now);

        // 校验 allowedTransitions
        await EnsureAllowedTransitionsValidAsync(phase.AllowedTransitionPhaseCodes, cancellationToken);

        _phaseRepository.Add(phase);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await PublishPhaseChangedAsync(phase, "CREATED", cancellationToken);

        return phase;
    }

    /// <summary>
    /// 更新 PhaseDefinition.
    /// </summary>
    public virtual async Task<PhaseDefinition> UpdateAsync(
        string id,
        string displayName,
        int order,
        IEnumerable<string>? allowedTransitions,
        string? propertiesJson,
        CancellationToken cancellationToken = default)
    {
        var phase = await _phaseRepository.GetAsync(id);
        if (phase == null)
        {
            throw new DomainException("PhaseDefinition 不存在.");
        }

        var allowed = allowedTransitions?.ToList() ?? new List<string>();
        await EnsureAllowedTransitionsValidAsync(allowed, cancellationToken);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        phase.UpdateBasicInfo(displayName, order, allowed, propertiesJson, now);

        _phaseRepository.Update(phase);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await PublishPhaseChangedAsync(phase, "UPDATED", cancellationToken);

        return phase;
    }

    /// <summary>
    /// 获取单个 PhaseDefinition.
    /// </summary>
    public virtual async Task<PhaseDefinition?> GetAsync(string id)
    {
        return await _phaseRepository.GetAsync(id);
    }

    /// <summary>
    /// 简单分页获取 PhaseDefinition 列表（按顺序号排序）.
    /// </summary>
    public virtual async Task<IReadOnlyCollection<PhaseDefinition>> GetListAsync(
        int page,
        int size,
        IQuerySpecification<PhaseDefinition> spec)
    {
        var list = await _phaseRepository.GetListAsync(spec);
        return list
            .OrderBy(x => x.Order)
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();
    }

    /// <summary>
    /// 删除 PhaseDefinition.
    /// </summary>
    public virtual async Task<bool> DeleteAsync(
        string id,
        bool force = false,
        CancellationToken cancellationToken = default)
    {
        var phase = await _phaseRepository.GetAsync(id);
        if (phase == null)
        {
            return false;
        }

        await EnsureDeleteAllowedAsync(phase, force, cancellationToken);

        _phaseRepository.Delete(phase);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await PublishPhaseChangedAsync(phase, "DELETED", cancellationToken);

        return true;
    }

    /// <summary>
    /// 确保 phaseCode 唯一.
    /// </summary>
    private async Task EnsurePhaseCodeUniqueAsync(string phaseCode, CancellationToken cancellationToken)
    {
        var spec = new PhaseByCodeSpecification(phaseCode);
        var any = await _phaseRepository.AnyAsync(spec);
        if (any)
        {
            throw new DomainException("阶段编码已存在.");
        }
    }

    /// <summary>
    /// 校验 allowedTransitions 引用的 phaseCode 是否存在.
    /// </summary>
    private async Task EnsureAllowedTransitionsValidAsync(
        IEnumerable<string> allowedTransitions,
        CancellationToken cancellationToken)
    {
        var codes = allowedTransitions.Distinct().ToList();
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
                throw new DomainException($"allowedTransitions 中的阶段编码[{code}]不存在.");
            }
        }
    }

    /// <summary>
    /// 删除前检查（SystemParameter 与 DocType 引用等在另外的 partial 中扩展）.
    /// </summary>
    private Task EnsureDeleteAllowedAsync(
        PhaseDefinition phase,
        bool force,
        CancellationToken cancellationToken)
    {
        // 当前实现为占位，后续将接入 SystemParameter 与 DocType 引用检查。
        return Task.CompletedTask;
    }

    /// <summary>
    /// 通过 phaseCode 的规约.
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