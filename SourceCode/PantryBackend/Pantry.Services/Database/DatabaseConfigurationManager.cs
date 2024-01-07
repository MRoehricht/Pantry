using Microsoft.Extensions.Configuration;
using Pantry.Services.Database.Models;

namespace Pantry.Services.Database;
public static class DatabaseConfigurationManager
{
    public static DatabaseConfiguration CreateDatabaseConfiguration(IConfiguration configuration)
    {
        var host = configuration["DB_HOST"];
        var port = configuration["DB_PORT"];
        var user = configuration["DB_USER"];
        var password = configuration["DB_PASSWORD"];
        var database = configuration["DB_DB"];

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(database))
        {
            throw new Exception("Database configuration is not set");
        }

        var databaseConfiguration = new DatabaseConfiguration { Host = host, User = user, Password = password, Port = port, Database = database };
        return databaseConfiguration;
    }
}
