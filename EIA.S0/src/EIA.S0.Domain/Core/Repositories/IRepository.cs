using EIA.S0.Domain.Core.Aggregates;
using EIA.S0.Domain.Core.Specifications.Queries;

namespace EIA.S0.Domain.Core.Repositories;

public interface IRepository<TAggregateRoot>
    where TAggregateRoot : AggregateRoot
{
    Task<TAggregateRoot?> GetAsync(string id);
    
    TAggregateRoot Add(TAggregateRoot entity);
    
    TAggregateRoot Update(TAggregateRoot entity);
    
    void Delete(TAggregateRoot entity);

    Task<IEnumerable<TAggregateRoot>> GetListAsync(IQuerySpecification<TAggregateRoot> spec);

    Task<bool> AnyAsync(IQuerySpecification<TAggregateRoot> spec);
}