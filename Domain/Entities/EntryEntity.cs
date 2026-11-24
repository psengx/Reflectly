namespace Domain.Entities;

public class EntryEntity
{
    public Guid Id { get; set; }
    public Guid TemplateId { get; set; }
    public TemplateEntity Template { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Answers { get; set; }
    public string FieldDefinitionSnapshot { get; set; }
}