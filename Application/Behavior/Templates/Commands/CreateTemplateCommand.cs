using Application.DTO;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Behavior.Templates.Commands;

public class CreateTemplateCommand : IRequest<Guid>
{
    public string Name { get; set; }
    public List<TemplateFieldToCreateDto> Fields { get; set; }
}

public class CreateTemplateCommandHandler(
    ITemplateRepository templateRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTemplateCommand, Guid>
{
    public async Task<Guid> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
    {
        TemplateEntity template = new()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            IsArchived = false
        };
        foreach (TemplateFieldToCreateDto field in request.Fields)
        {
            TemplateFieldEntity fieldEntity = new()
            {
                Id = Guid.NewGuid(),
                TemplateId = template.Id,
                Name = field.Name,
                FieldType = field.FieldType,
                IsRequired = field.IsRequired,
                Order = field.Order,
                Settings = field.Settings
            };
            template.Fields.Add(fieldEntity);
        }
        
        await templateRepository.CreateTemplateAsync(template);
        await unitOfWork.SaveChangesAsync();
        
        return template.Id;
    }
}