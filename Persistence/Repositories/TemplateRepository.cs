using Application.Interfaces.Repositories;
using Domain.Entities;

namespace Persistence.Repositories;

public class TemplateRepository(ApplicationContext context) : ITemplateRepository
{
    public async Task<TemplateEntity> CreateTemplateAsync(TemplateEntity template)
    {
        await context.Templates.AddAsync(template);
        return template;
    }
}