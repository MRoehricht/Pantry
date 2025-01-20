using Pantry.Module.Shared.Models.GoodModels;

namespace Pantry.Module.Shared.Models.RecipeModels;
public class Ingredient
{
    public required string Name { get; set; }

    public double CountOff { get; set; }

    public UnitOfMeasurement Unit { get; set; }

    public Guid? PantryItemId { get; set; }
}
