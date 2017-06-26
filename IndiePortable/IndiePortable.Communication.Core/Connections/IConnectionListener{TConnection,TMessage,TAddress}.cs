// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionListener{TConnection,TMessage,TAddress}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IConnectionListener&lt;TConnection, TSettings, TAddress&gt; interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices.Connections
{
    using System;

    /// <summary>
    /// Represents an object listening for protocol-based message connections.
    /// </summary>
    /// <typeparam name="TConnection">
    /// The type of the returned connection objects.
    /// Must derive from <see cref="IConnection{TAddress}" />.
    /// </typeparam>
    /// <typeparam name="TMessage">
    /// The type of the messages.
    /// </typeparam>
    /// <typeparam name="TAddress">
    /// The type of the addresses.
    /// Must derive from <see cref="IAddressInfo" />.
    /// </typeparam>
    /// <seealso cref="IDisposable" />
    public interface IConnectionListener<TConnection, TMessage, TAddress>
        : IDisposable
        where TConnection : MessageConnectionBase<TMessage, TAddress>
        where TMessage : class
    {
        /// <summary>
        /// Occurs when a connection has been received.
        /// </summary>
        event EventHandler<ConnectionReceivedEventArgs<TConnection, TMessage, TAddress>> ConnectionReceived;

        /// <summary>
        /// Gets a value indicating whether the <see cref="IConnectionListener{TConnection, TSettings, TAddress}" /> is actively listening for connections.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="IConnectionListener{TConnection, TSettings, TAddress}" />
        /// is actively listening for connections; otherwise, <c>false</c>.
        /// </value>
        bool IsListening { get; }

        /// <summary>
        /// Starts listening for connections.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para>The <see cref="IConnectionListener{TConnection, TMessage, TAddress}" /> is already listening.</para>
        /// </exception>
        void StartListening();

        /// <summary>
        /// Stops listening for connections.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para>The <see cref="IConnectionListener{TConnection, TMessage, TAddress}" /> is not listening.</para>
        /// </exception>
        void StopListening();
    }
}
