
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Pantry.Plan.Api.Configuration;
using Pantry.Plan.Api.Database.Contexts;
using Pantry.Plan.Api.Endpoints;
using Pantry.Plan.Api.Services.RabbitMqConsumerServices;
using Pantry.Services.RabbitMqServices;
using Pantry.Services.RabbitMqServices.DependencyInjection;

namespace Pantry.Plan.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
            options.MapType<DateOnly>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "date",
                Example = new OpenApiString("2023-10-31")
            }));
        builder.Services.AddAutoMapper(typeof(AutomapperConfiguratrion));

        builder.Services.AddDbContext<PlanContext>(optionsAction =>
        {
            var postgresHost = builder.Configuration["DB_HOST"];
            var postgresPort = builder.Configuration["DB_PORT"];
            var postgresDatabase = builder.Configuration["DB_DB"];
            var postgresUser = builder.Configuration["DB_USER"];
            var postgresPassword = builder.Configuration["DB_PASSWORD"];
            optionsAction.UseNpgsql($"host={postgresHost};port={postgresPort};database={postgresDatabase};username={postgresUser};password={postgresPassword};");
        });

        
        builder.Services.AddRabbitMqServices(builder.Configuration);
        builder.Services.AddTransient<IRabbitMqConsumerService, DeleteRecipeConsumerService>();
        builder.Services.AddHostedService<RabbitMqConsumerBackgroundService>();

        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            //dotnet ef migrations add ...-Script // add-migration ...
            var recipeContext = scope.ServiceProvider.GetRequiredService<PlanContext>();
            recipeContext.Database.Migrate();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapGroup("/meals").MapMealsEndpoint();

        app.Run();
    }
}
