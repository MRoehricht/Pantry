using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pantry.Api.Database.Contexts;
using Pantry.Shared.Models.GoodModels;

namespace Pantry.Api.Endpoints {
    public static class SuggestionEndpoint {
        public static RouteGroupBuilder MapSuggestionEndpoints(this RouteGroupBuilder group) {
            group.MapGet("/", GetSuggestions).WithName("GetSuggestions").Produces<GoodSuggestion>().WithTags("GoodSuggestion").WithOpenApi();

            return group;
        }

        private static async Task<IResult> GetSuggestions(IMapper mapper, PantryContext context) {
            var goods = await context.Goods.AsNoTracking().ToListAsync();
            return Results.Ok(mapper.Map<IEnumerable<GoodSuggestion>>(goods));
        }
    }
}
