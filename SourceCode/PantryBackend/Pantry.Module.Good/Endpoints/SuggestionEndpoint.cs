using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Pantry.Module.Authentication.UserServices;
using Pantry.Module.Good.Configuration;
using Pantry.Module.Good.Database.Contexts;
using Pantry.Module.Shared.Models.GoodModels;

namespace Pantry.Module.Good.Endpoints
{
    public static class SuggestionEndpoint
    {
        public static RouteGroupBuilder MapSuggestionEndpoints(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetSuggestions).WithName("GetSuggestions").Produces<GoodSuggestion>().WithTags("GoodSuggestion").WithOpenApi();
            return group;
        }

        private static async Task<IResult> GetSuggestions(IHeaderEMailService eMailService, PantryContext context)
        {
            var eMail = eMailService.GetHeaderEMail();
            if (string.IsNullOrEmpty(eMail)) { return Results.Unauthorized(); }
            Activity.Current?.SetTag(DiagnosticsNames.PantryUserEMail, eMail);

            var goods = await context.Goods.AsNoTracking().Where(e => e.Owner == eMail).ToListAsync();
            var mapper = new PantryMapper();
            return Results.Ok(mapper.MapToGoodSuggestions(goods));
        }
    }
}
