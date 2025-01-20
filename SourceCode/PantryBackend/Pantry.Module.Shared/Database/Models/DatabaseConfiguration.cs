namespace Pantry.Module.Shared.Database.Models;
public class DatabaseConfiguration
{
    public required string Host { get; set; }
    public required string Port { get; set; }
    public required string Database { get; set; }
    public required string User { get; set; }
    public required string Password { get; set; }

    public string GetConnectionString()
    {
        return $"host={Host};port={Port};database={Database};username={User};password={Password};";
    }
}
