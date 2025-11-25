using Microsoft.EntityFrameworkCore;
using EIA.S0.Domain.Governance.Entities;
using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Infrastructure.EntityFrameworkCore;

namespace EIA.S0.Infrastructure.Governance.Repositories;

/// <summary>
/// DocType 仓储.
/// </summary>
public class DocTypeRepository : IRepository<DocType>
{
    private readonly EiaS0dbContext _context;
    private readonly DbSet<DocType> _set;

    /// <summary>
    /// 构造.
    /// </summary>
    /// <param name="context"></param>
    public DocTypeRepository(EiaS0dbContext context)
    {
        _context = context;
        _set = context.Set<DocType>();
    }

    /// <inheritdoc />
    public async Task<DocType?> GetAsync(string id)
    {
        return await _set.FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <inheritdoc />
    public DocType Add(DocType entity)
    {
        return _set.Add(entity).Entity;
    }

    /// <inheritdoc />
    public DocType Update(DocType entity)
    {
        return _set.Update(entity).Entity;
    }

    /// <inheritdoc />
    public void Delete(DocType entity)
    {
        _set.Remove(entity);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DocType>> GetListAsync(EIA.S0.Domain.Core.Specifications.Queries.IQuerySpecification<DocType> spec)
    {
        return await _set.Where(spec.ToExpression()).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(EIA.S0.Domain.Core.Specifications.Queries.IQuerySpecification<DocType> spec)
    {
        return await _set.Where(spec.ToExpression()).AnyAsync();
    }

    /// <summary>
    /// 通过编码获取 DocType.
    /// </summary>
    public async Task<DocType?> GetByCodeAsync(string code)
    {
        return await _set.FirstOrDefaultAsync(x => x.Code == code);
    }
}