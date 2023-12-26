
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Pantry.Api.Database.Contexts;
using Pantry.Api.Endpoints;
using Pantry.Api.Services.RabbitMqConsumerServices;
using Pantry.Recipe.Api.Configuration;
using Pantry.Services.RabbitMqServices;
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
        builder.Services.AddTransient<IRabbitMqConsumerService, PantryRabbitMqConsumerService>();

        builder.Services.AddHostedService<RabbitMqConsumerBackgroundService>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<IHeaderEMailService, HeaderEMailService>();


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
