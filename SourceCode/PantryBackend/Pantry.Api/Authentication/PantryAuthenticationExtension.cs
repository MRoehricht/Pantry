using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Pantry.Api.Authentication;

public static class PantryAuthenticationExtension
{
    public static IServiceCollection AddPantryAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();

        services.AddAuthorization();
        
        services.AddDbContext<DbContext>(options =>
        {
            var path = $"Filename={Path.Combine(Path.GetTempPath(), "openiddict-velusia-server.sqlite3")}";
            options.UseSqlite(path);
            options.UseOpenIddict();
        });

        services.AddOpenIddict()

            // Register the OpenIddict core components.
            .AddCore(options =>
            {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                options.UseEntityFrameworkCore()
                    .UseDbContext<DbContext>();
            });

        services.AddOpenIddict()
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
                        options.SetClientId(configuration["Authentication:GitHub:ClientId"]!)
                            .SetClientSecret(configuration["Authentication:GitHub:ClientSecret"]!)
                            .SetRedirectUri("callback/login/github");
                    });
            });
        
        return services;
    }
}