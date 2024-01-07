
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Pantry.Api.Configuration;
using Pantry.Api.Database.Contexts;
using Pantry.Api.Endpoints;
using Pantry.Api.Metrics;
using Pantry.Services.Database;
using Pantry.Services.RabbitMqServices.DependencyInjection;
using Pantry.Services.UserServices;

namespace Pantry.Api;

public class Program
{
    const string PANTRY_ORIGINS = "pantry";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<PantryContext>(optionsAction =>
        {
            optionsAction.UseNpgsql(DatabaseConfigurationManager.CreateDatabaseConfiguration(builder.Configuration).GetConnectionString());
        });

        // Add services to the container.
        builder.Services.AddAuthorization();

        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.SetInMemorySagaRepositoryProvider();

            var assembly = typeof(Program).Assembly;

            x.AddConsumers(assembly);
            x.AddSagaStateMachines(assembly);
            x.AddSagas(assembly);
            x.AddActivities(assembly);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
                {
                    h.Username(builder.Configuration["RabbitMQ:User"]);
                    h.Password(builder.Configuration["RabbitMQ:Password"]);
                });
                cfg.ConfigureEndpoints(context);
            });
        });


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(conf =>
        {
            conf.AddSecurityDefinition("UserEMail", new OpenApiSecurityScheme
            {
                Description = "EMail of the user",
                Name = "UserEMail",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "UserEMailScheme",
            });
            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "UserEMail"
                },
                In = ParameterLocation.Header,
            };
            var requirement = new OpenApiSecurityRequirement
            {
                { scheme, Array.Empty<string>() }
            };
            conf.AddSecurityRequirement(requirement);
        });

        builder.Services.AddLogging();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<IHeaderEMailService, HeaderEMailService>();

        builder.Services.AddOpenTelemetry().ConfigureResource((resourceBuilder) => resourceBuilder.AddService("Pantry.API"))
        .WithMetrics(b =>
        {
            b.AddPrometheusExporter();
            //b.AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel", PantryApiMetrics.MeterName);
            b.AddMeter(PantryApiMetrics.MeterName);
            b.AddView("request-duration", new ExplicitBucketHistogramConfiguration { Boundaries = [0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2, 5, 10, 30, 60, 120, 300, 600, 1200, 1800, 3600] });
        }).WithTracing(b =>
        {
            b.AddAspNetCoreInstrumentation()
             .AddHttpClientInstrumentation()
             .AddEntityFrameworkCoreInstrumentation()
             .AddSource(DiagnosticsConfig.ActivitySource.Name)
            .AddOtlpExporter(opts =>
            {
                opts.Endpoint =
                    new Uri($"{builder.Configuration["Jaeger:Protocol"]}://{builder.Configuration["Jaeger:Host"]}:{builder.Configuration["Jaeger:Port"]}");
            });
        });

        builder.Services.AddMetrics();
        builder.Services.AddSingleton<PantryApiMetrics>();

        var allowedOrigins = builder.Configuration["ALLOWED_ORIGINS"]?.Split(',') ?? [];
        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy(name: PANTRY_ORIGINS, policyBuilder =>
            {
                policyBuilder.WithOrigins(allowedOrigins)
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddHealthChecks()
            .AddNpgSql(DatabaseConfigurationManager.CreateDatabaseConfiguration(builder.Configuration).GetConnectionString(), name: "Pantry.Api.Postgres")
            .AddRabbitMQ(RabbitMqConfigurationManager.CreateRabbitMqConfiguration(builder.Configuration).GetConnectionString(), name: "RabbitMQ");

        var app = builder.Build();
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
        using (var scope = app.Services.CreateScope())
        {
            //dotnet ef migrations add ...-Script // add-migration ...
            var recipeContext = scope.ServiceProvider.GetRequiredService<PantryContext>();
            recipeContext.Database.Migrate();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors(PANTRY_ORIGINS);
        app.UseAuthorization();
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.MapGroup("/goods").MapGoodsEndpoint();
        app.MapGroup("/goodratings").MapGoodRatingsEndpoint();
        app.MapGroup("/suggestions").MapSuggestionEndpoints();
        app.Run();
    }
}
