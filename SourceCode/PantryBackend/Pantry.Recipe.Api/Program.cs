
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Pantry.Recipe.Api.Database.Contexts;
using Pantry.Recipe.Api.Endpoints;
using Pantry.Services.UserServices;

namespace Pantry.Recipe.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(conf =>
        {
            conf.AddSecurityDefinition("UserEMail", new OpenApiSecurityScheme
            {
                Description = "EMail of the user",
                Name = "UserEMail",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "UserEMailScheme",
            });
            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "UserEMail"
                },
                In = ParameterLocation.Header,
            };
            var requirement = new OpenApiSecurityRequirement
            {
                { scheme, Array.Empty<string>() }
            };
            conf.AddSecurityRequirement(requirement);
        });

        builder.Services.AddDbContext<RecipeContext>(optionsAction =>
        {
            var postgresHost = builder.Configuration["DB_HOST"];
            var postgresPort = builder.Configuration["DB_PORT"];
            var postgresDatabase = builder.Configuration["DB_DB"];
            var postgresUser = builder.Configuration["DB_USER"];
            var postgresPassword = builder.Configuration["DB_PASSWORD"];
            optionsAction.UseNpgsql($"host={postgresHost};port={postgresPort};database={postgresDatabase};username={postgresUser};password={postgresPassword};");
        });

        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.SetInMemorySagaRepositoryProvider();

            var assembly = typeof(Program).Assembly;

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

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<IHeaderEMailService, HeaderEMailService>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            //dotnet ef migrations add ...-Script // add-migration ...
            var recipeContext = scope.ServiceProvider.GetRequiredService<RecipeContext>();
            recipeContext.Database.Migrate();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        app.MapGroup("/recipes").MapRecipesEndpoint();
        app.MapGroup("/ingredients").MapIngredientsEndpoint();
        app.MapGroup("/recipedetails").MapRecipeDetailsEndpoint();

        app.Run();
    }
}
