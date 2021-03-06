﻿using Arcus.EventGrid.Contracts;
using Arcus.EventGrid.Security.Contracts.Events.v1.Data;

namespace Arcus.EventGrid.Security.Contracts.Events.v1
{
    public class SubscriptionValidation : Event<SubscriptionEventData>
    {
        public SubscriptionValidation()
        {
        }

        public SubscriptionValidation(string id) : base(id)
        {
        }

        public SubscriptionValidation(string id, string subject) : base(id, subject)
        {
        }

        public override string DataVersion { get;  } = "1";
        public override string EventType { get;  } = "Microsoft.EventGrid.SubscriptionValidationEvent";
    }
}