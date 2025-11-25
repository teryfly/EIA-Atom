using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EIA.S0.Domain.Core.Entities;

namespace EIA.S0.Domain.Core.Specifications;

public interface ISpecification<TEntity>
    where TEntity : IEntity
{
    bool IsSatisfiedBy(TEntity entity);
}
