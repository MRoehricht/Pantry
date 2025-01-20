namespace Pantry.Module.Recipe.Database.Entities;

public class RecipeEntity : EntityBase
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<IngredientEntity> Ingredients { get; set; } = new();
    public RecipeDetailsEntity Details { get; set; } = new();
    public string Owner { get; set; }
    public DateTime CreatedOn { get; set; }
}
