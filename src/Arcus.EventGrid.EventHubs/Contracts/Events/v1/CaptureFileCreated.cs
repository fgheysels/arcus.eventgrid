using Arcus.EventGrid.Contracts;
using Arcus.EventGrid.EventHubs.Contracts.Events.v1.Data;
using Newtonsoft.Json;

namespace Arcus.EventGrid.EventHubs.Contracts.Events.v1
{
    public class CaptureFileCreated : Event<EventHubCaptureEventData>
    {
        [JsonConstructor]
        public CaptureFileCreated(string id) : this(id, null)
        {
        }

        public CaptureFileCreated(string id, string subject) : base(id, subject)
        {
        }

        public override string DataVersion { get;  } = "1";
        public override string EventType { get;  } = "Microsoft.EventHub.CaptureFileCreated";
    }
}
