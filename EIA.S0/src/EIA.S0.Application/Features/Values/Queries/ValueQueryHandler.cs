using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EIA.S0.Application.Common.Queries;

namespace EIA.S0.Application.Features.Values.Queries;

/// <summary>
/// value query handler.
/// </summary>
public class ValueQueryHandler : IRequestHandler<ValueQuery, PageResultDto<ValueDto>>
{
    /// <summary>
    /// handle.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<PageResultDto<ValueDto>> Handle(ValueQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new PageResultDto<ValueDto>
        {
            Total = 0,
        });
    }
}
