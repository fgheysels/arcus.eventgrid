﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Arcus.EventGrid.Contracts;
using Arcus.EventGrid.Contracts.Interfaces;
using Arcus.EventGrid.Publishing.Interfaces;
using Flurl.Http;
using GuardNet;
using Polly;

namespace Arcus.EventGrid.Publishing
{
    /// <summary>
    ///     Event Grid publisher can be used to publish events to a custom Event Grid topic
    /// </summary>
    public class EventGridPublisher : IEventGridPublisher
    {
        private readonly Policy _resilientPolicy;
        private readonly string _authenticationKey;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="topicEndpoint">Url of the custom Event Grid topic</param>
        /// <param name="authenticationKey">Authentication key for the custom Event Grid topic</param>
        /// <exception cref="UriFormatException">The topic endpoint must be a HTTP endpoint.</exception>
        internal EventGridPublisher(Uri topicEndpoint, string authenticationKey)
            : this(topicEndpoint, authenticationKey, Policy.NoOpAsync())
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="topicEndpoint">Url of the custom Event Grid topic</param>
        /// <param name="authenticationKey">Authentication key for the custom Event Grid topic</param>
        /// <param name="resilientPolicy">The policy to use making the publishing resilient.</param>
        /// <exception cref="UriFormatException">The topic endpoint must be a HTTP endpoint.</exception>
        internal EventGridPublisher(Uri topicEndpoint, string authenticationKey, Policy resilientPolicy)
        {
            Guard.NotNull(topicEndpoint, nameof(topicEndpoint), "The topic endpoint must be specified");
            Guard.NotNullOrWhitespace(authenticationKey, nameof(authenticationKey), "The authentication key must not be empty and is required");
            Guard.NotNull(resilientPolicy, nameof(resilientPolicy), "The resilient policy is required with this construction, otherwise use other constructor");
            Guard.For<UriFormatException>(
                () => topicEndpoint.Scheme != Uri.UriSchemeHttp
                      && topicEndpoint.Scheme != Uri.UriSchemeHttps,
                $"The topic endpoint must be and HTTP or HTTPS endpoint but is: {topicEndpoint.Scheme}");

            TopicEndpoint = topicEndpoint.OriginalString;

            _authenticationKey = authenticationKey;
            _resilientPolicy = resilientPolicy;
        }

        /// <summary>
        ///     Url of the custom Event Grid topic
        /// </summary>
        public string TopicEndpoint { get; }

        /// <summary>
        ///     Publish a raw JSON payload as event
        /// </summary>
        /// <param name="eventId">Id of the event</param>
        /// <param name="eventType">Type of the event</param>
        /// <param name="eventBody">Body of the event</param>
        public async Task PublishRaw(string eventId, string eventType, string eventBody)
        {
            await PublishRaw(eventId, eventType, eventBody, eventSubject: "/", dataVersion: "1.0", eventTime: DateTimeOffset.UtcNow);
        }

        /// <summary>
        ///     Publish a raw JSON payload as event
        /// </summary>
        /// <param name="eventId">Id of the event</param>
        /// <param name="eventType">Type of the event</param>
        /// <param name="eventBody">Body of the event</param>
        /// <param name="eventSubject">Subject of the event</param>
        /// <param name="dataVersion">Data version of the event body</param>
        /// <param name="eventTime">Time when the event occured</param>
        public async Task PublishRaw(string eventId, string eventType, string eventBody, string eventSubject, string dataVersion, DateTimeOffset eventTime)
        {
            Guard.NotNullOrWhitespace(eventId, nameof(eventId), "No event id was specified");
            Guard.NotNullOrWhitespace(eventType, nameof(eventType), "No event type was specified");
            Guard.NotNullOrWhitespace(eventBody, nameof(eventBody), "No event body was specified");
            Guard.NotNullOrWhitespace(dataVersion, nameof(dataVersion), "No data version body was specified");
            Guard.For<ArgumentException>(() => eventBody.IsValidJson() == false, "The event body is not a valid JSON payload");

            var rawEvent = new RawEvent(eventId, eventType, eventBody, eventSubject, dataVersion, eventTime);

            IEnumerable<RawEvent> eventList = new List<RawEvent>
            {
                rawEvent
            };

            await PublishEventToTopic(eventList);
        }

        /// <summary>
        ///     Publish an event grid message
        /// </summary>
        /// <typeparam name="TEvent">Type of the specific EventData</typeparam>
        /// <param name="event">Event to publish</param>
        public async Task Publish<TEvent>(TEvent @event)
            where TEvent : class, IEvent, new()
        {
            Guard.NotNull(@event, nameof(@event), "No event was specified");

            IEnumerable<TEvent> eventList = new List<TEvent>
            {
                @event
            };

            await PublishEventToTopic(eventList);
        }

        /// <summary>
        ///     Publish an event grid message
        /// </summary>
        /// <typeparam name="TEvent">Type of the specific EventData</typeparam>
        /// <param name="events">Events to publish</param>
        public async Task PublishMany<TEvent>(IEnumerable<TEvent> events)
            where TEvent : class, IEvent, new()
        {
            Guard.NotNull(events, nameof(events), "No events was specified");
            Guard.For(() => events.Any() == false, new ArgumentException("No events were specified", nameof(events)));

            await PublishEventToTopic(events);
        }

        private async Task PublishEventToTopic<TEvent>(IEnumerable<TEvent> eventList) where TEvent : class, IEvent, new()
        {
            // Calling HTTP endpoint
            var response = await _resilientPolicy.ExecuteAsync(() => SendAuthorizedHttpPostRequest(eventList));

            if (!response.IsSuccessStatusCode)
            {
                await ThrowApplicationExceptionAsync(response);
            }
        }

        private Task<HttpResponseMessage> SendAuthorizedHttpPostRequest<TEvent>(IEnumerable<TEvent> events) where TEvent : class, IEvent, new()
        {
            return TopicEndpoint.WithHeader(name: "aeg-sas-key", value: _authenticationKey)
                .PostJsonAsync(events);
        }

        private async Task ThrowApplicationExceptionAsync(HttpResponseMessage response)
        {
            var rawResponse = string.Empty;

            try
            {
                rawResponse = await response.Content.ReadAsStringAsync();
            }
            finally
            {
                // Throw custom exception in case of failure
                throw new ApplicationException($"Event grid publishing failed with status {response.StatusCode} and content {rawResponse}");
            }
        }
    }
}