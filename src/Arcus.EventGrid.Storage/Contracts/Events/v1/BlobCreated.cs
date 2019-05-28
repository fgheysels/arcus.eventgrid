using Arcus.EventGrid.Contracts;
using Arcus.EventGrid.Storage.Contracts.Events.v1.Data;
using Newtonsoft.Json;

namespace Arcus.EventGrid.Storage.Contracts.Events.v1
{
    public class BlobCreated : Event<BlobEventData>
    {
        [JsonConstructor]
        public BlobCreated(string id) : this(id, null)
        {
        }

        public BlobCreated(string id, string subject) : base(id, subject)
        {
        }

        public override string DataVersion { get; } = "1";
        public override string EventType { get; } = "Microsoft.Storage.BlobCreated";
    }
}