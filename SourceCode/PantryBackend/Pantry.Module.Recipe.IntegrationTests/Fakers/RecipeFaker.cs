using System.Net.Http.Json;
using Bogus;
using Pantry.Module.Shared.Models.GoodModels;
using Pantry.Module.Shared.Models.RecipeModels;
using Pantry.Module.Shared.Models.RecipeModels.RecipeRequestModels;

namespace Pantry.Module.Recipe.IntegrationTests.Fakers;

public static class RecipeFaker 
{
    public static Ingredient GenerateIngredient()
    {
        var fruit = new[] { "apple", "banana", "orange", "strawberry", "kiwi" };
        
        var ingredientFaker = new Faker<Ingredient>("de")
            .RuleFor(d => d.Name, f => f.PickRandom(fruit))
            .RuleFor(d => d.CountOff,f => f.Random.Double(0, 100))
            .RuleFor(d => d.Unit,f => f.PickRandom<UnitOfMeasurement>())
            .RuleFor(d => d.PantryItemId,f => Guid.NewGuid());
        return ingredientFaker.Generate();
    }
    
    public static RecipeCreateDto GenerateRecipe()
    {
        var recipeFaker = new Faker<RecipeCreateDto>("de")
            .RuleFor(d => d.Name, f => f.Commerce.ProductName())
            .RuleFor(d => d.Description, f => f.Lorem.Sentence());
        return recipeFaker.Generate();
    }
    
    public static RecipeUpdateDto GenerateRecipeUpdate(Guid id)
    {
        var recipeFaker = new Faker<RecipeUpdateDto>("de")
            .RuleFor(d => d.Id, f => id)
            .RuleFor(d => d.Name, f => f.Commerce.ProductName())
            .RuleFor(d => d.Description, f => f.Lorem.Sentence());
        return recipeFaker.Generate();
    }
    
    public static RecipeDetails GenerateRecipeDetails()
    {
        var recipeFaker = new Faker<RecipeDetails>("de")
            .RuleFor(d => d.Tags, f => f.Make(5, () => f.Commerce.ProductName()))
            .RuleFor(d => d.CookedOn, f => f.Make(10, () => f.Date.PastDateOnly()))
            .RuleFor(d => d.Reviews, f=> f.Make(15, () => f.Random.Int(1, 5)));
        return recipeFaker.Generate();
    }
}