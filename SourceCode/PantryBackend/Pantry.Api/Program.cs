using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Client.WebIntegration;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using Pantry.Api.Authentication;
using Pantry.Module.Recipe.Extensions;

var builder = WebApplication.CreateBuilder(args);


List<Assembly> mediatrAssemblies = [typeof(Program).Assembly];
builder.AddRecipeModule(mediatrAssemblies);

builder.Services.AddMediatR(c => { c.RegisterServicesFromAssemblies(mediatrAssemblies.ToArray()); });

builder.Services.AddPantryAuthentication(builder.Configuration);

var app = builder.Build();

// Create a new application registration matching the values configured in Mimban.Client.
// Note: in a real world application, this step should be part of a setup script.
await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DbContext>();
    await context.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/").AuthenticationEndpoints();
app.MapRecipeEndpoints();

app.MapReverseProxy();
app.Run();

