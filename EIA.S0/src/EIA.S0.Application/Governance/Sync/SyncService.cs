using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Core.Specifications.Queries;
using EIA.S0.Domain.Governance.Entities;

namespace EIA.S0.Application.Governance.Sync;

/// <summary>
/// 全量同步服务（简化版），包含 DocType 与 PhaseDefinition 部分.
/// </summary>
public class SyncService
{
    private readonly IRepository<DocType> _docTypeRepository;
    private readonly IRepository<PhaseDefinition> _phaseRepository;

    public SyncService(
        IRepository<DocType> docTypeRepository,
        IRepository<PhaseDefinition> phaseRepository)
    {
        _docTypeRepository = docTypeRepository;
        _phaseRepository = phaseRepository;
    }

    /// <summary>
    /// 获取所有 DocType，用于构建全量同步负载.
    /// </summary>
    public async Task<IReadOnlyCollection<DocType>> GetAllDocTypesAsync(IQuerySpecification<DocType> spec)
    {
        var list = await _docTypeRepository.GetListAsync(spec);
        return list.ToList();
    }

    /// <summary>
    /// 获取所有 PhaseDefinition，用于构建全量同步负载.
    /// </summary>
    public async Task<IReadOnlyCollection<PhaseDefinition>> GetAllPhasesAsync(IQuerySpecification<PhaseDefinition> spec)
    {
        var list = await _phaseRepository.GetListAsync(spec);
        return list.ToList();
    }
}