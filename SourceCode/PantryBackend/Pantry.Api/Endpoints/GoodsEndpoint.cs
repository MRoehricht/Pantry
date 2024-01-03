using MassTransit;
using Microsoft.EntityFrameworkCore;
using Pantry.Api.Configuration;
using Pantry.Api.Database.Contexts;
using Pantry.Api.Database.Entities;
using Pantry.Api.Metrics;
using Pantry.Services.UserServices;
using Pantry.Shared.Models.GoodModels;
using Pantry.Shared.Models.GoodModels.MealRequestModels;
using Pantry.Shared.Models.MessageModes;
using Pantry.Shared.Models.RecipeModels;

namespace Pantry.Api.Endpoints;

public static class GoodsEndpoint
{
    public static RouteGroupBuilder MapGoodsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/overview/", GetGoodsOverview).WithName("GetGoodsOverview").Produces<IEnumerable<GoodsOverview>>(StatusCodes.Status200OK).WithTags("Goods").WithOpenApi();
        group.MapGet("/", GetGoods).WithName("GetGoods").Produces<IEnumerable<Good>>(StatusCodes.Status200OK).WithTags("Goods").WithOpenApi();
        group.MapGet("/{id}", GetGood).WithName("GetGoodById").Produces<Good>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound).WithTags("Goods").WithOpenApi();
        group.MapPost("/", CreateGood).WithName("CreateGood").Produces<Good>(StatusCodes.Status201Created).WithTags("Goods").WithOpenApi();
        group.MapPut("/", UpdateGood).WithName("UpdateGood").Produces(StatusCodes.Status404NotFound).Produces(StatusCodes.Status204NoContent).WithTags("Goods").WithOpenApi();
        group.MapDelete("/{id}", DeleteGood).WithName("DeleteGood").Produces(StatusCodes.Status404NotFound).Produces(StatusCodes.Status204NoContent).WithTags("Goods").WithOpenApi();

        return group;
    }

    private static async Task<IResult> DeleteGood(PantryContext context, IHeaderEMailService eMailService, Guid id)
    {

        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var entity = await context.Goods.FindAsync(id);

        if (entity == null || entity.Owner != eMail)
        {
            return Results.NotFound();
        }

        context.Goods.Remove(entity);
        await context.SaveChangesAsync();

        return Results.NoContent();
    }

    private static async Task<IResult> UpdateGood(IPublishEndpoint publishEndpoint, IHeaderEMailService eMailService, PantryContext context, GoodEntity good)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var entity = await context.Goods.FindAsync(good.Id);

        if (entity == null || entity.Owner != eMail) return Results.NotFound();

        if (entity.Name != good.Name || entity.UnitOfMeasurement != good.UnitOfMeasurement)
        {
            var message = new UpdateIngredientNameMessage { Ingredient = new Ingredient { PantryItemId = entity.Id, Name = good.Name, Unit = good.UnitOfMeasurement }, Owner = eMail };
            await publishEndpoint.Publish(message);
        }

        entity.Name = good.Name;
        entity.Description = good.Description;
        entity.Amount = good.Amount;
        entity.MinimumAmount = good.MinimumAmount;
        entity.StorageLocation = good.StorageLocation;
        entity.EAN = good.EAN;
        entity.CurrentPrice = good.CurrentPrice;
        entity.ShoppinglistName = good.ShoppinglistName;
        entity.Details.Tags = good.Details.Tags;
        entity.UnitOfMeasurement = good.UnitOfMeasurement;

        await context.SaveChangesAsync();

        return Results.NoContent();
    }

    private static async Task<IResult> CreateGood(IHeaderEMailService eMailService, PantryContext context, GoodCreateDto goodCreateDto)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var entity = new GoodEntity() { Id = Guid.NewGuid(), Name = goodCreateDto.Name, Owner = eMail };

        await context.Goods.AddAsync(entity);
        await context.SaveChangesAsync();

        var mapper = new PantryMapper();
        return Results.Created($"/meals/{entity.Id}", mapper.MapToGood(entity));
    }

    private static async Task<IResult> GetGood(IHeaderEMailService eMailService, PantryApiMetrics pantryApiMetrics, PantryContext context, Guid id)
    {
        pantryApiMetrics.IncrementRequestsCounter();
        using var _ = pantryApiMetrics.TrackRequestsDuration();

        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }
        var mapper = new PantryMapper();
        return await context.Goods.FindAsync(id) is GoodEntity good && good.Owner == eMail
            ? Results.Ok(mapper.MapToGood(good))
            : Results.NotFound();
    }

    private static async Task<IResult> GetGoods(IHeaderEMailService eMailService, PantryApiMetrics pantryApiMetrics, PantryContext context)
    {
        pantryApiMetrics.IncrementRequestsCounter();
        using var _ = pantryApiMetrics.TrackRequestsDuration();

        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var goods = await context.Goods.AsNoTracking().Where(e => e.Owner == eMail).ToListAsync();
        var mapper = new PantryMapper();
        return Results.Ok(mapper.MapToGoods(goods));
    }

    private static async Task<IResult> GetGoodsOverview(IHeaderEMailService eMailService, PantryApiMetrics pantryApiMetrics, PantryContext context)
    {
        pantryApiMetrics.IncrementRequestsCounter();
        using var _ = pantryApiMetrics.TrackRequestsDuration();

        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var goods = await context.Goods.AsNoTracking().Where(e => e.Owner == eMail).OrderBy(_ => _.Name).ToListAsync();
        var mapper = new PantryMapper();
        return Results.Ok(mapper.MapToGoodsOverviews(goods));
    }

}
