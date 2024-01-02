
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using Pantry.Api.Database.Contexts;
using Pantry.Api.Endpoints;
using Pantry.Api.Metrics;
using Pantry.Recipe.Api.Configuration;
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
            var postgresHost = builder.Configuration["DB_HOST"];
            var postgresPort = builder.Configuration["DB_PORT"];
            var postgresDatabase = builder.Configuration["DB_DB"];
            var postgresUser = builder.Configuration["DB_USER"];
            var postgresPassword = builder.Configuration["DB_PASSWORD"];
            optionsAction.UseNpgsql($"host={postgresHost};port={postgresPort};database={postgresDatabase};username={postgresUser};password={postgresPassword};");
        });

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddRabbitMqServices(builder.Configuration);
        builder.Services.AddAutoMapper(typeof(AutomapperConfiguratrion));

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

        builder.Services.AddOpenTelemetry().WithMetrics(b =>
        {
            b.AddPrometheusExporter();
            //b.AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel", PantryApiMetrics.MeterName);
            b.AddMeter(PantryApiMetrics.MeterName);
            b.AddView("request-duration", new ExplicitBucketHistogramConfiguration { Boundaries = [0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2, 5, 10, 30, 60, 120, 300, 600, 1200, 1800, 3600] });
        });
        builder.Services.AddMetrics();
        builder.Services.AddSingleton<PantryApiMetrics>();

        var allowedOrigins = builder.Configuration["ALLOWED_ORIGINS"]?.Split(',') ?? Array.Empty<string>();
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

        app.MapGroup("/goods").MapGoodsEndpoint();
        app.MapGroup("/goodratings").MapGoodRatingsEndpoint();
        app.MapGroup("/suggestions").MapSuggestionEndpoints();
        app.Run();
    }
}
