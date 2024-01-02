using Pantry.Shared.Models.RecipeModels;

namespace Pantry.Shared.Models.MessageModes;
public class UpdateIngredientNameMessage
{
    public Ingredient Ingredient { get; set; }

    public string Owner { get; set; }
}
