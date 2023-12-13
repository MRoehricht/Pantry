namespace Pantry.Shared.Models.GoodModels;
public class Good
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public double Amount { get; set; }
    public double? MinimumAmount { get; set; }
    public string? StorageLocation { get; set; }
    public long? EAN { get; set; }
    public double? CurrentPrice { get; set; }
    public string? ShoppinglistName { get; set; }
    public GoodDetails Details { get; set; } = new GoodDetails();
    public List<PriceHistory> PriceHistories { get; set; } = new();
}
