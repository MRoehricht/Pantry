using AutoMapper;
using Pantry.Recipe.Api.Database.Contexts;
using Pantry.Shared.Models.RecipeModels;

namespace Pantry.Recipe.Api.Endpoints;

public static class RecipeDetailsEndpoint
{
    public static RouteGroupBuilder MapRecipeDetailsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{recipeId}", GetRecipeDetails).WithName("GetRecipeDetails").Produces<RecipeDetails>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound).WithOpenApi();
        group.MapPost("/review/{recipeId}", CreateReview).WithName("CreateReview").Produces<RecipeDetails>(StatusCodes.Status201Created).WithOpenApi();
        group.MapPost("/cookedon/{recipeId}", CreateCookedOn).WithName("CreateCookedOn").Produces<RecipeDetails>(StatusCodes.Status201Created).WithOpenApi();

        group.MapPost("/tag/{recipeId}", CreateTag).WithName("CreateTag").Produces<RecipeDetails>(StatusCodes.Status201Created).Produces(StatusCodes.Status404NotFound).WithOpenApi();
        group.MapDelete("/tag/{recipeId}/{tag}", DeleteTag).WithName("DeleteTag").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound).WithOpenApi();

        return group;
    }

    private static async Task<IResult> DeleteTag(IMapper mapper, RecipeContext context, Guid recipeId, string tag)
    {
        var recipe = await context.Recipes.FindAsync(recipeId);
        if (recipe != null && recipe.Details.Tags.Contains(tag))
        {
            recipe.Details.Tags.Remove(tag);
            await context.SaveChangesAsync();

            return Results.NoContent();
        }
        return Results.NotFound();
    }

    private static async Task<IResult> CreateTag(IMapper mapper, RecipeContext context, Guid recipeId, string tag)
    {
        var recipe = await context.Recipes.FindAsync(recipeId);
        if (recipe != null)
        {
            recipe.Details.Tags.Add(tag);
            await context.SaveChangesAsync();

            return Results.Created($"/recipedetails/{recipe.Id}", mapper.Map<RecipeDetails>(recipe.Details));
        }
        return Results.NotFound();
    }

    private static async Task<IResult> CreateReview(IMapper mapper, RecipeContext context, Guid recipeId, int review)
    {
        var recipe = await context.Recipes.FindAsync(recipeId);
        if (recipe != null)
        {
            recipe.Details.Reviews.Add(review > 5 ? 5 : review);
            await context.SaveChangesAsync();

            return Results.Created($"/recipedetails/{recipe.Id}", mapper.Map<RecipeDetails>(recipe.Details));
        }
        return Results.NotFound();
    }

    private static async Task<IResult> CreateCookedOn(IMapper mapper, RecipeContext context, Guid recipeId, DateOnly date)
    {
        var recipe = await context.Recipes.FindAsync(recipeId);
        if (recipe != null)
        {
            recipe.Details.CookedOn.Add(date);
            await context.SaveChangesAsync();

            return Results.Created($"/recipedetails/{recipe.Id}", mapper.Map<RecipeDetails>(recipe.Details));
        }
        return Results.NotFound();
    }

    private static async Task<IResult> GetRecipeDetails(IMapper mapper, RecipeContext context, Guid recipeId)
    {
        var recipe = await context.Recipes.FindAsync(recipeId);
        if (recipe != null)
        {

            return Results.Ok(mapper.Map<RecipeDetails>(recipe.Details));
        }
        return Results.NotFound();
    }
}
