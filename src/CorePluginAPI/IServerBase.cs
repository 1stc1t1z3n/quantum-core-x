using System.Net;
using QuantumCore.API.Core.Timekeeping;
using QuantumCore.Networking;

namespace QuantumCore.API;

public interface IServerBase
{
    Task RemoveConnection(IConnection connection);
    Task CallListener(IConnection connection, IPacketSerializable packet);
    ServerClock Clock { get; }
    IPAddress IpAddress { get; }
    IPAddress AdvertisedIpAddress { get; }
    ushort Port { get; }
    void CallConnectionListener(IConnection connection);
    void ForAllConnections(Action<IConnection> callback);
}
