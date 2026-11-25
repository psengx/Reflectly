using System.Net;
using Application.Behavior.Templates.Commands;
using Application.Behavior.Templates.Queries;
using Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReflectlyWeb.DTO.Template;

namespace ReflectlyWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TemplateController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<TemplateDto> CreateTemplate([FromBody] TemplateToCreateOrUpdateDto dto)
    {
        CreateTemplateCommand command = new()
        {
            Name = dto.Name,
            Fields = dto.Fields
        };

        TemplateEntity result = await mediator.Send(command);
        
        return result.Adapt<TemplateDto>();
    }

    [HttpGet]
    public async Task<List<TemplateDto>> GetAllTemplates()
    {
        GetAllTemplatesQuery query = new();
        List<TemplateEntity> result = await mediator.Send(query);
        return result.Adapt<List<TemplateDto>>();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TemplateDto>> GetTemplateById([FromRoute] Guid id)
    {
        GetTemplateByIdQuery query = new() { Id = id };
        TemplateEntity? result = await mediator.Send(query);
        
        return result == null ? NotFound() : result.Adapt<TemplateDto>();
    }

    [HttpPut("{id}/update")]
    public async Task<ActionResult<TemplateDto>> UpdateTemplate([FromRoute] Guid id, [FromBody] TemplateToCreateOrUpdateDto dto)
    {
        UpdateTemplateCommand command = new()
        {
            Id = id,
            Name = dto.Name,
            Fields = dto.Fields
        };
        TemplateEntity? result = await mediator.Send(command);
        
        return result == null ? NotFound() : result.Adapt<TemplateDto>();
    }
}