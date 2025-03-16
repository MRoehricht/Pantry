using System.Net;
using Pantry.Module.Recipe.IntegrationTests.Fakers;
using Pantry.Module.Recipe.IntegrationTests.Infrastructure;
using Shouldly;

namespace Pantry.Module.Recipe.IntegrationTests.Endpoints.RecipeEndpoints;

public class DeleteRecipeTests(RecipeIntegrationTestWebAppFactory factory)
    : RecipeBaseApiIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteRecipe_Unauthorized()
    {
        var result = await HttpClientNonAuthenticated.DeleteAsync("/recipes/" + Guid.NewGuid());

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteRecipe_NotFound()
    {
        var result = await HttpClientAuthenticated.DeleteAsync("/recipes/" + Guid.NewGuid());

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteRecipe_Ok()
    {
        var recipeId = await CreateRecipeAsync();
        var result = await HttpClientAuthenticated.DeleteAsync("/recipes/" + recipeId);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
    
}