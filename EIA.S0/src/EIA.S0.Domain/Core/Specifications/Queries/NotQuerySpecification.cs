using System.Linq.Expressions;
using EIA.S0.Domain.Core.Entities;

namespace EIA.S0.Domain.Core.Specifications.Queries;

public class NotQuerySpecification<TEntity> : BaseQuerySpecification<TEntity>, IQuerySpecification<TEntity>
    where TEntity : IEntity
{
    private readonly IQuerySpecification<TEntity> _inner;

    /// <summary>
    /// 构造.
    /// </summary>
    /// <param name="inner"></param>
    public NotQuerySpecification(IQuerySpecification<TEntity> inner)
    {
        _inner = inner;
    }

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        var innerExp = _inner.ToExpression();
        var param = Expression.Parameter(typeof(TEntity));
        var body = Expression.Not(Expression.Invoke(innerExp, param));
        return Expression.Lambda<Func<TEntity, bool>>(body, param);
    }
}