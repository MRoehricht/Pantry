using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pantry.Shared.Attribute.MessageAttributes {
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class DestinationQueueAttribute : System.Attribute{
   
        public string DestinationQueue { get; set; }

        public DestinationQueueAttribute(string destinationQueue) {
            DestinationQueue = destinationQueue;
        }
    }
}
