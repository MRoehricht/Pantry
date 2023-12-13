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
        CreateMap<RecipeEntity, RecipeOverview>()
            .ForPath(dest => dest.Tags, act => act.MapFrom(src => src.Details.Tags))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom<AverageResolver>());




    }
}

internal class AverageResolver : IValueResolver<RecipeEntity, RecipeOverview, double?> {
    public double? Resolve(RecipeEntity source, RecipeOverview destination, double? destMember, ResolutionContext context) {
        if (source?.Details?.Reviews == null || !source.Details.Reviews.Any()) {
            return null;
        }

        return source.Details.Reviews.Average();
    }
}
