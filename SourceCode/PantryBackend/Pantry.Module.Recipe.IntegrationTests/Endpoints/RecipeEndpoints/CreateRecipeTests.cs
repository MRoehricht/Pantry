using System.Net;
using System.Net.Http.Json;
using Pantry.Module.Recipe.IntegrationTests.Fakers;
using Pantry.Module.Recipe.IntegrationTests.Infrastructure;
using Shouldly;

namespace Pantry.Module.Recipe.IntegrationTests.Endpoints.RecipeEndpoints;

public class CreateRecipeTests(RecipeIntegrationTestWebAppFactory factory)
    : RecipeBaseApiIntegrationTest(factory)
{
    [Fact]
    public async Task CreateRecipe_Unauthorized()
    {
        var result = await HttpClientNonAuthenticated.PostAsJsonAsync("/recipes", RecipeFaker.GenerateRecipe());

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateRecipe_Ok()
    {
        var createRecipe = RecipeFaker.GenerateRecipe();
        var result = await HttpClientAuthenticated.PostAsJsonAsync("/recipes" , createRecipe);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);
        
        result.Headers.Location.ShouldNotBeNull();
        
        var recipeId = result.Headers.Location?.ToString().Split("/").Last();
        recipeId.ShouldNotBeNull();
        
        var recipeResult = await GetRecipe(Guid.Parse(recipeId));
        
        recipeResult.Name.ShouldBe(createRecipe.Name);
        recipeResult.Description.ShouldBe(createRecipe.Description);
    }
}