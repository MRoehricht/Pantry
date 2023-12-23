using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pantry.Plan.Api.Database.Contexts;
using Pantry.Plan.Api.Database.Entities;
using Pantry.Services.RabbitMqServices;
using Pantry.Services.UserServices;
using Pantry.Shared.Models.MessageModes;
using Pantry.Shared.Models.PlanModels;
using Pantry.Shared.Models.PlanModels.MealRequestModels;

namespace Pantry.Plan.Api.Endpoints;

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

    private static async Task<IResult> UpdateMeal(IMapper mapper, IHeaderEMailService eMailService, IRabbitMqPublisher publisher, PlanContext context, MealUpdateDto meal)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var entity = await context.Meals.FindAsync(meal.Id);

        if (entity == null || entity.Owner != eMail) return Results.NotFound();

        entity.Date = meal.Date;
        entity.RecipeId = meal.RecipeId;
        entity.WasCooked = meal.WasCooked;

        await context.SaveChangesAsync();

        if (meal.WasCooked) {
            publisher.SendMessage(meal.RecipeId, MessageType.MealWasCooked);
        }

        return Results.NoContent();
    }

    private static async Task<IResult> CreateMeal(IMapper mapper, IHeaderEMailService eMailService, PlanContext context, MealCreateDto meal)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var entity = mapper.Map<MealEntity>(meal);

        entity.Id = Guid.NewGuid();
        entity.Owner = eMail;

        await context.Meals.AddAsync(entity);
        await context.SaveChangesAsync();
        
        return Results.Created($"/meals/{entity.Id}", mapper.Map<Meal>(entity));
    }

    private static async Task<IResult> GetMeal(IMapper mapper, IHeaderEMailService eMailService, PlanContext context, Guid id)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        return await context.Meals.FindAsync(id) is MealEntity meal && meal.Owner == eMail
            ? Results.Ok(mapper.Map<Meal>(meal))
            : Results.NotFound();
    }

    private static async Task<IResult> GetMeals(IMapper mapper, IHeaderEMailService eMailService, PlanContext context)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var recipes = await context.Meals.AsNoTracking().Where(e => e.Owner == eMail).ToListAsync();
        return Results.Ok(mapper.Map<IEnumerable<Meal>>(recipes));
    }

    private static async Task<IResult> GetMealsByDate(IMapper mapper, IHeaderEMailService eMailService, PlanContext context, DateOnly date) {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var recipes = await context.Meals.AsNoTracking().Where(e => e.Owner == eMail).ToListAsync();
        return Results.Ok(mapper.Map<IEnumerable<Meal>>(recipes.Where(_ => _.Date == date)));
    }
}
