using Pantry.Module.Shared.Models.GoodModels;

namespace Pantry.Module.Good.Database.Entities;

public class GoodEntity
{
    public Guid Id { get; set; }
    public string Owner { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public double Amount { get; set; }
    public double? MinimumAmount { get; set; }
    public UnitOfMeasurement UnitOfMeasurement { get; set; }
    public string? StorageLocation { get; set; }
    public long? EAN { get; set; }
    public double? CurrentPrice { get; set; }
    public string? ShoppinglistName { get; set; }
    public GoodDetailsEntity Details { get; set; } = new GoodDetailsEntity();
    public List<PriceHistoryEntity> PriceHistories { get; set; } = new();
}
