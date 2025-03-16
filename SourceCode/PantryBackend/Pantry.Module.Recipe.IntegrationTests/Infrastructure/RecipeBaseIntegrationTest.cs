using Microsoft.Extensions.DependencyInjection;
using Pantry.Module.Recipe.Database.Contexts;

namespace Pantry.Module.Recipe.IntegrationTests.Infrastructure;

[Collection(nameof(RecipeTestCollection))]
public class RecipeBaseIntegrationTest
{
    protected RecipeBaseIntegrationTest(RecipeIntegrationTestWebAppFactory factory)
    {
        var scope = factory.Services.CreateScope();
        RecipeContext = scope.ServiceProvider.GetRequiredService<RecipeContext>();
    }

    public RecipeContext RecipeContext { get; set; }
}