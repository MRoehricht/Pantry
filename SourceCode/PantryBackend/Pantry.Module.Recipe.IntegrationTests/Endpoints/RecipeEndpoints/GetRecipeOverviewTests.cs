using System.Net;
using System.Net.Http.Json;
using Pantry.Module.Recipe.IntegrationTests.Infrastructure;
using Shouldly;

namespace Pantry.Module.Recipe.IntegrationTests.Endpoints.RecipeEndpoints;

public class GetRecipeOverviewTests(RecipeIntegrationTestWebAppFactory factory)
    : RecipeBaseApiIntegrationTest(factory)
{
    [Fact]
    public async Task GetRecipeOverview_Unauthorized()
    {
        var result = await HttpClientNonAuthenticated.GetAsync("/recipes/overview/");

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task GetRecipeOverview_Ok()
    {
        for (var i = 0; i < 10; i++)
        {
            await CreateRecipeAsync();
        }

        var recipeId = await CreateRecipeAsync();
        var recipe = await GetRecipe(recipeId);
        
        
        var result = await HttpClientAuthenticated.GetAsync("/recipes/overview/");

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        
        var recipes = await result.Content.ReadFromJsonAsync<List<Shared.Models.RecipeModels.RecipeOverview>>();
        
        recipes.ShouldNotBeNull();
        var recipeResult = recipes.First(r => r.Id == recipeId);
        recipeResult.ShouldNotBeNull();
        recipeResult.Id.ShouldBe(recipe.Id);
        recipeResult.Name.ShouldBe(recipe.Name);
        recipeResult.Description.ShouldBe(recipe.Description);
    }
}