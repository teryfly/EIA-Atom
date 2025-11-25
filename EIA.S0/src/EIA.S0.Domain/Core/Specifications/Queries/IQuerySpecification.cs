using System.Linq.Expressions;
using EIA.S0.Domain.Core.Entities;

namespace EIA.S0.Domain.Core.Specifications.Queries;

/// <summary>
/// 查询型规约.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IQuerySpecification<TEntity> : ISpecification<TEntity>
    where TEntity : IEntity
{
    Expression<Func<TEntity, bool>> ToExpression();
}