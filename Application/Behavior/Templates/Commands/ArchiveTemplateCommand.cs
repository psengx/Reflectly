using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Behavior.Templates.Commands;

public class ArchiveTemplateCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class ArchiveTemplateCommandHandler(
    ITemplateRepository templateRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ArchiveTemplateCommand, bool>
{
    public async Task<bool> Handle(ArchiveTemplateCommand request, CancellationToken cancellationToken)
    {
        TemplateEntity? template = await templateRepository.GetTemplateByIdAsync(request.Id);
        if (template == null)
            return false;

        template.IsArchived = true;
        await unitOfWork.SaveChangesAsync();
        return true;
    }
}