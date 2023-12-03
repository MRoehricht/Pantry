namespace Pantry.Shared.Models.RecipeModels;

public class Recipe
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<Ingredient> Ingredients { get; set; } = new();
    public RecipeDetails Details { get; set; } = new();
}

