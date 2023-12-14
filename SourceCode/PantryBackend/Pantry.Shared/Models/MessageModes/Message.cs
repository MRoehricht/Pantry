namespace Pantry.Shared.Models.MessageModes {
    public class Message<T> {
        public MessageType Type { get; set; }
        public T Content { get; set; }
    }
}
