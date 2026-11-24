using Domain.Models;

namespace Domain.Entities;

public class TemplateFieldEntity
{
    public Guid Id { get; set; }
    public Guid TemplateId { get; set; }
    public TemplateEntity Template { get; set; }
    public string Name { get; set; }
    public FieldType FieldType { get; set; }
    public bool IsRequired { get; set; }
    public int Order { get; set; }
    public string? Settings { get; set; }
}