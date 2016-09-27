// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionListener.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IConnectionListener&lt;TConnection, TSettings, TAddress&gt; interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices
{
    using System;


    public interface IConnectionListener<TConnection, TSettings, TAddress>
        : IDisposable
        where TConnection : IConnection<TAddress>
        where TAddress : IAddressInfo
    {

        event EventHandler<ConnectionReceivedEventArgs<TConnection, TAddress>> ConnectionReceived;


        bool IsListening { get; }


        void StartListening(TSettings settings);


        void StopListening();
    }
}
