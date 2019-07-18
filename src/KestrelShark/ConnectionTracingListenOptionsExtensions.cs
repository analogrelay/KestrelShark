using KestrelShark;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Microsoft.AspNetCore.Hosting
{
    public static class ConnectionTracingListenOptionsExtensions
    {
        public static ListenOptions UseConnectionTracing(this ListenOptions listenOptions) =>
            UseConnectionTracing(listenOptions, ConnectionTracingEventSource.Log);

        internal static ListenOptions UseConnectionTracing(this ListenOptions listenOptions, IConnectionTracingEventSource eventSource)
        {
            listenOptions.Use(next => new ConnectionTracingMiddleware(next, eventSource).OnConnectionAsync);
            return listenOptions;
        }
    }
}
