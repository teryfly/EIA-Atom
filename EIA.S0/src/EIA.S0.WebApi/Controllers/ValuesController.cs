using MediatR;
using Microsoft.AspNetCore.Mvc;
using EIA.S0.Application.Common.Queries;
using EIA.S0.Application.Features.Values.Commands;
using EIA.S0.Application.Features.Values.Queries;
using EIA.S0.Contracts.Dtos;

namespace EIA.S0.WebApi.Controllers;

/// <summary>
/// value controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ValuesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// 构造.
    /// </summary>
    /// <param name="mediator"></param>
    public ValuesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 创建.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<ResultDto>> CreateAsync(ValueCreateCommand command)
    {
        return await _mediator.Send(command);
    }

    /// <summary>
    /// 获取一些随机数.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<PageResultDto<ValueDto>>> GetAsync([FromQuery] ValueQuery query)
    {
        return Ok(await _mediator.Send(query));
    }
}