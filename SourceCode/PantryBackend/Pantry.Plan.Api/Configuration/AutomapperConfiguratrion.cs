using AutoMapper;
using Pantry.Plan.Api.Database.Entities;
using Pantry.Shared.Models.PlanModels;
using Pantry.Shared.Models.PlanModels.MealRequestModels;

namespace Pantry.Plan.Api.Configuration;

public class AutomapperConfiguratrion : Profile
{
    public AutomapperConfiguratrion()
    {
        CreateMap<MealEntity, Meal>().ReverseMap();
        CreateMap<MealEntity, MealCreateDto>().ReverseMap();
        CreateMap<MealEntity, MealUpdateDto>().ReverseMap();
    }
}
