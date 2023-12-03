namespace Pantry.Services.RabbitMqServices;
public interface IRabbitMqPublisher
{
    public void SendMessage<T>(T message);
}
