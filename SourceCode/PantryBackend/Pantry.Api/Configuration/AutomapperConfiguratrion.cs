using AutoMapper;
using Pantry.Api.Database.Entities;
using Pantry.Shared.Models.GoodModels;

namespace Pantry.Recipe.Api.Configuration;

public class AutomapperConfiguratrion : Profile
{
    public AutomapperConfiguratrion()
    {
        CreateMap<GoodDetailsEntity, GoodDetails>().ReverseMap();
        CreateMap<GoodEntity, Good>().ReverseMap();
        CreateMap<PriceHistoryEntity, PriceHistory>().ReverseMap();
        CreateMap<GoodEntity, GoodsOverview>()
            .ForPath(dest => dest.Tags, act => act.MapFrom(src => src.Details.Tags))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom<AverageResolver>());
        CreateMap<GoodEntity, GoodRating>()
            .ForPath(dest => dest.Ratings, act => act.MapFrom(src => src.Details.Ratings))
            .ForPath(dest => dest.GoodId, act => act.MapFrom(src => src.Id));
        CreateMap<GoodEntity, GoodSuggestion>().ReverseMap();
    }
}

public class AverageResolver : IValueResolver<GoodEntity, GoodsOverview, double?> {
    public double? Resolve(GoodEntity source, GoodsOverview destination, double? destMember, ResolutionContext context) {
        if (source?.Details?.Ratings == null || !source.Details.Ratings.Any()) {
            return null;
        }

        return source.Details.Ratings.Average();
    }
}
