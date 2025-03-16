using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Pantry.Module.Authentication.UserServices;
using Pantry.Module.Plan.Configuration;
using Pantry.Module.Plan.Database.Contexts;
using Pantry.Module.Plan.Database.Entities;
using Pantry.Module.Shared.Models.MessageModes;
using Pantry.Module.Shared.Models.PlanModels;
using Pantry.Module.Shared.Models.PlanModels.MealRequestModels;

namespace Pantry.Module.Plan.Endpoints;

public static class MealsEndpoint
{
    public static RouteGroupBuilder MapMealsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/date/{date}", GetMealsByDate).WithName("GetMealsByDate").Produces<IEnumerable<Meal>>(StatusCodes.Status200OK).WithTags("Meals").WithOpenApi();
        group.MapGet("/", GetMeals).WithName("GetMeals").Produces<IEnumerable<Meal>>(StatusCodes.Status200OK).WithTags("Meals").WithOpenApi();
        group.MapGet("/{id}", GetMeal).WithName("GetMealById").Produces<Meal>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound).WithTags("Meals").WithOpenApi();
        group.MapPost("/", CreateMeal).WithName("CreateMeal").Produces<Meal>(StatusCodes.Status201Created).WithTags("Meals").WithOpenApi();
        group.MapPut("/", UpdateMeal).WithName("UpdateMeal").Produces(StatusCodes.Status404NotFound).Produces(StatusCodes.Status204NoContent).WithTags("Meals").WithOpenApi();

        return group;
    }

    private static async Task<IResult> UpdateMeal(IHeaderEMailService eMailService, IPublishEndpoint publishEndpoint, PlanContext context, MealUpdateDto meal)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var entity = await context.Meals.FindAsync(meal.Id);

        if (entity == null || entity.Owner != eMail) return Results.NotFound();

        entity.Date = meal.Date;
        entity.RecipeId = meal.RecipeId;
        entity.WasCooked = meal.WasCooked;

        await context.SaveChangesAsync();

        if (meal.WasCooked)
        {
            await publishEndpoint.Publish(new MealWasCookedMessage { RecipeId = meal.RecipeId });
        }

        return Results.NoContent();
    }

    private static async Task<IResult> CreateMeal(IHeaderEMailService eMailService, PlanContext context, MealCreateDto meal)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        if (meal == null) return Results.BadRequest();

        var mapper = new PlanMapper();
        var entity = mapper.MealCreateDtoToMealEntity(meal);

        entity.Id = Guid.NewGuid();
        entity.Owner = eMail;

        await context.Meals.AddAsync(entity);
        await context.SaveChangesAsync();

        return Results.Created($"/meals/{entity.Id}", mapper.MealEntityToMeal(entity));
    }

    private static async Task<IResult> GetMeal(IHeaderEMailService eMailService, PlanContext context, Guid id)
    {
        var mapper = new PlanMapper();
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        return await context.Meals.FindAsync(id) is MealEntity meal && meal.Owner == eMail
            ? Results.Ok(mapper.MealEntityToMeal(meal))
            : Results.NotFound();
    }

    private static async Task<IResult> GetMeals(IHeaderEMailService eMailService, PlanContext context)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }
        var mapper = new PlanMapper();

        var recipes = await context.Meals.AsNoTracking().Where(e => e.Owner == eMail).ToListAsync();
        return Results.Ok(mapper.MealEntitiesToMeals(recipes));
    }

    private static async Task<IResult> GetMealsByDate(IHeaderEMailService eMailService, PlanContext context, DateOnly date)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }
        var mapper = new PlanMapper();

        var recipes = await context.Meals.AsNoTracking().Where(e => e.Owner == eMail && e.Date == date).ToListAsync();
        return Results.Ok(mapper.MealEntitiesToMeals(recipes));
    }
}
