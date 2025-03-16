using System.Net;
using System.Net.Http.Json;
using Pantry.Module.Recipe.IntegrationTests.Fakers;
using Pantry.Module.Recipe.IntegrationTests.Infrastructure;
using Shouldly;

namespace Pantry.Module.Recipe.IntegrationTests.Endpoints.RecipeDetailsEndpoints;

public class GetRecipeDetailsTests(RecipeIntegrationTestWebAppFactory factory)
    : RecipeBaseApiIntegrationTest(factory)
{
    [Fact]
    public async Task GetRecipe_Unauthorized()
    {
        var result = await HttpClientNonAuthenticated.GetAsync("/recipedetails/" + Guid.NewGuid());

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetRecipe_NotFound()
    {
        var result = await HttpClientAuthenticated.GetAsync("/recipedetails/" + Guid.NewGuid());

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

        var details = RecipeFaker.GenerateRecipeDetails();
        await AddRecipeDetailsAsync(recipeId, details);
        var result = await HttpClientAuthenticated.GetAsync("/recipedetails/" + recipeId);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var recipeResult = await result.Content.ReadFromJsonAsync<Shared.Models.RecipeModels.RecipeDetails>();
        recipeResult.ShouldNotBeNull();
        recipeResult.ShouldBeEquivalentTo(details);
    }
    
}