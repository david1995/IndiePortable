// <copyright file="IStreamConnectionListener{TConnection,TAddress}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;

namespace IndiePortable.Communication.Binary
{
    public interface IStreamConnectionListener<TConnection, TAddress>
        : IDisposable
        where TConnection : IStreamConnection
    {
        event ConnectionRequestHandler<TConnection, TAddress> ConnectionRequested;

        TAddress LocalAddress { get; }

        bool IsListening { get; }

        void StartListening();

        void StopListening();
    }
}
