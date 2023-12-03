namespace Pantry.Recipe.Api.Database.Entities;

internal class RecipeEntity : EntityBase
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<IngredientEntity> Ingredients { get; set; } = new();
    public required RecipeDetailsEntity Details { get; set; } = new();

    public Guid OwnerId { get; set; }
    public DateTime CreatedOn { get; set; }
}
