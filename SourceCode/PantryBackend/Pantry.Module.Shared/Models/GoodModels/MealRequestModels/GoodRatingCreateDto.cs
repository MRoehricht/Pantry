namespace Pantry.Module.Shared.Models.GoodModels.MealRequestModels; 

public class GoodRatingCreateDto {
    public Guid GoodId { get; set; }
    public int Rating { get; set; }
}