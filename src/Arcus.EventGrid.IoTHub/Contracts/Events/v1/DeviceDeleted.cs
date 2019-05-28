using Arcus.EventGrid.Contracts;
using Arcus.EventGrid.IoTHub.Contracts.Events.v1.Data;
using Newtonsoft.Json;

namespace Arcus.EventGrid.IoTHub.Contracts.Events.v1
{
    public class DeviceDeleted : Event<IoTDeviceEventData>
    {
        [JsonConstructor]
        public DeviceDeleted(string id) : this(id, null)
        {
        }

        public DeviceDeleted(string id, string subject) : base(id, subject)
        {
        }

        public override string DataVersion { get; } = "1";
        public override string EventType { get; } = "Microsoft.Devices.DeviceDeleted";
    }
}