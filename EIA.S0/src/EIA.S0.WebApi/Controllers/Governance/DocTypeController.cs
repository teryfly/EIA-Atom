using Microsoft.AspNetCore.Mvc;
using EIA.S0.Application.Governance.DocTypes;
using EIA.S0.Domain.Governance.Entities;

namespace EIA.S0.WebApi.Controllers.Governance;

/// <summary>
/// DocType API.
/// </summary>
[ApiController]
[Route("api/governance/[controller]")]
public class DocTypeController : ControllerBase
{
    private readonly DocTypeService _service;

    /// <summary>
    /// 构造.
    /// </summary>
    /// <param name="service"></param>
    public DocTypeController(DocTypeService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取 DocType 列表.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DocType>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListAsync([FromQuery] int page = 1, [FromQuery] int size = 50)
    {
        // TODO: 使用真正分页逻辑; 当前仅返回空集合占位
        return Ok(Array.Empty<DocType>());
    }

    // ... other actions unchanged ...
}