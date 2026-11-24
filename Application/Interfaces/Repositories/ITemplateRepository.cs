using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ITemplateRepository
{
    Task<TemplateEntity> CreateTemplateAsync(TemplateEntity template);
}