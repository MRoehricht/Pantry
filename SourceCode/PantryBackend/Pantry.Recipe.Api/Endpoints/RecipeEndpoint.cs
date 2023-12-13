
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pantry.Recipe.Api.Database.Contexts;
using Pantry.Recipe.Api.Database.Entities;
using Pantry.Shared.Models.RecipeModels;
using Pantry.Shared.Models.RecipeModels.RecipeRequestModels;

namespace Pantry.Recipe.Api.Endpoints;

public static class RecipeEndpoint
{
    public static RouteGroupBuilder MapRecipesEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/overview/", GetRecipeOverview).WithName("GetRecipeOverview").Produces<IList<RecipeOverview>>().WithOpenApi();
        group.MapGet("/", GetRecipes).WithName("GetRecipes").Produces<IList<Shared.Models.RecipeModels.Recipe>>().WithOpenApi();
        group.MapGet("/{id}", GetRecipe).WithName("GetRecipeById").Produces<Shared.Models.RecipeModels.Recipe>().Produces(StatusCodes.Status404NotFound).WithOpenApi();
        group.MapPost("/", CreateRecipe).WithName("CreateRecipe").Produces<Shared.Models.RecipeModels.Recipe>(StatusCodes.Status201Created).WithOpenApi();
        group.MapPut("/", UpdateRecipe).WithName("UpdateRecipe").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound).WithOpenApi();
        group.MapDelete("/{id}", DeleteRecipe).WithName("DeleteRecipe").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound).WithOpenApi();

        return group;
    }

    private static async Task<IResult> UpdateRecipe(IMapper mapper, RecipeContext context, RecipeUpdateDto recipe)
    {
        if (await context.Recipes.FindAsync(recipe.Id) is RecipeEntity recipeRecord)
        {
            recipeRecord.Name = recipe.Name;
            recipeRecord.Description = recipe.Description;

            context.Recipes.Update(recipeRecord);
            await context.SaveChangesAsync();
            return Results.NoContent();
        }

        return Results.NotFound();
    }

    private static async Task<IResult> DeleteRecipe(RecipeContext context, Guid id)
    {
        if (await context.Recipes.FindAsync(id) is RecipeEntity recipe)
        {
            context.Recipes.Remove(recipe);
            await context.SaveChangesAsync();
            return Results.NoContent();
        }

        return Results.NotFound();
    }

    private static async Task<IResult> GetRecipe(IMapper mapper, RecipeContext context, Guid id)
    {
        return await context.Recipes.FindAsync(id) is RecipeEntity recipe
            ? Results.Ok(mapper.Map<Shared.Models.RecipeModels.Recipe>(recipe))
            : Results.NotFound();
    }

    private static async Task<IResult> CreateRecipe(IMapper mapper, RecipeContext context, RecipeCreateDto recipe)
    {
        var entity = mapper.Map<RecipeEntity>(recipe);

        entity.Id = Guid.NewGuid();
        entity.CreatedOn = DateTime.Now;

        await context.Recipes.AddAsync(entity);
        await context.SaveChangesAsync();

        return Results.Created($"/recipes/{entity.Id}", mapper.Map<Shared.Models.RecipeModels.Recipe>(entity));
    }

    private static async Task<IResult> GetRecipeOverview(IMapper mapper, RecipeContext context) {
        var recipes = await context.Recipes.AsNoTracking().ToListAsync();

        foreach (var recipe in recipes) {
            if (recipe.Description is { Length: > 50 })
                recipe.Description = recipe.Description[..50];
        }

        return Results.Ok(mapper.Map<IEnumerable<RecipeOverview>>(recipes));
    }


    private static async Task<IResult> GetRecipes(IMapper mapper, RecipeContext context)
    {
        var recipes = await context.Recipes.AsNoTracking().ToListAsync();
        return Results.Ok(mapper.Map<IEnumerable<Shared.Models.RecipeModels.Recipe>>(recipes));
    }
}
