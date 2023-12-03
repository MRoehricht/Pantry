using Pantry.Shared.Converter.JsonConverter;
using System.Text.Json.Serialization;

namespace Pantry.Shared.Models.PlanModels.MealRequestModels;
public class MealCreateDto
{
    public Guid RecipeId { get; set; }

    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly Date { get; set; }
}
