using System;
using System.Threading.Tasks;
using IndiePortable.Communication.Core;

namespace IndiePortable.Communication.Binary
{
    public interface IStreamConnectionClient<out TConnection, TAddress>
        : IDisposable
        where TConnection : IStreamConnection
    {

        event Action<IStreamConnectionClient<TConnection, TAddress>> ClientStateChanged;

        event Action<IStreamConnectionClient<TConnection, TAddress>> Disconnected;

        TConnection Connection { get; }

        ConnectionClientState ClientState { get; }

        TAddress Address { get; }

        void Initialize();

        void ConnectTo(TAddress targetAddress);

        bool TryConnectTo(TAddress targetAddress);

        void Disconnect();

        Task DisconnectAsync();
    }
}
