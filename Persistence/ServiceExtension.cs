using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence;

public static class ServiceExtension
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(options =>
            options.UseNpgsql(configuration.GetSection("ConnectionStrings").GetSection("PostgreSql").Value)
        );

        return services;
    }

    public static async Task RunMigration(this IServiceProvider provider)
    {
        using IServiceScope scope = provider.CreateScope();
        ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        await context.Database.MigrateAsync();
        
    }
}