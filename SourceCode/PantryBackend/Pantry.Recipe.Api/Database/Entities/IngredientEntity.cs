using Pantry.Shared.Models.GoodModels;

namespace Pantry.Recipe.Api.Database.Entities;

internal class IngredientEntity
{
    public required string Name { get; set; }

    public double CountOff { get; set; }

    public required UnitOfMeasurement Unit { get; set; }

    public Guid? PantryItemId { get; set; }
}
