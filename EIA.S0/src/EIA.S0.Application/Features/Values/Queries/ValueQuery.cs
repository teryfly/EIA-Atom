using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EIA.S0.Application.Common.Queries;

namespace EIA.S0.Application.Features.Values.Queries;

/// <summary>
/// value query.
/// </summary>
public class ValueQuery : PageQuery<PageResultDto<ValueDto>>
{
    /// <summary>
    /// name.
    /// </summary>
    public string? Name { get; set; }
}
