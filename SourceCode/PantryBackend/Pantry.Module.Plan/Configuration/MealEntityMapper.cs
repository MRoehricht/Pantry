using Pantry.Module.Plan.Database.Entities;
using Pantry.Module.Shared.Models.PlanModels;
using Pantry.Module.Shared.Models.PlanModels.MealRequestModels;
using Riok.Mapperly.Abstractions;

namespace Pantry.Module.Plan.Configuration;

[Mapper]
public partial class PlanMapper
{
    public partial Meal MealEntityToMeal(MealEntity mealEntity);
    public partial MealEntity MealCreateDtoToMealEntity(MealCreateDto mealCreateDto);
    public partial IEnumerable<Meal> MealEntitiesToMeals(List<MealEntity> mealEntity);
}
