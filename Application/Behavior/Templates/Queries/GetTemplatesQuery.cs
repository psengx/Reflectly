using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Behavior.Templates.Queries;

public class GetAllTemplatesQuery : IRequest<List<TemplateEntity>>;

public class GetAllTemplatesQueryHandler(ITemplateRepository templateRepository) : IRequestHandler<GetAllTemplatesQuery, List<TemplateEntity>>
{
    public async Task<List<TemplateEntity>> Handle(GetAllTemplatesQuery request, CancellationToken cancellationToken)
    {
        List<TemplateEntity> templates = await templateRepository.GetAllTemplatesAsync();
        return templates;
    }
}