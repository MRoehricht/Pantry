using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Pantry.Gateway.Authentication;

public static class PantryAuthenticationExtension
{
    public static IServiceCollection AddPantryAuthentication(this IServiceCollection services)
    {
        services.AddDbContext<DbContext>(options =>
        {
            options.UseInMemoryDatabase("db");
            options.UseOpenIddict();
        });


        services.AddOpenIddict()

            // Register the OpenIddict core components.
            .AddCore(options =>
            {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                options.UseEntityFrameworkCore()
                    .UseDbContext<DbContext>();
            })

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
                        options.SetClientId("Ov23lidCCKma1TXEXzee")
                            .SetClientSecret("7fb2f1169ccba051fa51426d3dcb8672bec1e64c")
                            .SetRedirectUri("callback/login/github");
                    });
            })
            // Register the OpenIddict validation components.
            .AddValidation(options =>
            {
                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            });
        
        

        return services;
    }


    public static IServiceCollection AddPantryAuthenticationFull(this IServiceCollection services)
    {
        services.AddDbContext<DbContext>(options =>
        {
            options.UseSqlite($"Filename={Path.Combine(Path.GetTempPath(), "openiddict-mimban-server.sqlite3")}");
            options.UseOpenIddict();
        });

        services.AddOpenIddict()

            // Register the OpenIddict core components.
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<DbContext>();
            })

            // Register the OpenIddict client components.
            .AddClient(options =>
            {
                // Note: this sample uses the code flow, but you can enable the other flows if necessary.
                options.AllowAuthorizationCodeFlow();

                // Register the signing and encryption credentials used to protect
                // sensitive data like the state tokens produced by OpenIddict.
                options.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                options.UseAspNetCore()
                    .EnableRedirectionEndpointPassthrough();

                // Register the System.Net.Http integration and use the identity of the current
                // assembly as a more specific user agent, which can be useful when dealing with
                // providers that use the user agent as a way to throttle requests (e.g Reddit).
                options.UseSystemNetHttp()
                    .SetProductInformation(typeof(Program).Assembly);

                // Register the Web providers integrations.
                //
                // Note: to mitigate mix-up attacks, it's recommended to use a unique redirection endpoint
                // URI per provider, unless all the registered providers support returning a special "iss"
                // parameter containing their URL as part of authorization responses. For more information,
                // see https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics#section-4.4.
                options.UseWebProviders()
                    .AddGitHub(options =>
                    {
                        options.SetClientId("Ov23lidCCKma1TXEXzee")
                            .SetClientSecret("7fb2f1169ccba051fa51426d3dcb8672bec1e64c")
                            .SetRedirectUri("callback/login/github");
                    });
            })

            // Register the OpenIddict server components.
            .AddServer(options =>
            {
                // Enable the authorization and token endpoints.
                options.SetAuthorizationEndpointUris("authorize")
                    .SetTokenEndpointUris("token");

                // Note: this sample only uses the authorization code flow but you can enable
                // the other flows if you need to support implicit, password or client credentials.
                options.AllowAuthorizationCodeFlow();

                // Register the signing and encryption credentials.
                options.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                //
                // Note: unlike other samples, this sample doesn't use token endpoint pass-through
                // to handle token requests in a custom MVC action. As such, the token requests
                // will be automatically handled by OpenIddict, that will reuse the identity
                // resolved from the authorization code to produce access and identity tokens.
                //
                options.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough();
            })

            // Register the OpenIddict validation components.
            .AddValidation(options =>
            {
                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            });

        services.AddAuthorization()
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        
            .AddCookie("OpenIddict.Server.AspNetCore");


        return services;
    }
}