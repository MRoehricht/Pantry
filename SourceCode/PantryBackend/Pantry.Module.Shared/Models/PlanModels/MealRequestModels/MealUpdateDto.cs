namespace Pantry.Module.Shared.Models.PlanModels.MealRequestModels;
public class MealUpdateDto : MealCreateDto
{
    public Guid Id { get; set; }
    public bool WasCooked { get; set; }
}
