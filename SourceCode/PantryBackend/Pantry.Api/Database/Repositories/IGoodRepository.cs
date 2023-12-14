using Pantry.Shared.Models.RecipeModels;

namespace Pantry.Api.Database.Repositories;

public interface IGoodRepository {
    Task Create( Ingredient ingredient);
}