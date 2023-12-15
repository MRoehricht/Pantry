using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pantry.Plan.Api.Database.Contexts;
using Pantry.Plan.Api.Database.Entities;
using Pantry.Services.RabbitMqServices;
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

    private static async Task<IResult> UpdateMeal(IMapper mapper, PlanContext context, MealUpdateDto meal)
    {
        var entity = await context.Meals.FindAsync(meal.Id);

        if (entity == null) return Results.NotFound();

        entity.Date = meal.Date;
        entity.RecipeId = meal.RecipeId;
        entity.WasCooked = meal.WasCooked;

        await context.SaveChangesAsync();

        return Results.NoContent();
    }

    private static async Task<IResult> CreateMeal(IMapper mapper, IRabbitMqPublisher publisher, PlanContext context, MealCreateDto meal)
    {
        var entity = mapper.Map<MealEntity>(meal);

        entity.Id = Guid.NewGuid();

        await context.Meals.AddAsync(entity);
        await context.SaveChangesAsync();

        publisher.SendMessage(entity);

        return Results.Created($"/meals/{entity.Id}", mapper.Map<Meal>(entity));
    }

    private static async Task<IResult> GetMeal(IMapper mapper, PlanContext context, Guid id)
    {
        return await context.Meals.FindAsync(id) is MealEntity meal
            ? Results.Ok(mapper.Map<Meal>(meal))
            : Results.NotFound();
    }

    private static async Task<IResult> GetMeals(IMapper mapper, PlanContext context)
    {
        var recipes = await context.Meals.AsNoTracking().ToListAsync();
        return Results.Ok(mapper.Map<IEnumerable<Meal>>(recipes));
    }

    private static async Task<IResult> GetMealsByDate(IMapper mapper, PlanContext context, DateOnly date) {
        var recipes = await context.Meals.AsNoTracking().ToListAsync();
        return Results.Ok(mapper.Map<IEnumerable<Meal>>(recipes.Where(_ => _.Date == date)));
    }
}
