using Application.DTO;
using Application.Interfaces.Repositories;
using Domain.Entities;
using FluentValidation;
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
        HashSet<Guid> incomingIds = request.Fields
            .Where(f => f.Id.HasValue)
            .Select(f => f.Id!.Value)
            .ToHashSet();

        List<TemplateFieldEntity> fieldsToRemove = template.Fields
            .Where(f => !incomingIds.Contains(f.Id))
            .ToList();

        foreach (TemplateFieldEntity fieldToRemove in fieldsToRemove)
        {
            template.Fields.Remove(fieldToRemove);
        }

        foreach (TemplateFieldToCreateOrUpdateDto field in request.Fields)
        {
            if (field.Id.HasValue)
            {
                TemplateFieldEntity existingField = template.Fields.FirstOrDefault(x => x.Id == field.Id.Value);
                if (existingField != null)
                {
                    existingField.Name = field.Name;
                    existingField.FieldType = field.FieldType;
                    existingField.IsRequired = field.IsRequired;
                    existingField.Order = field.Order;
                    existingField.Settings = field.Settings;
                }
            }
            else
            {
                template.Fields.Add(new TemplateFieldEntity
                {
                    Name = field.Name,
                    FieldType = field.FieldType,
                    IsRequired = field.IsRequired,
                    Order = field.Order,
                    Settings = field.Settings
                });
            }
        }
        
        await unitOfWork.SaveChangesAsync();
        
        return template;
    }
}
public class UpdateTemplateCommandValidator
    : AbstractValidator<UpdateTemplateCommand>
{
    public UpdateTemplateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Не указан идентификатор шаблона.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Название шаблона обязательно.")
            .MaximumLength(100)
            .WithMessage("Название шаблона не должно превышать 100 символов.")
            .Must(name => name == name.Trim())
            .WithMessage("Название шаблона не должно содержать пробелы в начале или конце.");

        RuleFor(x => x.Fields)
            .NotNull()
            .WithMessage("Список полей не может быть null.")
            .NotEmpty()
            .WithMessage("Шаблон должен содержать хотя бы одно поле.")
            .Must(fields => fields.Count <= 30)
            .WithMessage("Шаблон не может содержать более 30 полей.");

        RuleForEach(x => x.Fields)
            .SetValidator(new TemplateFieldToCreateOrUpdateDtoValidator());

        RuleFor(x => x.Fields)
            .Must(HaveUniqueOrders)
            .WithMessage("Порядок полей не должен повторяться.");

        RuleFor(x => x.Fields)
            .Must(HaveUniqueNames)
            .WithMessage("Названия полей не должны повторяться.");
    }

    private static bool HaveUniqueOrders(
        List<TemplateFieldToCreateOrUpdateDto> fields)
    {
        return fields == null ||
               fields.Select(x => x.Order)
                   .Distinct()
                   .Count() == fields.Count;
    }

    private static bool HaveUniqueNames(
        List<TemplateFieldToCreateOrUpdateDto> fields)
    {
        return fields == null ||
               fields.Select(x => x.Name?.Trim())
                   .Distinct(StringComparer.OrdinalIgnoreCase)
                   .Count() == fields.Count;
    }
}