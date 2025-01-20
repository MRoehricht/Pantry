namespace Pantry.Module.Shared.RabbitMqServices.Model;
public class RabbitMqConfiguration
{
    public required string Host { get; set; }
    public required string Port { get; set; }
    public required string User { get; set; }
    public required string Password { get; set; }

    public string GetConnectionString()
    {
        return $"amqp://{User}:{Password}@{Host}:{Port}";
    }
}
