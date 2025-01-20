using System.Text.Json.Serialization;
using Pantry.Module.Shared.Converter.JsonConverter;

namespace Pantry.Module.Shared.Models.PlanModels.MealRequestModels;
public class MealCreateDto
{
    public Guid RecipeId { get; set; }

    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly Date { get; set; }
}
