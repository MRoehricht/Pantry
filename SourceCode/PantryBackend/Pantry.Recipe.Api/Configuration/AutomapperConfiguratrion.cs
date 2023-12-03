using AutoMapper;
using Pantry.Recipe.Api.Database.Entities;
using Pantry.Shared.Models.RecipeModels;
using Pantry.Shared.Models.RecipeModels.RecipeRequestModels;

namespace Pantry.Recipe.Api.Configuration;

public class AutomapperConfiguratrion : Profile
{
    public AutomapperConfiguratrion()
    {
        CreateMap<Shared.Models.RecipeModels.Recipe, RecipeEntity>().ReverseMap();
        CreateMap<RecipeCreateDto, RecipeEntity>().ReverseMap();
        CreateMap<Ingredient, IngredientEntity>().ReverseMap();
        CreateMap<RecipeDetails, RecipeDetailsEntity>().ReverseMap();
    }
}
