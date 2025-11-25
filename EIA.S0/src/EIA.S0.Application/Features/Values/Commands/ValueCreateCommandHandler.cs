using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EIA.S0.Contracts.Dtos;

namespace EIA.S0.Application.Features.Values.Commands;

/// <summary>
/// value create command handler.
/// </summary>
public class ValueCreateCommandHandler : IRequestHandler<ValueCreateCommand, ResultDto>
{
    /// <summary>
    /// handle.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ResultDto> Handle(ValueCreateCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(ResultDto.Success());
    }
}
