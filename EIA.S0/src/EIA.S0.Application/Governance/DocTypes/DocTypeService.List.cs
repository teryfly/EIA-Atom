using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Core.Specifications.Queries;
using EIA.S0.Domain.Governance.Entities;

namespace EIA.S0.Application.Governance.DocTypes;

/// <summary>
/// DocTypeService 列表查询部分.
/// </summary>
public partial class DocTypeService
{
    /// <summary>
    /// 简单分页获取 DocType 列表（按 Code 排序）.
    /// </summary>
    public virtual async Task<IReadOnlyCollection<DocType>> ListDocTypesAsync(
        int page,
        int size,
        IQuerySpecification<DocType> spec)
    {
        var list = await _docTypeRepository.GetListAsync(spec);
        return list
            .OrderBy(x => x.Code)
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();
    }
}