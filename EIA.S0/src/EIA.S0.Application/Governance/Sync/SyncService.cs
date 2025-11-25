using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Governance.Entities;

namespace EIA.S0.Application.Governance.Sync;

/// <summary>
/// 全量同步服务（简化版），包含 DocType 部分.
/// </summary>
public class SyncService
{
    private readonly IRepository<DocType> _docTypeRepository;

    public SyncService(IRepository<DocType> docTypeRepository)
    {
        _docTypeRepository = docTypeRepository;
    }

    /// <summary>
    /// 获取所有 DocType，用于构建全量同步负载.
    /// </summary>
    public async Task<IReadOnlyCollection<DocType>> GetAllDocTypesAsync(EIA.S0.Domain.Core.Specifications.Queries.IQuerySpecification<DocType> spec)
    {
        var list = await _docTypeRepository.GetListAsync(spec);
        return list.ToList();
    }
}