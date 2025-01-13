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
using Pantry.Gateway.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));






builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddAuthorization();



builder.Services.AddDbContext<DbContext>(options =>
{
    options.UseInMemoryDatabase("db");
    options.UseOpenIddict();
});

builder.Services.AddOpenIddict()

    // Register the OpenIddict core components.
    .AddCore(options =>
    {
        // Configure OpenIddict to use the Entity Framework Core stores and models.
        options.UseEntityFrameworkCore()
            .UseDbContext<DbContext>();
    });

builder.Services.AddOpenIddict()
    // Register the OpenIddict client components.
    .AddClient(options =>
    {
        // Allow the OpenIddict client to negotiate the authorization code flow.
        options.AllowAuthorizationCodeFlow();

        // Register the signing and encryption credentials used to protect
        // sensitive data like the state tokens produced by OpenIddict.
        options.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();

        // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        options.UseAspNetCore()
            .EnableRedirectionEndpointPassthrough();

        // Register the GitHub integration.
        options.UseWebProviders()
            .AddGitHub(options =>
            {
                options.SetClientId("[]")
                    .SetClientSecret("][")
                    .SetRedirectUri("callback/login/github");
            });
    });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Add a minimal action responsible for triggering a GitHub challenge
// and redirecting the user agent to the GitHub authorization endpoint.
app.MapGet("challenge",
    () => Results.Challenge(properties: null,
        authenticationSchemes: [OpenIddictClientWebIntegrationConstants.Providers.GitHub]));

app.MapMethods("callback/login/github", [HttpMethods.Get, HttpMethods.Post], async (HttpContext context) =>
{
    // Retrieve the authorization data validated by OpenIddict as part of the callback handling.
    var result = await context.AuthenticateAsync(OpenIddictClientWebIntegrationConstants.Providers.GitHub);

    // Build an identity based on the external claims and that will be used to create the authentication cookie.
    var identity = new ClaimsIdentity(authenticationType: "ExternalLogin");

    // By default, OpenIddict will automatically try to map the email/name and name identifier claims from
    // their standard OpenID Connect or provider-specific equivalent, if available. If needed, additional
    // claims can be resolved from the external identity and copied to the final authentication cookie.
    identity.SetClaim(ClaimTypes.Email, result.Principal!.GetClaim(ClaimTypes.Email))
            .SetClaim(ClaimTypes.Name, result.Principal!.GetClaim(ClaimTypes.Name))
            .SetClaim(ClaimTypes.NameIdentifier, result.Principal!.GetClaim(ClaimTypes.NameIdentifier));

    // Preserve the registration details to be able to resolve them later.
    identity.SetClaim(OpenIddictConstants.Claims.Private.RegistrationId, result.Principal!.GetClaim(OpenIddictConstants.Claims.Private.RegistrationId))
            .SetClaim(OpenIddictConstants.Claims.Private.ProviderName, result.Principal!.GetClaim(OpenIddictConstants.Claims.Private.ProviderName));

    // Build the authentication properties based on the properties that were added when the challenge was triggered.
    var properties = new AuthenticationProperties(result.Properties.Items)
    {
        RedirectUri = "/whoami" //result.Properties.RedirectUri ?? "/whoami"
    };

    // Ask the default sign-in handler to return a new cookie and redirect the
    // user agent to the return URL stored in the authentication properties.
    //
    // For scenarios where the default sign-in handler configured in the ASP.NET Core
    // authentication options shouldn't be used, a specific scheme can be specified here.
    return Results.SignIn(new ClaimsPrincipal(identity), properties);
});

app.MapGet("whoami", async (HttpContext context) =>
{
    var result = await context.AuthenticateAsync();
    if (result is not { Succeeded: true })
    {
        return Results.Text("You're not logged in.");
    }

    return Results.Text(string.Format("You are {0}.", result.Principal.FindFirst(ClaimTypes.Name)!.Value));
});

app.MapGet("signout", async (HttpContext context) =>
{
    await context.SignOutAsync();
    return Results.Text("You're not logged out.");
});



app.MapReverseProxy();
app.Run();

