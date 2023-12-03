namespace Pantry.Recipe.Api.Database.Entities;

internal class IngredientEntity
{
    public required string Name { get; set; }

    public double CountOff { get; set; }

    public required string Unit { get; set; }

    public Guid? PantryItemId { get; set; }
}
