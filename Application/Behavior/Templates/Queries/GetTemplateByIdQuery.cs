using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Behavior.Templates.Queries;

public class GetTemplateByIdQuery : IRequest<TemplateEntity?>
{
    public Guid Id { get; set; }
}

public class GetTemplateByIdQueryHandler(ITemplateRepository templateRepository)
    : IRequestHandler<GetTemplateByIdQuery, TemplateEntity?>
{
    public async Task<TemplateEntity?> Handle(GetTemplateByIdQuery query, CancellationToken cancellationToken)
    {
        return await templateRepository.GetTemplateByIdAsync(query.Id);
    }
}