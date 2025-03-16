using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Pantry.Module.Recipe.Endpoints;

namespace Pantry.Module.Recipe.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapRecipeEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGroup("/recipes").MapRecipesEndpoint();
        builder.MapGroup("/ingredients").MapIngredientsEndpoint();
        builder.MapGroup("/recipedetails").MapRecipeDetailsEndpoint();

        return builder;
    }
}