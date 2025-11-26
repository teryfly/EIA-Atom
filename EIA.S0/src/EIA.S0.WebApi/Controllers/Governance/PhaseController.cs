using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using EIA.S0.Application.Governance.Phases;
using EIA.S0.Domain.Core.Exceptions;
using EIA.S0.Domain.Core.Specifications.Queries;
using EIA.S0.Domain.Governance.Entities;
using EIA.S0.Contracts.Dtos;

namespace EIA.S0.WebApi.Controllers.Governance;

/// <summary>
/// PhaseDefinition API.
/// </summary>
[ApiController]
[Route("api/governance/[controller]")]
public class PhaseController : ControllerBase
{
    private readonly PhaseService _service;

    /// <summary>
    /// 构造.
    /// </summary>
    public PhaseController(PhaseService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取 PhaseDefinition 列表.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PhaseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListAsync(
        [FromQuery] int page = 1,
        [FromQuery] int size = 50)
    {
        if (page < 1) page = 1;
        if (size < 1) size = 1;
        if (size > 200) size = 200;

        var spec = new AllPhaseSpecification();
        var list = await _service.GetListAsync(page, size, spec);

        var result = list.Select(MapToDto);
        return Ok(result);
    }

    /// <summary>
    /// 获取指定 PhaseDefinition.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PhaseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(string id)
    {
        var entity = await _service.GetAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(MapToDto(entity));
    }

    /// <summary>
    /// 创建 PhaseDefinition.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreatePhaseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePhaseRequest request)
    {
        try
        {
            var propertiesJson = request.Properties == null
                ? null
                : JsonSerializer.Serialize(request.Properties);

            var entity = await _service.CreateAsync(
                request.PhaseCode,
                request.DisplayName,
                request.Order,
                request.AllowedTransitions ?? Array.Empty<string>(),
                propertiesJson,
                HttpContext.RequestAborted);

            var response = new CreatePhaseResponse { Id = entity.Id };
            return CreatedAtAction(nameof(GetAsync), new { id = entity.Id }, response);
        }
        catch (DomainException)
        {
            throw;
        }
    }

    /// <summary>
    /// 更新 PhaseDefinition.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdatePhaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdatePhaseRequest request)
    {
        try
        {
            var propertiesJson = request.Properties == null
                ? null
                : JsonSerializer.Serialize(request.Properties);

            var entity = await _service.UpdateAsync(
                id,
                request.DisplayName,
                request.Order,
                request.AllowedTransitions ?? Array.Empty<string>(),
                propertiesJson,
                HttpContext.RequestAborted);

            var response = new UpdatePhaseResponse { Updated = true };
            return Ok(response);
        }
        catch (DomainException)
        {
            throw;
        }
    }

    /// <summary>
    /// 删除 PhaseDefinition.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(DeletePhaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var deleted = await _service.DeleteAsync(id, force: false, HttpContext.RequestAborted);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok(new DeletePhaseResponse { Deleted = true });
    }

    private static PhaseDto MapToDto(PhaseDefinition entity)
    {
        object? propertiesObj = null;
        if (!string.IsNullOrWhiteSpace(entity.PropertiesJson))
        {
            try
            {
                propertiesObj = JsonSerializer.Deserialize<object>(entity.PropertiesJson);
            }
            catch
            {
                propertiesObj = null;
            }
        }

        return new PhaseDto
        {
            Id = entity.Id,
            PhaseCode = entity.PhaseCode,
            DisplayName = entity.DisplayName,
            Order = entity.Order,
            AllowedTransitions = entity.AllowedTransitionPhaseCodes.ToList(),
            Properties = propertiesObj,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    /// <summary>
    /// 匹配全部 Phase 的规约.
    /// </summary>
    private sealed class AllPhaseSpecification : BaseQuerySpecification<PhaseDefinition>
    {
        public override System.Linq.Expressions.Expression<Func<PhaseDefinition, bool>> ToExpression()
        {
            return _ => true;
        }
    }
}