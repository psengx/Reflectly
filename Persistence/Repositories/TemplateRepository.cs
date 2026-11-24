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
        return await context.Templates
            .Include(t => t.Fields)
            .ToListAsync();
    }

    public async Task<TemplateEntity?> GetTemplateByIdAsync(Guid id)
    {
        return await context.Templates
            .Include(t => t.Fields)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}