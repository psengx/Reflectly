using Application.DTO;

namespace ReflectlyWeb.DTO.Template;

public class TemplateToCreateOrUpdateDto
{
    public string Name { get; set; }
    public List<TemplateFieldToCreateOrUpdateDto> Fields { get; set; }
}