using System.Net;
using System.Net.Http.Json;
using Pantry.Module.Recipe.IntegrationTests.Fakers;
using Pantry.Module.Recipe.IntegrationTests.Infrastructure;
using Shouldly;

namespace Pantry.Module.Recipe.IntegrationTests.Endpoints.IngredientEndpoints;

public class CreateIngredientTests(RecipeIntegrationTestWebAppFactory factory)
    : RecipeBaseApiIntegrationTest(factory)
{
    [Fact]
    public async Task CreateIngredient_Unauthorized()
    {
        var result = await HttpClientNonAuthenticated.PostAsJsonAsync("/ingredients/" + Guid.NewGuid(), RecipeFaker.GenerateIngredient());

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateIngredient_NotFound()
    {
        var result = await HttpClientAuthenticated.PostAsJsonAsync("/ingredients/" + Guid.NewGuid(), RecipeFaker.GenerateIngredient());

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CreateIngredient_Ok()
    {
        var recipeId = await CreateRecipeAsync();
        
        var result = await HttpClientAuthenticated.PostAsJsonAsync("/ingredients/" + recipeId, RecipeFaker.GenerateIngredient());

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
    
}