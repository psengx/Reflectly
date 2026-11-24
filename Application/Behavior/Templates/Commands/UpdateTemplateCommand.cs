using Application.DTO;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Behavior.Templates.Commands;

public class UpdateTemplateCommand : IRequest<TemplateEntity?>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<TemplateFieldToCreateOrUpdateDto> Fields { get; set; }
}

public class UpdateTemplateCommandHandler(
    ITemplateRepository templateRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateTemplateCommand, TemplateEntity?>
{
    public async Task<TemplateEntity?> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
    {
        TemplateEntity? template = await templateRepository.GetTemplateByIdAsync(request.Id);
        if (template == null)
            return null;
        
        template.Name = request.Name;
        template.Fields.Clear();
        
        foreach (TemplateFieldToCreateOrUpdateDto field in request.Fields)
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
        
        await unitOfWork.SaveChangesAsync();
        
        return template;
    }
}