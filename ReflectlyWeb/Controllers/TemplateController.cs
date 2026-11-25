using Application.Behavior.Templates.Commands;
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
}