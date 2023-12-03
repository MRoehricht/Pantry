namespace Pantry.Shared.Models.PlanModels;
public class Meal
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public DateOnly Date { get; set; }
    public bool WasCooked { get; set; }
}
