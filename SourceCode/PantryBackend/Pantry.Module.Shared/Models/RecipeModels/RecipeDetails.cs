namespace Pantry.Module.Shared.Models.RecipeModels;
public class RecipeDetails
{
    public List<int> Reviews { get; set; } = new();
    public List<DateOnly> CookedOn { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}
