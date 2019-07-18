using System;
using System.Diagnostics.Tracing;

namespace KestrelShark
{
    [EventSource(Name = "Microsoft-AspNetCore-NetworkTrace")]
    internal class ConnectionTracingEventSource : EventSource, IConnectionTracingEventSource
    {
        internal static readonly ConnectionTracingEventSource Log = new ConnectionTracingEventSource();

        private ConnectionTracingEventSource() : base (EventSourceSettings.ThrowOnEventWriteErrors)
        {
        }

        public static class Keywords
        {
            public const EventKeywords TransportStream = (EventKeywords)0x01;
        }

        [NonEvent]
        public void ReceivedData(string connectionId, ReadOnlySpan<byte> data)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.TransportStream))
            {
                // Gross, we have to flatten to an array until EventSource supports span...
                // TODO: We could pool.
                var arr = data.ToArray();
                ReceivedData(connectionId, arr);
            }
        }

        [NonEvent]
        public void SentData(string connectionId, ReadOnlySpan<byte> data)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.TransportStream))
            {
                // Gross, we have to flatten to an array until EventSource supports span...
                // TODO: We could pool.
                var arr = data.ToArray();
                SentData(connectionId, arr);
            }
        }

        [Event(eventId: 1, Keywords = Keywords.TransportStream, Message = "Connection '{0}' received data.", Level = EventLevel.Informational)]
        private void ReceivedData(string connectionId, byte[] data)
        {
            WriteEvent(1, connectionId, data);
        }

        [Event(eventId: 2, Keywords = Keywords.TransportStream, Message = "Connection '{0}' sent data.", Level = EventLevel.Informational)]
        private void SentData(string connectionId, byte[] data)
        {
            WriteEvent(2, connectionId, data);
        }

        [Event(eventId: 3, Message = "Connection '{0}' started.", Level = EventLevel.Informational)]
        private void StartConnection(string connectionId)
        {
            WriteEvent(3, connectionId);
        }

        [Event(eventId: 4, Message = "Connection '{0}' ended.", Level = EventLevel.Informational)]
        private void EndConnection(string connectionId)
        {
            WriteEvent(4, connectionId);
        }

        [NonEvent]
        void IConnectionTracingEventSource.StartConnection(string connectionId)
        {
            if(IsEnabled())
            {
                StartConnection(connectionId);
            }
        }

        void IConnectionTracingEventSource.EndConnection(string connectionId)
        {
            if(IsEnabled())
            {
                EndConnection(connectionId);
            }
        }
    }
}
