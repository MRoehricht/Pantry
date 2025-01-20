namespace Pantry.Module.Recipe.Database.Entities;

public class RecipeDetailsEntity
{
    public List<int> Reviews { get; set; } = new();
    public List<DateOnly> CookedOn { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}
