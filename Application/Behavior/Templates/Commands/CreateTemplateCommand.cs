using Application.DTO;
using Application.Interfaces.Repositories;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Behavior.Templates.Commands;

public class CreateTemplateCommand : IRequest<Guid>
{
    public string Name { get; set; }
    public List<TemplateFieldToCreateOrUpdateDto> Fields { get; set; }
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
        
        await templateRepository.CreateTemplateAsync(template);
        await unitOfWork.SaveChangesAsync();
        
        return template.Id;
    }
}
public class CreateTemplateCommandValidator
    : AbstractValidator<CreateTemplateCommand>
{
    public CreateTemplateCommandValidator()
    {
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

public class TemplateFieldToCreateOrUpdateDtoValidator
    : AbstractValidator<TemplateFieldToCreateOrUpdateDto>
{
    public TemplateFieldToCreateOrUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Название поля обязательно.")
            .MaximumLength(100)
            .WithMessage("Название поля не должно превышать 100 символов.")
            .Must(name => name == name.Trim())
            .WithMessage("Название поля не должно содержать пробелы в начале или конце.");

        RuleFor(x => x.FieldType)
            .IsInEnum()
            .WithMessage("Указан недопустимый тип поля.");

        RuleFor(x => x.Order)
            .GreaterThan(0)
            .WithMessage("Порядок поля должен быть больше нуля.");

        // RuleFor(x => x.Settings)
         //   .Must(BeValidJson)
          //  .WithMessage("Настройки поля должны содержать корректный JSON.");
    }

    private static bool BeValidJson(string? settings)
    {
        if (string.IsNullOrWhiteSpace(settings))
            return true;

        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(settings);

            return document.RootElement.ValueKind == System.Text.Json.JsonValueKind.Object;
        }
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }
}