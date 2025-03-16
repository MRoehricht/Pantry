using System.Net;
using System.Net.Http.Json;
using Pantry.Module.Recipe.IntegrationTests.Infrastructure;
using Shouldly;

namespace Pantry.Module.Recipe.IntegrationTests.Endpoints.RecipeEndpoints;

public class GetRecipeTests(RecipeIntegrationTestWebAppFactory factory)
    : RecipeBaseApiIntegrationTest(factory)
{
    [Fact]
    public async Task GetRecipe_Unauthorized()
    {
        var result = await HttpClientNonAuthenticated.GetAsync("/recipes/" + Guid.NewGuid());

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetRecipe_NotFound()
    {
        var result = await HttpClientAuthenticated.GetAsync("/recipes/" + Guid.NewGuid());

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetRecipe_Ok()
    {
        for (var i = 0; i < 10; i++)
        {
            await CreateRecipeAsync();
        }

        var recipeId = await CreateRecipeAsync();
        var recipe = await GetRecipe(recipeId);
        var result = await HttpClientAuthenticated.GetAsync("/recipes/" + recipeId);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var recipeResult = await result.Content.ReadFromJsonAsync<Shared.Models.RecipeModels.Recipe>();
       recipeResult.ShouldNotBeNull();

        recipeResult.Id.ShouldBe(recipe.Id);
        recipeResult.Name.ShouldBe(recipe.Name);
        recipeResult.Description.ShouldBe(recipe.Description);
    }
}