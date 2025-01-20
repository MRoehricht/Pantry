using Pantry.Module.Recipe.Database.Entities;
using Pantry.Module.Shared.Models.RecipeModels;
using Pantry.Module.Shared.Models.RecipeModels.RecipeRequestModels;
using Riok.Mapperly.Abstractions;

namespace Pantry.Module.Recipe.Configuration;

[Mapper]
public partial class RecipeMapper
{
    public partial Shared.Models.RecipeModels.Recipe MapToRecipe(RecipeEntity recipeEntity);
    public partial RecipeEntity MapToRecipeEntity(RecipeCreateDto recipeEntity);
    public partial IEnumerable<Shared.Models.RecipeModels.Recipe> MapToRecipes(List<RecipeEntity> recipeEntity);
    public partial Ingredient MapToIngredient(IngredientEntity source);
    public partial List<Ingredient> MapToIngredients(List<IngredientEntity> source);
    public partial IngredientEntity MapToIngredientEntity(Ingredient source);
    public partial RecipeDetails MapToRecipeDetails(RecipeDetailsEntity source);

    public RecipeOverview MapToRecipeOverview(RecipeEntity source)
    {
        return new RecipeOverview
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            Tags = source.Details.Tags,
            Rating = source.Details.Reviews.Any() ? source.Details.Reviews.Average() : null
        };
    }

    public IEnumerable<RecipeOverview> MapToRecipeOverviews(List<RecipeEntity> source)
    {
        return source.Select(x => MapToRecipeOverview(x));
    }
}