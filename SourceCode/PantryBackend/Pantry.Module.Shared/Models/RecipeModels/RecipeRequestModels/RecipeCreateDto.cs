namespace Pantry.Module.Shared.Models.RecipeModels.RecipeRequestModels;
public class RecipeCreateDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
