using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Pantry.Module.Recipe.Database.Contexts;
using Pantry.Module.Recipe.Extensions;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Pantry.Module.Recipe.IntegrationTests.Infrastructure;

public sealed class RecipeIntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .WithDatabase("pantry")
        .WithUsername("pantry")
        .WithPassword("pantry")
        .Build();

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:3.13.7-alpine")
        .WithUsername("pantry")
        .WithPassword("pantry")
        .WithPortBinding(5672, false)
        .Build();

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();

        var connection = new NpgsqlConnection(_postgresContainer.GetConnectionString());
        var exeFilePath = Assembly.GetExecutingAssembly().Location;
        var index = exeFilePath.IndexOf($"{Path.DirectorySeparatorChar}PantryBackend", StringComparison.Ordinal);
        var path = exeFilePath.Substring(0, index);
        var script = await File.ReadAllTextAsync(path +
                                                 $"{Path.DirectorySeparatorChar}PantryBackend{Path.DirectorySeparatorChar}script.sql");
        var command = new NpgsqlCommand(script, connection);
        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
        await connection.CloseAsync();
    }

    public new async Task DisposeAsync()
    {
        await _rabbitMqContainer.DisposeAsync().AsTask();
        await _postgresContainer.DisposeAsync().AsTask();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor =
                services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<RecipeContext>));

            if (descriptor is not null) services.Remove(descriptor);

            services.AddDbContext<RecipeContext>(options =>
            {
                options.UseNpgsql(_postgresContainer.GetConnectionString());
            });
        });
    }
}