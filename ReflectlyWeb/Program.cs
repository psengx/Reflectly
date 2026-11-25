using Application;
using Persistence;

namespace ReflectlyWeb;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddApplication();
        builder.Services.AddPersistence(builder.Configuration);
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
            });

        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.MapControllers();

        await app.Services.RunMigration();

        app.Run();
    }
}