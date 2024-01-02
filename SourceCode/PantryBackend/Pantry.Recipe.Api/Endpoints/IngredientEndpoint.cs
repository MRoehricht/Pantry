﻿using AutoMapper;
using MassTransit;
using Pantry.Recipe.Api.Database.Contexts;
using Pantry.Recipe.Api.Database.Entities;
using Pantry.Services.UserServices;
using Pantry.Shared.Models.MessageModes;
using Pantry.Shared.Models.RecipeModels;

namespace Pantry.Recipe.Api.Endpoints;

public static class IngredientEndpoint
{
    public static RouteGroupBuilder MapIngredientsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{recipeId}", GetIngredients).WithName("GetIngredients").Produces<IList<Ingredient>>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound).WithOpenApi();
        group.MapGet("/{recipeId}/{name}", GetIngredient).WithName("GetIngredientById").Produces<Ingredient>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound).WithOpenApi();
        group.MapPost("/{recipeId}", CreateIngredient).WithName("CreateIngredient").Produces<Ingredient>(StatusCodes.Status201Created).WithOpenApi();
        group.MapPut("/{recipeId}/{name}", UpdateIngredient).WithName("UpdateIngredient").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound).WithOpenApi();
        group.MapDelete("/{recipeId}/{name}", DeleteIngredient).WithName("DeleteIngredient").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound).WithOpenApi();

        return group;
    }

    private static async Task<IResult> DeleteIngredient(IHeaderEMailService eMailService, RecipeContext context, Guid recipeId, string name)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var recipe = await context.Recipes.FindAsync(recipeId);
        if (recipe != null && recipe.Owner == eMail && recipe.Ingredients.FirstOrDefault(i => i.Name == name) is IngredientEntity ingredientEntity)
        {
            recipe.Ingredients.Remove(ingredientEntity);
            await context.SaveChangesAsync();

            return Results.NoContent();
        }
        return Results.NotFound();
    }

    private static async Task<IResult> UpdateIngredient(IHeaderEMailService eMailService, RecipeContext context, Guid recipeId, string name, Ingredient ingredient)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var recipe = await context.Recipes.FindAsync(recipeId);
        if (recipe != null && recipe.Owner == eMail && recipe.Ingredients.FirstOrDefault(i => i.Name == name) is IngredientEntity ingredientRecord)
        {
            ingredientRecord.CountOff = ingredient.CountOff;
            ingredientRecord.Unit = ingredient.Unit;

            await context.SaveChangesAsync();

            return Results.NoContent();
        }
        return Results.NotFound();
    }

    private static async Task<IResult> CreateIngredient(IMapper mapper, IPublishEndpoint publishEndpoint, IHeaderEMailService eMailService, RecipeContext context, Guid recipeId, Ingredient ingredient)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var recipe = await context.Recipes.FindAsync(recipeId);
        if (recipe != null && recipe.Owner == eMail)
        {
            var entity = mapper.Map<IngredientEntity>(ingredient);
            if (entity.PantryItemId is null)
            {
                entity.PantryItemId = Guid.NewGuid();
                var messag = new RegisterGoodMessage { Ingredient = mapper.Map<Ingredient>(entity), Owner = eMail };
                await publishEndpoint.Publish(messag);
            }

            recipe.Ingredients.Add(entity);
            await context.SaveChangesAsync();

            return Results.Created($"/ingredients/{recipe.Id}/{ingredient.Name}", ingredient);
        }
        return Results.NotFound();
    }

    private static async Task<IResult> GetIngredient(IMapper mapper, IHeaderEMailService eMailService, RecipeContext context, Guid recipeId, string name)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var recipe = await context.Recipes.FindAsync(recipeId);
        if (recipe != null && recipe.Owner == eMail && recipe.Ingredients.FirstOrDefault(i => i.Name == name) is IngredientEntity ingredientEntity)
        {
            return Results.Ok(mapper.Map<Ingredient>(ingredientEntity));
        }
        return Results.NotFound();
    }

    private static async Task<IResult> GetIngredients(IMapper mapper, IHeaderEMailService eMailService, RecipeContext context, Guid recipeId)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var recipe = await context.Recipes.FindAsync(recipeId);
        if (recipe != null && recipe.Owner == eMail)
        {
            return Results.Ok(mapper.Map<IEnumerable<Ingredient>>(recipe.Ingredients));
        }
        return Results.NotFound();
    }
}

