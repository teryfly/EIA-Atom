using EIA.S0.Domain.Core.Entities;

namespace EIA.S0.Domain.Core.Specifications.Validations;

internal class AndValidationSpecification<TEntity> : BaseValidationSpecification<TEntity>
    where TEntity : IEntity
{
    private readonly IValidationSpecification<TEntity> _left;
    private readonly IValidationSpecification<TEntity> _right;

    /// <summary>
    /// and.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    internal AndValidationSpecification(IValidationSpecification<TEntity> left, IValidationSpecification<TEntity> right)
    {
        _left = left;
        _right = right;
    }

    public override bool IsSatisfiedBy(TEntity entity)
    {
        return _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);
    }

    public override string GetErrorMessage(TEntity entity)
    {
        return _left.IsSatisfiedBy(entity) ? _right.GetErrorMessage(entity) : _left.GetErrorMessage(entity);
    }
}