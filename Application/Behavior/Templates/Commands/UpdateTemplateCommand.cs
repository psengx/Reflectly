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
        template.Fields.Clear();
        
        foreach (TemplateFieldToCreateOrUpdateDto field in request.Fields)
        {
            TemplateFieldEntity fieldEntity = new()
            {
                Id = field.Id ?? Guid.NewGuid(),
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