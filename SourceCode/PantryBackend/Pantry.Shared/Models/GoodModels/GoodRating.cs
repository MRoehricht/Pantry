namespace Pantry.Shared.Models.GoodModels; 

public class GoodRating {
    public Guid GoodId { get; set; }
    public List<int> Ratings { get; set; } = new();
}