using System.Net;
using Pantry.Module.Recipe.IntegrationTests.Fakers;
using Pantry.Module.Recipe.IntegrationTests.Infrastructure;
using Shouldly;

namespace Pantry.Module.Recipe.IntegrationTests.Endpoints.IngredientEndpoints;

public class DeleteIngredientTests(RecipeIntegrationTestWebAppFactory factory)
    : RecipeBaseApiIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteIngredient_Unauthorized()
    {
        var result = await HttpClientNonAuthenticated.DeleteAsync("/ingredients/" + Guid.NewGuid() + "/Name");

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteIngredient_NotFound()
    {
        var result = await HttpClientAuthenticated.DeleteAsync("/ingredients/" + Guid.NewGuid()+ "/Name");

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteIngredient_Ok()
    {
        var ingredient = RecipeFaker.GenerateIngredient();

        var recipeId = await CreateIngredientAsync(ingredient);
        var result = await HttpClientAuthenticated.DeleteAsync("/ingredients/" + recipeId + "/" + ingredient.Name);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}