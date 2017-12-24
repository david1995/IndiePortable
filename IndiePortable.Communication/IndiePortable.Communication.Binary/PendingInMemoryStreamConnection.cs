using System;
using System.IO;

namespace IndiePortable.Communication.Binary
{
    internal class PendingInMemoryStreamConnection
        : IPendingStreamConnection<InMemoryStreamConnection, int>
    {
        public PendingInMemoryStreamConnection(int sourceAddress, MemoryStream inputStream, MemoryStream outputStream)
        {
            this.SourceAddress = sourceAddress;
        }

        public int SourceAddress { get; }

        public StreamConnectionClient<InMemoryStreamConnection, int> Accept()
        {
            throw new NotImplementedException();
        }

        public void Deny()
        {
            throw new NotImplementedException();
        }
    }
}
