using Pantry.Module.Shared.Models.GoodModels;

namespace Pantry.Module.Recipe.Database.Entities;

public class IngredientEntity
{
    public required string Name { get; set; }

    public double CountOff { get; set; }

    public required UnitOfMeasurement Unit { get; set; }

    public Guid? PantryItemId { get; set; }
}