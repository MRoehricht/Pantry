using System.Net;
using System.Net.Http.Json;
using Pantry.Module.Recipe.IntegrationTests.Fakers;
using Pantry.Module.Recipe.IntegrationTests.Infrastructure;
using Pantry.Module.Shared.Models.RecipeModels;
using Shouldly;

namespace Pantry.Module.Recipe.IntegrationTests.Endpoints.IngredientEndpoints;

public class GetIngredientTests(RecipeIntegrationTestWebAppFactory factory)
    : RecipeBaseApiIntegrationTest(factory)
{
    [Fact]
    public async Task GetIngredient_Unauthorized()
    {
        var result = await HttpClientNonAuthenticated.GetAsync("/ingredients/" + Guid.NewGuid() + "/Name");

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetIngredient_NotFound()
    {
        var result = await HttpClientAuthenticated.GetAsync("/ingredients/" + Guid.NewGuid() + "/Name");

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetIngredient_Ok()
    {
        var ingredient = RecipeFaker.GenerateIngredient();
        var recipeId = await CreateIngredientAsync(ingredient);

        var result = await HttpClientAuthenticated.GetAsync("/ingredients/" + recipeId + "/" + ingredient.Name);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var ingredientResult = await result.Content.ReadFromJsonAsync<Ingredient>();

        ingredientResult.ShouldNotBeNull();
        ingredientResult.Name.ShouldBe(ingredient.Name);
        ingredientResult.CountOff.ShouldBe(ingredient.CountOff);
        ingredientResult.Unit.ShouldBe(ingredient.Unit);
        ingredientResult.PantryItemId.ShouldBe(ingredient.PantryItemId);
    }
}