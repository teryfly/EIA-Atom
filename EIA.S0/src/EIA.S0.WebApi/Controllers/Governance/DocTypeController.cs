using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using EIA.S0.Application.Governance.DocTypes;
using EIA.S0.Domain.Core.Specifications.Queries;
using EIA.S0.Domain.Governance.Entities;
using EIA.S0.Contracts.Dtos;

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
    [ProducesResponseType(typeof(IEnumerable<DocTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListAsync([FromQuery] int page = 1, [FromQuery] int size = 50)
    {
        if (page < 1) page = 1;
        if (size < 1) size = 1;
        if (size > 200) size = 200;

        var spec = new AllDocTypeSpecification();
        var list = await _service.ListDocTypesAsync(page, size, spec);

        var result = list.Select(MapToDto);
        return Ok(result);
    }

    /// <summary>
    /// 获取指定 DocType.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DocTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(string id)
    {
        var entity = await _service.GetDocTypeAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(MapToDto(entity));
    }

    /// <summary>
    /// 创建 DocType.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateDocTypeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateDocTypeRequest request)
    {
        var metadataJson = request.Metadata == null ? null : JsonSerializer.Serialize(request.Metadata);
        var customFieldsJson = request.CustomFields == null ? null : JsonSerializer.Serialize(request.CustomFields);

        var entity = await _service.CreateDocTypeAsync(
            request.Code,
            request.Name,
            request.Description,
            request.AllowedPhases,
            request.DefaultPhase,
            request.CategoryId,
            request.AiDraftPromptTemplateId,
            metadataJson,
            customFieldsJson,
            HttpContext.RequestAborted);

        var response = new CreateDocTypeResponse { Id = entity.Id };
        return CreatedAtAction(nameof(GetAsync), new { id = entity.Id }, response);
    }

    /// <summary>
    /// 更新 DocType.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdateDocTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateDocTypeRequest request)
    {
        var metadataJson = request.Metadata == null ? null : JsonSerializer.Serialize(request.Metadata);
        var customFieldsJson = request.CustomFields == null ? null : JsonSerializer.Serialize(request.CustomFields);

        var entity = await _service.UpdateDocTypeAsync(
            id,
            request.Name,
            request.Description,
            request.AllowedPhases,
            request.DefaultPhase,
            request.CategoryId,
            request.AiDraftPromptTemplateId,
            metadataJson,
            customFieldsJson,
            HttpContext.RequestAborted);

        var response = new UpdateDocTypeResponse { Updated = true };
        return Ok(response);
    }

    /// <summary>
    /// 删除 DocType.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(DeleteDocTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var deleted = await _service.DeleteDocTypeAsync(id, HttpContext.RequestAborted);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok(new DeleteDocTypeResponse { Deleted = true });
    }

    private static DocTypeDto MapToDto(DocType entity)
    {
        object? metadataObj = null;
        if (!string.IsNullOrWhiteSpace(entity.MetadataJson))
        {
            try
            {
                metadataObj = JsonSerializer.Deserialize<object>(entity.MetadataJson);
            }
            catch
            {
                metadataObj = null;
            }
        }

        object? customFieldsObj = null;
        if (!string.IsNullOrWhiteSpace(entity.CustomFieldsJson))
        {
            try
            {
                customFieldsObj = JsonSerializer.Deserialize<object>(entity.CustomFieldsJson);
            }
            catch
            {
                customFieldsObj = null;
            }
        }

        return new DocTypeDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,
            AllowedPhases = entity.AllowedPhaseCodes.ToList(),
            DefaultPhase = entity.DefaultPhaseCode,
            CategoryId = entity.CategoryId,
            AiDraftPromptTemplateId = entity.AiDraftPromptTemplateId,
            Metadata = metadataObj,
            CustomFields = customFieldsObj
        };
    }

    /// <summary>
    /// 匹配全部 DocType 的规约.
    /// </summary>
    private sealed class AllDocTypeSpecification : BaseQuerySpecification<DocType>
    {
        public override System.Linq.Expressions.Expression<Func<DocType, bool>> ToExpression()
        {
            return _ => true;
        }
    }
}