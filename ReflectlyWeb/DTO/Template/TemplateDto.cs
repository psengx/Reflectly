using Application.DTO;

namespace ReflectlyWeb.DTO.Template;

public class TemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsArchived { get; set; }
    public List<TemplateFieldToCreateOrUpdateDto> Fields { get; set; }
}