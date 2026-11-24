using Domain.Models;

namespace Application.DTO;

public class TemplateFieldToCreateOrUpdateDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public FieldType FieldType { get; set; }
    public bool IsRequired { get; set; }
    public int Order { get; set; }
    public string? Settings { get; set; }
}