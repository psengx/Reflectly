using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Persistence;

public static class ServiceExtension
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(options =>
            options.UseNpgsql(configuration.GetSection("ConnectionStrings").GetSection("PostgreSql").Value)
        );
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITemplateRepository, TemplateRepository>();
        return services;
    }

    public static async Task RunMigration(this IServiceProvider provider)
    {
        using IServiceScope scope = provider.CreateScope();
        ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        await context.Database.MigrateAsync();
        
    }
}