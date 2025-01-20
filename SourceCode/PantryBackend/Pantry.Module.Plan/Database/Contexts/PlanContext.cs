using Microsoft.EntityFrameworkCore;
using Pantry.Module.Plan.Database.Entities;

namespace Pantry.Module.Plan.Database.Contexts;

public class PlanContext : DbContext
{
    public PlanContext(DbContextOptions<PlanContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    internal DbSet<MealEntity> Meals { get; set; }
}
