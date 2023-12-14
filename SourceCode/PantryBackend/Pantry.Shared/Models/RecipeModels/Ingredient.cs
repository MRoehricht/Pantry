namespace Pantry.Shared.Models.RecipeModels;
public class Ingredient
{
    public required string Name { get; set; }

    public double CountOff { get; set; }

    public string Unit { get; set; }

    public Guid? PantryItemId { get; set; }
}
