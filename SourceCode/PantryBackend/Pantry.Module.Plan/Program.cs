// using HealthChecks.UI.Client;
// using MassTransit;
// using MassTransit.Logging;
// using MassTransit.Monitoring;
// using Microsoft.AspNetCore.Diagnostics.HealthChecks;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.OpenApi.Any;
// using Microsoft.OpenApi.Models;
// using Npgsql;
// using OpenTelemetry.Metrics;
// using OpenTelemetry.Resources;
// using OpenTelemetry.Trace;
// using Pantry.Module.Authentication.UserServices;
// using Pantry.Module.Plan.Configuration;
// using Pantry.Module.Plan.Database.Contexts;
// using Pantry.Module.Plan.Endpoints;
// using Pantry.Module.Shared.Database;
// using Pantry.Module.Shared.RabbitMqServices.DependencyInjection;
//
// namespace Pantry.Module.Plan;
//
// public class Program
// {
//     public static void Main(string[] args)
//     {
//         var builder = WebApplication.CreateBuilder(args);
//
//         // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//         builder.Services.AddEndpointsApiExplorer();
//         builder.Services.AddSwaggerGen(conf =>
//         {
//             conf.MapType<DateOnly>(() => new OpenApiSchema
//             {
//                 Type = "string",
//                 Format = "date",
//                 Example = new OpenApiString("2023-10-31")
//             });
//
//             conf.AddSecurityDefinition("UserEMail", new OpenApiSecurityScheme
//             {
//                 Description = "EMail of the user",
//                 Name = "UserEMail",
//                 In = ParameterLocation.Header,
//                 Type = SecuritySchemeType.ApiKey,
//                 Scheme = "UserEMailScheme",
//             });
//             var scheme = new OpenApiSecurityScheme
//             {
//                 Reference = new OpenApiReference
//                 {
//                     Type = ReferenceType.SecurityScheme,
//                     Id = "UserEMail"
//                 },
//                 In = ParameterLocation.Header,
//             };
//             var requirement = new OpenApiSecurityRequirement
//             {
//                 { scheme, Array.Empty<string>() }
//             };
//             conf.AddSecurityRequirement(requirement);
//         });
//
//         builder.Services.AddDbContext<PlanContext>(optionsAction =>
//         {
//             optionsAction.UseNpgsql(DatabaseConfigurationManager.CreateDatabaseConfiguration(builder.Configuration).GetConnectionString());
//         });
//
//
//         builder.Services.AddMassTransit(x =>
//         {
//             x.SetKebabCaseEndpointNameFormatter();
//             x.SetInMemorySagaRepositoryProvider();
//
//             var assembly = typeof(Program).Assembly;
//
//             x.AddConsumers(assembly);
//             x.AddSagaStateMachines(assembly);
//             x.AddSagas(assembly);
//             x.AddActivities(assembly);
//
//             x.UsingRabbitMq((context, cfg) =>
//             {
//                 cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
//                 {
//                     h.Username(builder.Configuration["RabbitMQ:User"]);
//                     h.Password(builder.Configuration["RabbitMQ:Password"]);
//                 });
//                 cfg.ConfigureEndpoints(context);
//             });
//         });
//
//         builder.Services.AddHttpContextAccessor();
//         builder.Services.AddTransient<IHeaderEMailService, HeaderEMailService>();
//
//         builder.Services.AddOpenTelemetry().ConfigureResource(
//             (resourceBuilder) => resourceBuilder.AddService("Pantry.Plan.API"))
//         .WithMetrics(b =>
//         {
//             b.AddPrometheusExporter();
//             b.AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel");
//             b.AddMeter(InstrumentationOptions.MeterName);
//         }).WithTracing(b =>
//         {
//             b.AddAspNetCoreInstrumentation()
//              .AddHttpClientInstrumentation()
//              .AddEntityFrameworkCoreInstrumentation()
//              .AddNpgsql()
//              .AddSource(DiagnosticsConfig.ActivitySource.Name)
//              .AddSource(DiagnosticHeaders.DefaultListenerName)
//             .AddOtlpExporter(opts =>
//             {
//                 opts.Endpoint =
//                     new Uri($"{builder.Configuration["Jaeger:Protocol"]}://{builder.Configuration["Jaeger:Host"]}:{builder.Configuration["Jaeger:Port"]}");
//             });
//         });
//
//         builder.Services.AddMetrics();
//
//
//         builder.Services.AddHealthChecks()
//             .AddNpgSql(DatabaseConfigurationManager.CreateDatabaseConfiguration(builder.Configuration).GetConnectionString(), name: "Pantry.Module.Plan.Postgres");
//             //.AddRabbitMQ(RabbitMqConfigurationManager.CreateRabbitMqConfiguration(builder.Configuration).GetConnectionString(), name: "RabbitMQ");
//
//         var app = builder.Build();
//         app.UseOpenTelemetryPrometheusScrapingEndpoint();
//         using (var scope = app.Services.CreateScope())
//         {
//             //dotnet ef migrations add ...-Script // add-migration ...
//             var recipeContext = scope.ServiceProvider.GetRequiredService<PlanContext>();
//             recipeContext.Database.Migrate();
//         }
//
//         // Configure the HTTP request pipeline.
//         if (app.Environment.IsDevelopment())
//         {
//             app.UseSwagger();
//             app.UseSwaggerUI();
//         }
//
//         app.UseHealthChecks("/health", new HealthCheckOptions
//         {
//             ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
//         });
//         app.MapGroup("/meals").MapMealsEndpoint();
//
//         app.Run();
//     }
// }
