using System.IO.Pipelines;
using System.Threading.Tasks;
using KestrelShark;
using Microsoft.AspNetCore.Connections;

namespace Microsoft.AspNetCore.Hosting
{
    internal class ConnectionTracingMiddleware
    {
        private readonly ConnectionDelegate _next;
        private readonly IConnectionTracingEventSource _eventSource;

        public ConnectionTracingMiddleware(ConnectionDelegate next, IConnectionTracingEventSource eventSource)
        {
            _next = next;
            _eventSource = eventSource;
        }

        public async Task OnConnectionAsync(ConnectionContext context)
        {
            // In theory, we could do some witchcraft here and NOT adapt the connection
            // by default, only actually adapting it when the event source is enabled
            // (observable via OnEventCommand in the EventSource itself)
            // That would mean that connections running prior to starting the trace wouldn't
            // be traced though.
            // But it may be closer to what we'd do if this was integrated into Kestrel itself.

            _eventSource.StartConnection(context.ConnectionId);

            var oldTransport = context.Transport;

            try
            {
                await using var tracingPipe = new TracingPipe(context.Transport, _eventSource, context.ConnectionId);
                context.Transport = tracingPipe;
                await _next(context);
            }
            finally
            {
                context.Transport = oldTransport;
                _eventSource.EndConnection(context.ConnectionId);
            }
        }

        private class TracingPipe: DuplexPipeStreamAdapter<TracingStream>
        {
            public TracingPipe(IDuplexPipe transport, IConnectionTracingEventSource eventSource, string connectionId)
                : base(transport, (stream) => new TracingStream(stream, eventSource, connectionId))
            {
            }
        }
    }
}