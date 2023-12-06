using Microsoft.EntityFrameworkCore;
using Pantry.Api.Database.Entities;

namespace Pantry.Api.Database.Contexts;

public class PantryContext : DbContext
{
    public PantryContext(DbContextOptions<PantryContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    internal DbSet<GoodEntity> Goods { get; set; }
    internal DbSet<PriceHistoryEntity> PriceHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GoodEntity>().OwnsOne(e => e.Details, b => { b.ToJson(); });
        modelBuilder.Entity<GoodEntity>().HasMany(e => e.PriceHistories).WithOne(b => b.GoodEntity);
    }
}
