using EIA.S0.Domain.Core.Entities;

namespace EIA.S0.Domain.Core.Specifications.Validations;

/// <summary>
/// 验证型规约.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
internal interface IValidationSpecification<TEntity> : ISpecification<TEntity>
    where TEntity : IEntity
{
    void EnsureSatisfied(TEntity entity);

    string GetErrorMessage(TEntity entity);
}