using System.Net;
using Pantry.Module.Recipe.IntegrationTests.Infrastructure;
using Shouldly;

namespace Pantry.Module.Recipe.IntegrationTests.Endpoints.RecipeDetailsEndpoints;

public class DeleteTagTests(RecipeIntegrationTestWebAppFactory factory)
    : RecipeBaseApiIntegrationTest(factory)
{
    [Fact]
    public async Task GetRecipe_Unauthorized()
    {
        var result = await HttpClientNonAuthenticated.DeleteAsync("/recipedetails/tag/" +  Guid.NewGuid() +"/tagName");

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetRecipe_NotFound()
    {
        var result = await HttpClientAuthenticated.DeleteAsync("/recipedetails/tag/" + Guid.NewGuid() + "/tagName");

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}