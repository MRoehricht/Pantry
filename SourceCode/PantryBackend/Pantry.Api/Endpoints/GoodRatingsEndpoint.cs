using AutoMapper;
using Pantry.Api.Database.Contexts;
using Pantry.Api.Database.Entities;
using Pantry.Shared.Models.GoodModels;
using Pantry.Shared.Models.GoodModels.MealRequestModels;

namespace Pantry.Api.Endpoints {
    public static class GoodRatingsEndpoint {
        public static RouteGroupBuilder MapGoodRatingsEndpoint(this RouteGroupBuilder group) {
            group.MapGet("/{id}", GetRatings).WithName("GetRatingsById").Produces<GoodRating>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound).WithTags("GoodRating").WithOpenApi();
            group.MapPost("/{id}", CreateRatings).WithName("CreateRatings").Produces(StatusCodes.Status404NotFound).Produces(StatusCodes.Status204NoContent).WithTags("GoodRating").WithOpenApi();
           
            return group;
        }

        private static async Task<IResult> GetRatings(IMapper mapper, PantryContext context, Guid id) {
            return await context.Goods.FindAsync(id) is GoodEntity good
                ? Results.Ok(mapper.Map<GoodRating>(good))
                : Results.NotFound();
        }

        private static async Task<IResult> CreateRatings(PantryContext context, GoodRatingCreateDto goodRating) {
            var entity = await context.Goods.FindAsync(goodRating.GoodId);

            if (entity == null) {
                return Results.NotFound();
            }

            entity.Details.Ratings.Add(goodRating.Rating);            
            await context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
