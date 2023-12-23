using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pantry.Api.Database.Contexts;
using Pantry.Services.UserServices;
using Pantry.Shared.Models.GoodModels;

namespace Pantry.Api.Endpoints {
    public static class SuggestionEndpoint 
    {
        public static RouteGroupBuilder MapSuggestionEndpoints(this RouteGroupBuilder group) 
        {
            group.MapGet("/", GetSuggestions).WithName("GetSuggestions").Produces<GoodSuggestion>().WithTags("GoodSuggestion").WithOpenApi();
            return group;
        }

        private static async Task<IResult> GetSuggestions(IMapper mapper, IHeaderEMailService eMailService, PantryContext context) {
            var eMail = eMailService.GetHeaderEMail();
            if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }

            var goods = await context.Goods.AsNoTracking().Where(e => e.Owner == eMail).ToListAsync();
            return Results.Ok(mapper.Map<IEnumerable<GoodSuggestion>>(goods));
        }
    }
}
