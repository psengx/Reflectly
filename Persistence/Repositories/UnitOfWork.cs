using Application.Interfaces.Repositories;

namespace Persistence.Repositories;

public class UnitOfWork(ApplicationContext context) : IUnitOfWork
{
    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}