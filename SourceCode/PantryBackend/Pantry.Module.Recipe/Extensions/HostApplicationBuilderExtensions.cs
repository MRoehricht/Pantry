using System.Reflection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pantry.Module.Recipe.Database.Contexts;
using Pantry.Module.Shared.Database;

namespace Pantry.Module.Recipe.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddRecipeModule(this IHostApplicationBuilder builder,
        IList<Assembly> mediatrAssemblies)
    {
        mediatrAssemblies.Add(typeof(HostApplicationBuilderExtensions).Assembly);

        builder.Services.AddDbContext<RecipeContext>(optionsAction =>
        {
            optionsAction.UseNpgsql(DatabaseConfigurationManager.CreateDatabaseConfiguration(builder.Configuration).GetConnectionString());
        });
        
        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.SetInMemorySagaRepositoryProvider();

            var assembly = typeof(HostApplicationBuilderExtensions).Assembly;

            x.AddConsumers(assembly);
            x.AddSagaStateMachines(assembly);
            x.AddSagas(assembly);
            x.AddActivities(assembly);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
                {
                    h.Username(builder.Configuration["RabbitMQ:User"]);
                    h.Password(builder.Configuration["RabbitMQ:Password"]);
                });
                cfg.ConfigureEndpoints(context);
            });
        });
        
        return builder;
    }
}