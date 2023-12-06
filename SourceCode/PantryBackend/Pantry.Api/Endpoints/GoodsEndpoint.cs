using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pantry.Api.Database.Contexts;
using Pantry.Api.Database.Entities;
using Pantry.Shared.Models.GoodModels;

namespace Pantry.Api.Endpoints;

public static class GoodsEndpoint
{
    public static RouteGroupBuilder MapGoodsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetGoods).WithName("GetGoods").Produces<IEnumerable<Good>>(StatusCodes.Status200OK).WithTags("Goods").WithOpenApi();
        group.MapGet("/{id}", GetGood).WithName("GetGoodById").Produces<Good>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound).WithTags("Goods").WithOpenApi();
        group.MapPost("/", CreateGood).WithName("CreateGood").Produces<Good>(StatusCodes.Status201Created).WithTags("Goods").WithOpenApi();
        group.MapPut("/", UpdateGood).WithName("UpdateGood").Produces(StatusCodes.Status404NotFound).Produces(StatusCodes.Status204NoContent).WithTags("Goods").WithOpenApi();

        return group;
    }

    private static async Task<IResult> UpdateGood(PantryContext context, GoodEntity good)
    {
        var entity = await context.Goods.FindAsync(good.Id);

        if (entity == null) return Results.NotFound();



        await context.SaveChangesAsync();

        return Results.NoContent();
    }

    private static async Task<IResult> CreateGood(IMapper mapper, PantryContext context, GoodEntity good)
    {
        good.Id = Guid.NewGuid();

        await context.Goods.AddAsync(good);
        await context.SaveChangesAsync();


        return Results.Created($"/meals/{good.Id}", mapper.Map<Good>(good));
    }

    private static async Task<IResult> GetGood(IMapper mapper, PantryContext context, Guid id)
    {
        return await context.Goods.FindAsync(id) is GoodEntity good
            ? Results.Ok(mapper.Map<Good>(good))
            : Results.NotFound();
    }

    private static async Task<IResult> GetGoods(IMapper mapper, PantryContext context)
    {
        var goods = await context.Goods.AsNoTracking().ToListAsync();
        return Results.Ok(mapper.Map<IEnumerable<Good>>(goods));
    }
}
