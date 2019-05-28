using Arcus.EventGrid.Contracts;
using Arcus.EventGrid.Tests.Core.Events.Data;
using GuardNet;
using Newtonsoft.Json;

namespace Arcus.EventGrid.Tests.Core.Events
{
    public class NewCarRegistered : Event<CarEventData>
    {
        [JsonConstructor]
        public NewCarRegistered(string id) : base(id)
        { }

        public NewCarRegistered(string id, string licensePlate) : base(id)
        {
            Guard.NotNullOrWhitespace(licensePlate, nameof(licensePlate));

            Data.LicensePlate = licensePlate;
        }

        public NewCarRegistered(string id, string subject, string licensePlate) : base(id, subject)
        {
            Guard.NotNullOrWhitespace(licensePlate, nameof(licensePlate));

            Data.LicensePlate = licensePlate;
        }

        public override string DataVersion { get; } = "1";
        public override string EventType { get; } = "Arcus.Samples.Cars.NewCarRegistered";
    }
}