using MassTransit;
using Microsoft.EntityFrameworkCore;
using Pantry.Module.Authentication.UserServices;
using Pantry.Module.Recipe.Configuration;
using Pantry.Module.Recipe.Database.Contexts;
using Pantry.Module.Recipe.Database.Entities;
using Pantry.Module.Shared.Models.MessageModes;
using Pantry.Module.Shared.Models.RecipeModels;
using Pantry.Module.Shared.Models.RecipeModels.RecipeRequestModels;

namespace Pantry.Module.Recipe.Endpoints;

public static class RecipeEndpoint
{
    public static RouteGroupBuilder MapRecipesEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/overview/{id}", GetRecipeOverviewById).WithName("GetRecipeOverviewById").Produces<IList<RecipeOverview>>().Produces(StatusCodes.Status404NotFound).WithOpenApi();
        group.MapGet("/overview/", GetRecipeOverview).WithName("GetRecipeOverview").Produces<IList<RecipeOverview>>().WithOpenApi();
        group.MapGet("/", GetRecipes).WithName("GetRecipes").Produces<IList<Shared.Models.RecipeModels.Recipe>>().WithOpenApi();
        group.MapGet("/{id}", GetRecipe).WithName("GetRecipeById").Produces<Shared.Models.RecipeModels.Recipe>().Produces(StatusCodes.Status404NotFound).WithOpenApi();
        group.MapPost("/", CreateRecipe).WithName("CreateRecipe").Produces<Shared.Models.RecipeModels.Recipe>(StatusCodes.Status201Created).WithOpenApi();
        group.MapPut("/", UpdateRecipe).WithName("UpdateRecipe").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound).WithOpenApi();
        group.MapDelete("/{id}", DeleteRecipe).WithName("DeleteRecipe").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound).WithOpenApi();

        return group;
    }

    private static async Task<IResult> UpdateRecipe(IHeaderEMailService eMailService, RecipeContext context, RecipeUpdateDto recipe)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        if (await context.Recipes.FindAsync(recipe.Id) is RecipeEntity recipeRecord && recipeRecord.Owner == eMail)
        {
            recipeRecord.Name = recipe.Name;
            recipeRecord.Description = recipe.Description;

            context.Recipes.Update(recipeRecord);
            await context.SaveChangesAsync();
            return Results.NoContent();
        }

        return Results.NotFound();
    }

    private static async Task<IResult> DeleteRecipe(IPublishEndpoint publishEndpoint, IHeaderEMailService eMailService, RecipeContext context, Guid id)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        if (await context.Recipes.FindAsync(id) is RecipeEntity recipe && recipe.Owner == eMail)
        {
            context.Recipes.Remove(recipe);
            await context.SaveChangesAsync();

            await publishEndpoint.Publish(new RecipeIsDeletedMessage { RecipeId = id });

            return Results.NoContent();
        }

        return Results.NotFound();
    }

    private static async Task<IResult> GetRecipe(IHeaderEMailService eMailService, RecipeContext context, Guid id)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }
        var mapper = new RecipeMapper();

        return await context.Recipes.FindAsync(id) is RecipeEntity recipe && recipe.Owner == eMail
            ? Results.Ok(mapper.MapToRecipe(recipe))
            : Results.NotFound();
    }

    private static async Task<IResult> CreateRecipe(IHeaderEMailService eMailService, RecipeContext context, RecipeCreateDto recipe)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }
        var mapper = new RecipeMapper();

        var entity = mapper.MapToRecipeEntity(recipe);

        entity.Id = Guid.NewGuid();
        entity.CreatedOn = DateTime.Now;
        entity.Owner = eMail;

        await context.Recipes.AddAsync(entity);
        await context.SaveChangesAsync();

        return Results.Created($"/recipes/{entity.Id}", mapper.MapToRecipe(entity));
    }


    private static async Task<IResult> GetRecipeOverviewById(IHeaderEMailService eMailService, RecipeContext context, Guid id)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var recipes = await context.Recipes.FindAsync(id);

        if (await context.Recipes.FindAsync(id) is RecipeEntity recipe && recipe.Owner == eMail)
        {
            var mapper = new RecipeMapper();
            if (recipe.Description is { Length: > 50 })
                recipe.Description = recipe.Description[..50];
            return Results.Ok(mapper.MapToRecipeOverview(recipe));

        }

        return Results.NotFound();
    }


    private static async Task<IResult> GetRecipeOverview(IHeaderEMailService eMailService, RecipeContext context)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var recipes = await context.Recipes.AsNoTracking().Where(e => e.Owner == eMail).ToListAsync();

        foreach (var recipe in recipes)
        {
            if (recipe.Description is { Length: > 50 })
                recipe.Description = recipe.Description[..50];
        }

        var mapper = new RecipeMapper();
        return Results.Ok(mapper.MapToRecipeOverviews(recipes));
    }


    private static async Task<IResult> GetRecipes(IHeaderEMailService eMailService, RecipeContext context)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }
        var mapper = new RecipeMapper();

        var recipes = await context.Recipes.AsNoTracking().Where(e => e.Owner == eMail).ToListAsync();
        return Results.Ok(mapper.MapToRecipes(recipes));
    }
}
