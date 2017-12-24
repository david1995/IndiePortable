// <copyright file="IMessageConnectionListener{TSend,TReceive}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;

namespace IndiePortable.Communication.Message
{
    public interface IMessageConnectionListener<TSend, TReceive>
        : IDisposable
        where TSend : class
        where TReceive : class
    {
        event Action<DuplexMessageConnection<TSend, TReceive>> ConnectionReceived;

        void StartListening();

        void StopListening();
    }
}
