using Arcus.EventGrid.Contracts;
using Arcus.EventGrid.IoTHub.Contracts.Events.v1.Data;
using Newtonsoft.Json;

namespace Arcus.EventGrid.IoTHub.Contracts.Events.v1
{
    public class DeviceCreated : Event<IoTDeviceEventData>
    {
        [JsonConstructor]
        public DeviceCreated(string id) : this(id, null)
        {
        }

        public DeviceCreated(string id, string subject) : base(id, subject)
        {
        }

        public override string DataVersion { get; } = "1";
        public override string EventType { get;  } = "Microsoft.Devices.DeviceCreated";
    }
}