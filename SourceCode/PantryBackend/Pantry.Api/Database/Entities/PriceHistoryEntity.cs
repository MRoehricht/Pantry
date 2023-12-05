namespace Pantry.Api.Database.Entities;

public class PriceHistoryEntity
{
    public Guid Id { get; set; }
    public Guid GoodId { get; set; }
    public DateOnly Date { get; set; }
    public double Price { get; set; }
    public virtual GoodEntity? GoodEntity { get; set; }
}
