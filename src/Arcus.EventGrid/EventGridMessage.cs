using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arcus.EventGrid.Contracts.Interfaces;
using GuardNet;

namespace Arcus.EventGrid
{
    public class EventGridMessage<TEvent> where TEvent : IEvent, new()
    {
        /// <summary>
        /// Creates event grid message with passed session Id
        /// </summary>
        /// <param name="sessionId">Unique session id for all batched messages</param>
        /// <param name="events">List of events that are part of this Event Grid message</param>
        public EventGridMessage(string sessionId, IList<TEvent> events)
        {
            Guard.NotNullOrWhitespace(sessionId, nameof(sessionId));
            Guard.NotNull(sessionId, nameof(sessionId));

            SessionId = sessionId;
            Events = new ReadOnlyCollection<TEvent>(events);
        }

        /// <summary>
        ///     Unique session id for all batched messages
        /// </summary>
        public string SessionId { get; }

        /// <summary>
        ///     List of all events, belonging to the Event Grid message
        /// </summary>
        public ReadOnlyCollection<TEvent> Events { get; }
    }
}