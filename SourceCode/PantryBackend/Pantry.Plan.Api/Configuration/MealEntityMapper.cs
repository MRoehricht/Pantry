using Pantry.Plan.Api.Database.Entities;
using Pantry.Shared.Models.PlanModels;
using Pantry.Shared.Models.PlanModels.MealRequestModels;
using Riok.Mapperly.Abstractions;

namespace Pantry.Plan.Api.Configuration;

[Mapper]
public partial class PlanMapper
{
    public partial Meal MealEntityToMeal(MealEntity mealEntity);
    public partial MealEntity MealCreateDtoToMealEntity(MealCreateDto mealCreateDto);
    public partial IEnumerable<Meal> MealEntitiesToMeals(List<MealEntity> mealEntity);
}
