using Microsoft.EntityFrameworkCore;
using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Core.Specifications.Queries;
using EIA.S0.Domain.Governance.Entities;
using EIA.S0.Infrastructure.EntityFrameworkCore;

namespace EIA.S0.Infrastructure.Governance.Repositories;

/// <summary>
/// PhaseDefinition 仓储.
/// </summary>
public class PhaseRepository : IRepository<PhaseDefinition>
{
    private readonly EiaS0dbContext _context;
    private readonly DbSet<PhaseDefinition> _set;

    /// <summary>
    /// 构造.
    /// </summary>
    /// <param name="context"></param>
    public PhaseRepository(EiaS0dbContext context)
    {
        _context = context;
        _set = context.Set<PhaseDefinition>();
    }

    /// <inheritdoc />
    public async Task<PhaseDefinition?> GetAsync(string id)
    {
        return await _set.FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// 根据阶段编码获取 PhaseDefinition.
    /// </summary>
    public async Task<PhaseDefinition?> GetByCodeAsync(string phaseCode)
    {
        return await _set.FirstOrDefaultAsync(x => x.PhaseCode == phaseCode);
    }

    /// <inheritdoc />
    public PhaseDefinition Add(PhaseDefinition entity)
    {
        return _set.Add(entity).Entity;
    }

    /// <inheritdoc />
    public PhaseDefinition Update(PhaseDefinition entity)
    {
        return _set.Update(entity).Entity;
    }

    /// <inheritdoc />
    public void Delete(PhaseDefinition entity)
    {
        _set.Remove(entity);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PhaseDefinition>> GetListAsync(IQuerySpecification<PhaseDefinition> spec)
    {
        return await _set.Where(spec.ToExpression()).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(IQuerySpecification<PhaseDefinition> spec)
    {
        return await _set.Where(spec.ToExpression()).AnyAsync();
    }
}