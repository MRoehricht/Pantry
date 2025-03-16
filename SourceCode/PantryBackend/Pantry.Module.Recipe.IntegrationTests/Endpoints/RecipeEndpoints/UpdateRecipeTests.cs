using System.Net;
using System.Net.Http.Json;
using Pantry.Module.Recipe.IntegrationTests.Fakers;
using Pantry.Module.Recipe.IntegrationTests.Infrastructure;
using Shouldly;

namespace Pantry.Module.Recipe.IntegrationTests.Endpoints.RecipeEndpoints;

public class UpdateRecipeTests(RecipeIntegrationTestWebAppFactory factory)
    : RecipeBaseApiIntegrationTest(factory)
{
    [Fact]
    public async Task UpdateRecipe_Unauthorized()
    {
        var result = await HttpClientNonAuthenticated.PutAsJsonAsync("/recipes", RecipeFaker.GenerateRecipeUpdate(Guid.NewGuid()));

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateRecipe_NotFound()
    {
        var result = await HttpClientAuthenticated.PutAsJsonAsync("/recipes" , RecipeFaker.GenerateRecipeUpdate(Guid.NewGuid()));

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdateRecipe_Ok()
    {
        var recipeId = await CreateRecipeAsync();
        var update = RecipeFaker.GenerateRecipeUpdate(recipeId);
        var result = await HttpClientAuthenticated.PutAsJsonAsync("/recipes" , update);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        
        var recipeResult = await GetRecipe(recipeId);
        recipeResult.ShouldNotBeNull(); 
        recipeResult.Name.ShouldBe(update.Name);
        recipeResult.Description.ShouldBe(update.Description);
    }
}