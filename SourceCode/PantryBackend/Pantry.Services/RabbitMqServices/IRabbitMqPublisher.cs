using Pantry.Shared.Models.MessageModes;

namespace Pantry.Services.RabbitMqServices;
public interface IRabbitMqPublisher
{
    public void SendMessage<T>(T message, MessageType messageType = MessageType.None);
}
