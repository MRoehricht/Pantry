using System.Net;
using System.Net.Http.Json;
using Pantry.Module.Recipe.IntegrationTests.Fakers;
using Pantry.Module.Recipe.IntegrationTests.Infrastructure;
using Pantry.Module.Shared.Models.RecipeModels;
using Shouldly;

namespace Pantry.Module.Recipe.IntegrationTests.Endpoints.IngredientEndpoints;

public class GetIngredientsTests(RecipeIntegrationTestWebAppFactory factory)
    : RecipeBaseApiIntegrationTest(factory)
{
    [Fact]
    public async Task GetIngredients_Unauthorized()
    {
        var result = await HttpClientNonAuthenticated.GetAsync("/ingredients/" + Guid.NewGuid());

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetIngredients_NotFound()
    {
        var result = await HttpClientAuthenticated.GetAsync("/ingredients/" + Guid.NewGuid());

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetIngredients_Ok()
    {
        var ingredient = RecipeFaker.GenerateIngredient();

        var recipeId = await CreateIngredientAsync(ingredient);
        var result = await HttpClientAuthenticated.GetAsync("/ingredients/" + recipeId);

        var ingredientResults = await result.Content.ReadFromJsonAsync<Ingredient[]>();

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        ingredientResults.ShouldNotBeNull();
        ingredientResults.Length.ShouldBe(1);

        var ingredientResult = ingredientResults.First();

        ingredientResult.Name.ShouldBe(ingredient.Name);
        ingredientResult.CountOff.ShouldBe(ingredient.CountOff);
        ingredientResult.Unit.ShouldBe(ingredient.Unit);
        ingredientResult.PantryItemId.ShouldBe(ingredient.PantryItemId);
    }
}