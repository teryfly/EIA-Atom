using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EIA.S0.Contracts.Dtos;

namespace EIA.S0.Application.Features.Values.Commands;

/// <summary>
/// 测试创建命令.
/// </summary>
public class ValueCreateCommand : IRequest<ResultDto>
{
    /// <summary>
    /// id.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// name.
    /// </summary>
    public required string Name { get; init; }
}
