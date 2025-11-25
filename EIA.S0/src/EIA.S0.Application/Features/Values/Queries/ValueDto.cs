using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIA.S0.Application.Features.Values.Queries;

/// <summary>
/// value dto.
/// </summary>
public class ValueDto
{
    /// <summary>
    /// 构造.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public ValueDto(string id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// name.
    /// </summary>
    public string Name { get; set; }
}
