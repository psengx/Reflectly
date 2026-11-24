namespace Domain.Entities;

public class TemplateEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsArchived { get; set; }
    public ICollection<TemplateFieldEntity> Fields { get; set; }
        = new List<TemplateFieldEntity>();
}