using EIA.S0.Domain.Core.Entities;
using EIA.S0.Domain.Core.Specifications.Queries;
using EIA.S0.Domain.Core.Specifications.Validations;

namespace EIA.S0.Domain.Core.Specifications;

/// <summary>
/// 扩展.
/// </summary>
public static class SpecificationExtensions
{
    /// <summary>
    /// and validation.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    internal static IValidationSpecification<TEntity> And<TEntity>(this IValidationSpecification<TEntity> left, IValidationSpecification<TEntity> right)
        where TEntity : IEntity
    {
        return new AndValidationSpecification<TEntity>(left, right);
    }
    
    /// <summary>
    /// and query.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IQuerySpecification<TEntity> And<TEntity>(this IQuerySpecification<TEntity> left, IQuerySpecification<TEntity> right)
        where TEntity : IEntity
    {
        return new AndQuerySpecification<TEntity>(left, right);
    }
}