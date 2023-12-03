using Microsoft.EntityFrameworkCore;
using Pantry.Recipe.Api.Database.Entities;

namespace Pantry.Recipe.Api.Database.Contexts;

public class RecipeContext : DbContext
{
    public RecipeContext(DbContextOptions<RecipeContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    internal DbSet<RecipeEntity> Recipes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecipeEntity>().OwnsMany(e => e.Ingredients, b => { b.ToJson(); });
        modelBuilder.Entity<RecipeEntity>().OwnsOne(e => e.Details, b => { b.ToJson(); });
    }
}
