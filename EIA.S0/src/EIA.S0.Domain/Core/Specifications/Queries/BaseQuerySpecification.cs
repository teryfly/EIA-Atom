using System.Linq.Expressions;
using EIA.S0.Domain.Core.Entities;

namespace EIA.S0.Domain.Core.Specifications.Queries;

public abstract class BaseQuerySpecification<TEntity>: IQuerySpecification<TEntity>
    where TEntity : IEntity
{
    public abstract Expression<Func<TEntity, bool>> ToExpression();

    public bool IsSatisfiedBy(TEntity entity)
    {
        return ToExpression().Compile().Invoke(entity);
    }
}