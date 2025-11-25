using EIA.S0.Domain.Core.Entities;
using EIA.S0.Domain.Core.Exceptions;

namespace EIA.S0.Domain.Core.Specifications.Validations;

public abstract class BaseValidationSpecification<TEntity> : IValidationSpecification<TEntity>
    where TEntity : IEntity
{
    public abstract bool IsSatisfiedBy(TEntity entity);
    
    public void EnsureSatisfied(TEntity entity)
    {
        if (!IsSatisfiedBy(entity))
            throw new DomainException(GetErrorMessage(entity));
    }
    
    public abstract string GetErrorMessage(TEntity entity);
}