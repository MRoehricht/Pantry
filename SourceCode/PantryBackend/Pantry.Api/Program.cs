
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pantry.Api.Database.Contexts;
using Pantry.Api.Endpoints;
using Pantry.Recipe.Api.Configuration;
using Pantry.Services.RabbitMqServices;
using Pantry.Services.RabbitMqServices.DependencyInjection;

namespace Pantry.Api;

public class Program
{
    const string PANTRY_ORIGINS = "pantry";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<PantryContext>(optionsAction =>
        {
            var postgresHost = builder.Configuration["DB_HOST"];
            var postgresPort = builder.Configuration["DB_PORT"];
            var postgresDatabase = builder.Configuration["DB_DB"];
            var postgresUser = builder.Configuration["DB_USER"];
            var postgresPassword = builder.Configuration["DB_PASSWORD"];
            optionsAction.UseNpgsql($"host={postgresHost};port={postgresPort};database={postgresDatabase};username={postgresUser};password={postgresPassword};");
        });

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddRabbitMqServices(builder.Configuration);
        builder.Services.AddAutoMapper(typeof(AutomapperConfiguratrion));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHostedService<RabbitMqConsumerBackgroundService>();

        
        var allowedOrigins = builder.Configuration["ALLOWED_ORIGINS"]?.Split(',') ?? Array.Empty<string>();
        builder.Services.AddCors(opt => {
            opt.AddPolicy(name: PANTRY_ORIGINS, policyBuilder => {
                policyBuilder.WithOrigins(allowedOrigins)
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            //dotnet ef migrations add ...-Script // add-migration ...
            var recipeContext = scope.ServiceProvider.GetRequiredService<PantryContext>();
            recipeContext.Database.Migrate();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors(PANTRY_ORIGINS);
        app.UseAuthorization();


        app.MapGroup("/goods").MapGoodsEndpoint();
        app.Run();
    }
}
