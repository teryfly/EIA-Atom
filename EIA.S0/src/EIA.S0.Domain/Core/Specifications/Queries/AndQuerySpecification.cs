using System.Linq.Expressions;
using EIA.S0.Domain.Core.Entities;

namespace EIA.S0.Domain.Core.Specifications.Queries;

public class AndQuerySpecification<TEntity> : BaseQuerySpecification<TEntity>, IQuerySpecification<TEntity>
    where TEntity : IEntity
{
    private readonly IQuerySpecification<TEntity> _left;
    private readonly IQuerySpecification<TEntity> _right;

    /// <summary>
    /// 构造.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public AndQuerySpecification(IQuerySpecification<TEntity> left, IQuerySpecification<TEntity> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        var leftExp = _left.ToExpression();
        var rightExp = _right.ToExpression();

        var param = Expression.Parameter(typeof(TEntity));
        var body = Expression.AndAlso(
            Expression.Invoke(leftExp, param),
            Expression.Invoke(rightExp, param)
        );

        return Expression.Lambda<Func<TEntity, bool>>(body, param);
    }
}