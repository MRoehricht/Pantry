namespace Pantry.Module.Shared.Attribute.MessageAttributes {
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class DestinationQueueAttribute : System.Attribute{
   
        public string DestinationQueue { get; set; }

        public DestinationQueueAttribute(string destinationQueue) {
            DestinationQueue = destinationQueue;
        }
    }
}
