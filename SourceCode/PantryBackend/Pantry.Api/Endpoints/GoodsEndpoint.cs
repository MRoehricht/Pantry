﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pantry.Api.Database.Contexts;
using Pantry.Api.Database.Entities;
using Pantry.Services.RabbitMqServices;
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

    private static async Task<IResult> DeleteGood(PantryContext context, IHeaderEMailService eMailService, Guid id) {

        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var entity = await context.Goods.FindAsync(id);

        if (entity == null || entity.Owner != eMail) {
            return Results.NotFound();
        }

        context.Goods.Remove(entity);
        await context.SaveChangesAsync();

        return Results.NoContent();
    }

    private static async Task<IResult> UpdateGood(IRabbitMqPublisher rabbitMqPublisher, IHeaderEMailService eMailService, PantryContext context, GoodEntity good)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var entity = await context.Goods.FindAsync(good.Id);

        if (entity == null || entity.Owner != eMail) return Results.NotFound();

        if (entity.Name != good.Name) {
            var message = new Ingredient { PantryItemId = entity.Id, Name = good.Name };
            rabbitMqPublisher.SendMessage(message, MessageType.UpdateIngredientName);
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

        await context.SaveChangesAsync();

        return Results.NoContent();
    }

    private static async Task<IResult> CreateGood(IMapper mapper, IHeaderEMailService eMailService, PantryContext context, GoodCreateDto goodCreateDto) {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var entity = new GoodEntity() {Id = Guid.NewGuid(), Name = goodCreateDto.Name, Owner = eMail };

        await context.Goods.AddAsync(entity);
        await context.SaveChangesAsync();


        return Results.Created($"/meals/{entity.Id}", mapper.Map<Good>(entity));
    }

    private static async Task<IResult> GetGood(IMapper mapper, IHeaderEMailService eMailService, PantryContext context, Guid id)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        return await context.Goods.FindAsync(id) is GoodEntity good && good.Owner == eMail
            ? Results.Ok(mapper.Map<Good>(good))
            : Results.NotFound();
    }

    private static async Task<IResult> GetGoods(IMapper mapper, IHeaderEMailService eMailService, PantryContext context)
    {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

        var goods = await context.Goods.AsNoTracking().Where(e => e.Owner == eMail).ToListAsync();
        return Results.Ok(mapper.Map<IEnumerable<Good>>(goods));
    }

    private static async Task<IResult> GetGoodsOverview(IMapper mapper, IHeaderEMailService eMailService, PantryContext context) {
        var eMail = eMailService.GetHeaderEMail();
        if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized();}

        var goods = await context.Goods.AsNoTracking().Where(e => e.Owner == eMail).ToListAsync();
        return Results.Ok(mapper.Map<IEnumerable<GoodsOverview>>(goods).OrderBy(_ => _.Name));
    }
    
}
