using System;

namespace KestrelShark
{
    internal interface IConnectionTracingEventSource
    {
        void ReceivedData(string connectionId, ReadOnlySpan<byte> data);
        void SentData(string connectionId, ReadOnlySpan<byte> data);
        void StartConnection(string connectionId);
        void EndConnection(string connectionId);
    }
}
