
using Microsoft.EntityFrameworkCore;
using Pantry.Recipe.Api.Configuration;
using Pantry.Recipe.Api.Database.Contexts;
using Pantry.Recipe.Api.Endpoints;

namespace Pantry.Recipe.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAutoMapper(typeof(AutomapperConfiguratrion));



        builder.Services.AddDbContext<RecipeContext>(optionsAction =>
        {
            var postgresHost = builder.Configuration["DB_HOST"];
            var postgresPort = builder.Configuration["DB_PORT"];
            var postgresDatabase = builder.Configuration["DB_DB"];
            var postgresUser = builder.Configuration["DB_USER"];
            var postgresPassword = builder.Configuration["DB_PASSWORD"];
            optionsAction.UseNpgsql($"host={postgresHost};port={postgresPort};database={postgresDatabase};username={postgresUser};password={postgresPassword};");
        });

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
