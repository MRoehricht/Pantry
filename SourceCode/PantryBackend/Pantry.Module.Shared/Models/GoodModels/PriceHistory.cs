namespace Pantry.Module.Shared.Models.GoodModels;
public class PriceHistory
{
    public Guid Id { get; set; }
    public Guid GoodId { get; set; }
    public DateOnly Date { get; set; }
    public double Price { get; set; }
}
