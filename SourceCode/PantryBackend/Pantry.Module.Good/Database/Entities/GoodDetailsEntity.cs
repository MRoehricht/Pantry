namespace Pantry.Module.Good.Database.Entities;

public class GoodDetailsEntity
{
    public List<string> Tags { get; set; } = new();
    public List<string> PurchaseLocations { get; set; } = new();
    public List<int> Ratings { get; set; } = new();
    public int TotalPurchaseNumber { get; set; }
    public int? DaysUntilConsume { get; set; }

}
