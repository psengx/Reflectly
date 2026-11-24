using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class TemplateRepository(ApplicationContext context) : ITemplateRepository
{
    public async Task<TemplateEntity> CreateTemplateAsync(TemplateEntity template)
    {
        await context.Templates.AddAsync(template);
        return template;
    }

    public async Task<List<TemplateEntity>> GetAllTemplatesAsync()
    {
        List<TemplateEntity> templates = await context.Templates.ToListAsync();
        return templates;
    }
}