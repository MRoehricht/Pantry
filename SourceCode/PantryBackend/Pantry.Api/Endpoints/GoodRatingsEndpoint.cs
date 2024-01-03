using Pantry.Api.Configuration;
using Pantry.Api.Database.Contexts;
using Pantry.Api.Database.Entities;
using Pantry.Services.UserServices;
using Pantry.Shared.Models.GoodModels;
using Pantry.Shared.Models.GoodModels.MealRequestModels;

namespace Pantry.Api.Endpoints
{
    public static class GoodRatingsEndpoint
    {
        public static RouteGroupBuilder MapGoodRatingsEndpoint(this RouteGroupBuilder group)
        {
            group.MapGet("/{id}", GetRatings).WithName("GetRatingsById").Produces<GoodRating>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound).WithTags("GoodRating").WithOpenApi();
            group.MapPost("/{id}", CreateRatings).WithName("CreateRatings").Produces(StatusCodes.Status404NotFound).Produces(StatusCodes.Status204NoContent).WithTags("GoodRating").WithOpenApi();

            return group;
        }

        private static async Task<IResult> GetRatings(IHeaderEMailService eMailService, PantryContext context, Guid id)
        {
            var eMail = eMailService.GetHeaderEMail();
            if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

            var mapper = new PantryMapper();
            return await context.Goods.FindAsync(id) is GoodEntity good && good.Owner == eMail
                ? Results.Ok(mapper.MapToGoodRating(good))
                : Results.NotFound();
        }

        private static async Task<IResult> CreateRatings(IHeaderEMailService eMailService, PantryContext context, GoodRatingCreateDto goodRating)
        {
            var eMail = eMailService.GetHeaderEMail();
            if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

            var entity = await context.Goods.FindAsync(goodRating.GoodId);

            if (entity == null || entity.Owner != eMail)
            {
                return Results.NotFound();
            }

            entity.Details.Ratings.Add(goodRating.Rating);
            await context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
