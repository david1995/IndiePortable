// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionListener.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IConnectionListener&lt;TConnection, TSettings, TAddress&gt; interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices
{
    using System;

    /// <summary>
    /// Represents an object listening for protocol-based message connections.
    /// </summary>
    /// <typeparam name="TConnection">
    ///     The type of the returned connection objects.
    ///     Must derive from <see cref="IConnection{TAddress}" />.
    /// </typeparam>
    /// <typeparam name="TSettings">
    ///     The type of the listener settings.
    /// </typeparam>
    /// <typeparam name="TAddress">
    ///     The type of the addresses.
    ///     Must derive from <see cref="IAddressInfo" />.
    /// </typeparam>
    /// <seealso cref="IDisposable" />
    public interface IConnectionListener<TConnection, TSettings, TAddress>
        : IDisposable
        where TConnection : IConnection<TAddress>
        where TAddress : IAddressInfo
    {
        /// <summary>
        /// Raised when a connection has been received.
        /// </summary>
        event EventHandler<ConnectionReceivedEventArgs<TConnection, TAddress>> ConnectionReceived;

        /// <summary>
        /// Gets a value indicating whether the <see cref="IConnectionListener{TConnection, TSettings, TAddress}" /> is actively listening for connections.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="IConnectionListener{TConnection, TSettings, TAddress}" />
        ///     is actively listening for connections; otherwise, <c>false</c>.
        /// </value>
        bool IsListening { get; }

        /// <summary>
        /// Starts listening for connections.
        /// </summary>
        /// <param name="settings">
        ///     The settings specifying parameters for the <see cref="IConnectionListener{TConnection, TSettings, TAddress}" />.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the <see cref="IConnectionListener{TConnection, TSettings, TAddress}" /> is already listening.</para>
        /// </exception>
        void StartListening(TSettings settings);

        /// <summary>
        /// Stops listening for connections.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the <see cref="IConnectionListener{TConnection, TSettings, TAddress}" /> is not listening.</para>
        /// </exception>
        void StopListening();
    }
}
