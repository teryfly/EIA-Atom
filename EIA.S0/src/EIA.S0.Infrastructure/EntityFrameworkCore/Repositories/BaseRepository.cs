using Microsoft.EntityFrameworkCore;
using EIA.S0.Domain.Core.Aggregates;
using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Core.Specifications;
using EIA.S0.Domain.Core.Specifications.Queries;

namespace EIA.S0.Infrastructure.EntityFrameworkCore.Repositories;

/// <summary>
/// base repository.
/// </summary>
/// <typeparam name="TAggregateRoot"></typeparam>
/// <typeparam name="TDbContext"></typeparam>
public abstract class BaseRepository<TAggregateRoot, TDbContext> : IRepository<TAggregateRoot>
    where TAggregateRoot : AggregateRoot
    where TDbContext : DbContext
{
    protected readonly TDbContext _context;
    protected readonly DbSet<TAggregateRoot> _dbSet;

    /// <summary>
    /// 构造.
    /// </summary>
    /// <param name="dbContext"></param>
    protected BaseRepository(TDbContext dbContext)
    {
        _context = dbContext;
        _dbSet = dbContext.Set<TAggregateRoot>();
    }

    public async Task<TAggregateRoot?> GetAsync(string id)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Id == id);
    }

    public TAggregateRoot Add(TAggregateRoot entity)
    {
        return _dbSet.Add(entity).Entity;
    }

    public TAggregateRoot Update(TAggregateRoot entity)
    {
        return _dbSet.Update(entity).Entity;
    }

    public void Delete(TAggregateRoot entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<IEnumerable<TAggregateRoot>> GetListAsync(IQuerySpecification<TAggregateRoot> spec)
    {
        return await _dbSet
            .Where(spec.ToExpression())
            .ToListAsync();
    }

    public async Task<bool> AnyAsync(IQuerySpecification<TAggregateRoot> spec)
    {
        return await _dbSet
            .Where(spec.ToExpression())
            .AnyAsync();
    }
}