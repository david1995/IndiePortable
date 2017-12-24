using System;

namespace IndiePortable.Communication.Binary
{
    public class InMemoryStreamConnectionClient
        : StreamConnectionClient<InMemoryStreamConnection, int>
    {
        public InMemoryStreamConnectionClient(int address)
        {
            this.Address = address;
        }

        public override int Address { get; }

        protected override void DisconnectOverride()
        {
            throw new NotImplementedException();
        }

        protected override void InitializeOverride()
        {
        }

        protected override (bool Success, InMemoryStreamConnection Result) ConnectOverride(int address)
        {
            var (write, read) = InMemoryStreamConnectionListener.ConnectTo(this, address);
            return (true, new InMemoryStreamConnection(read, write));
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();
            this.Connection.Disconnect();
        }
    }
}
