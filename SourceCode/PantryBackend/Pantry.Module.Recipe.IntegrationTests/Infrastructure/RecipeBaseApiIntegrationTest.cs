using System.Net.Http.Json;
using Pantry.Module.Recipe.Configuration;
using Pantry.Module.Recipe.Database.Contexts;
using Pantry.Module.Recipe.Database.Entities;
using Pantry.Module.Recipe.IntegrationTests.Fakers;
using Pantry.Module.Shared.Models.RecipeModels;
using Pantry.Module.Shared.Models.RecipeModels.RecipeRequestModels;

namespace Pantry.Module.Recipe.IntegrationTests.Infrastructure;

public class RecipeBaseApiIntegrationTest: RecipeBaseIntegrationTest, IAsyncLifetime
{
    protected readonly HttpClient HttpClientNonAuthenticated;
    protected readonly HttpClient HttpClientAuthenticated;

    protected RecipeBaseApiIntegrationTest(RecipeIntegrationTestWebAppFactory factory) : base(factory)
    {
        
        HttpClientNonAuthenticated = factory.CreateClient();
        HttpClientAuthenticated = factory.CreateClient();
    }
    
    public Task InitializeAsync()
    {
        HttpClientAuthenticated.DefaultRequestHeaders.Add("UserEMail", "me@me.de");
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        HttpClientNonAuthenticated.Dispose();
        HttpClientAuthenticated.Dispose();
        return Task.CompletedTask;
    }
    
    protected async Task<Guid> CreateIngredientAsync(Ingredient ingredient)
    {
        Guid? recipeId = null;
        var recipeCreateDto = RecipeFaker.GenerateRecipe();
        
        await HttpClientAuthenticated.PostAsJsonAsync("/recipes", recipeCreateDto)
            .ContinueWith(async task =>
            {
                var response = await task;
                response.EnsureSuccessStatusCode();
                var recipe = await response.Content.ReadFromJsonAsync<Shared.Models.RecipeModels.Recipe>();
                recipeId = recipe?.Id;
            });
        
        if(!recipeId.HasValue) throw new Exception("RecipeId is null");
        
        await HttpClientAuthenticated.PostAsJsonAsync("/ingredients/" + recipeId.Value, ingredient);
        
        return recipeId.Value;
    }
    
    protected async Task<Guid> CreateRecipeAsync()
    {
        Guid? recipeId = null;
        var recipeCreateDto = RecipeFaker.GenerateRecipe();
        
        await HttpClientAuthenticated.PostAsJsonAsync("/recipes", recipeCreateDto)
            .ContinueWith(async task =>
            {
                var response = await task;
                response.EnsureSuccessStatusCode();
                var recipe = await response.Content.ReadFromJsonAsync<Shared.Models.RecipeModels.Recipe>();
                recipeId = recipe?.Id;
            });
        
        if(!recipeId.HasValue) throw new Exception("RecipeId is null");
        
        return recipeId.Value;
    }

    protected async Task<Ingredient> GetIngredient(Guid recipeId, string name)
    {
        var mapper = new RecipeMapper();
        var recipe = await RecipeContext.Recipes.FindAsync(recipeId) ?? throw new Exception("Recipe is null");
        
        var ingredient = recipe.Ingredients.FirstOrDefault(i => i.Name == name) ?? throw new Exception("Ingredient is null");
        return mapper.MapToIngredient(ingredient);
    }
    
    protected async Task<Shared.Models.RecipeModels.Recipe> GetRecipe(Guid recipeId)
    {
        var mapper = new RecipeMapper();
        var recipe = await RecipeContext.Recipes.FindAsync(recipeId) ?? throw new Exception("Recipe is null");

        return mapper.MapToRecipe(recipe);
    }
    
    protected async Task AddRecipeDetailsAsync(Guid recipeId, RecipeDetails details)
    {
        var mapper = new RecipeMapper();
        var recipe = await RecipeContext.Recipes.FindAsync(recipeId) ?? throw new Exception("Recipe is null");

        recipe.Details = new RecipeDetailsEntity()
        {
            Reviews = details.Reviews,
            Tags = details.Tags,
            CookedOn = details.CookedOn
        };

        await RecipeContext.SaveChangesAsync();
    }
}