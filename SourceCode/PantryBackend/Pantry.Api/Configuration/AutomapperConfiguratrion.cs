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
    }
}
