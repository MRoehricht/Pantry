using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pantry.Module.Recipe.Database.Entities;

namespace Pantry.Module.Recipe.Database.Contexts;

public class RecipeEntityTypeConfiguration: IEntityTypeConfiguration<RecipeEntity>
{
    public void Configure(EntityTypeBuilder<RecipeEntity> builder)
    {
        builder.ToTable("Recipes", "recipes");

        builder.OwnsMany(e => e.Ingredients, b => { b.ToJson(); });
        builder.OwnsOne(e => e.Details, b => { b.ToJson(); });

    }
}