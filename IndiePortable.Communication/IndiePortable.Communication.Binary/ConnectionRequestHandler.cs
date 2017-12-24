using System;
using System.Collections.Generic;
using System.Text;

namespace IndiePortable.Communication.Binary
{
    public delegate void ConnectionRequestHandler<TConnection, TAddress>(
        IStreamConnectionListener<TConnection, TAddress> receiver,
        IPendingStreamConnection<TConnection, TAddress> pendingConnection)
        where TConnection : IStreamConnection;
}
