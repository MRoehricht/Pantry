namespace Pantry.Module.Plan.Database.Entities;

public class MealEntity
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public DateOnly Date { get; set; }
    public bool WasCooked { get; set; }
    public string Owner { get; set; }
}
