using System.Net;
using System.Net.Http.Json;
using Pantry.Module.Recipe.IntegrationTests.Fakers;
using Pantry.Module.Recipe.IntegrationTests.Infrastructure;
using Pantry.Module.Shared.Models.GoodModels;
using Shouldly;

namespace Pantry.Module.Recipe.IntegrationTests.Endpoints.IngredientEndpoints;

public class UpdateIngredientTests(RecipeIntegrationTestWebAppFactory factory)
    : RecipeBaseApiIntegrationTest(factory)
{
    [Fact]
    public async Task UpdateIngredient_Unauthorized()
    {
        var result = await HttpClientNonAuthenticated.PutAsJsonAsync("/ingredients/" + Guid.NewGuid() + "/Name",  RecipeFaker.GenerateIngredient());

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateIngredient_NotFound()
    {
        var result = await HttpClientAuthenticated.PutAsJsonAsync("/ingredients/" + Guid.NewGuid()+ "/Name",  RecipeFaker.GenerateIngredient());

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdateIngredient_Ok()
    {
        var ingredient = RecipeFaker.GenerateIngredient();
        var recipeId = await CreateIngredientAsync(ingredient);
        var ingredientUpdate = ingredient;
        
        ingredientUpdate.CountOff = 2;
        ingredientUpdate.Unit = UnitOfMeasurement.Piece;
        
        var result = await HttpClientAuthenticated.PutAsJsonAsync("/ingredients/" + recipeId + "/" + ingredient.Name, ingredientUpdate);
        
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var ingredientResult = await GetIngredient(recipeId, ingredientUpdate.Name);
        
        ingredientResult.ShouldNotBeNull();
        ingredientResult.CountOff.ShouldBe(ingredientUpdate.CountOff);
        ingredientResult.Unit.ShouldBe(ingredientUpdate.Unit);
    }
}