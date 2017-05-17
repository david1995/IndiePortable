// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IConnection&lt;T&gt; interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices
{
    using System;

    /// <summary>
    /// Represents a connection that can be used to send messages.
    /// </summary>
    /// <typeparam name="TAddress">
    ///     The type of the local and remote addresses.
    /// </typeparam>
    /// <seealso cref="IMessageTransciever" />
    /// <seealso cref="IDisposable" />
    public interface IConnection<TAddress>
        : IMessageTransciever, IDisposable
    {
        /// <summary>
        /// Raised when the <see cref="IConnection{TAddress}" /> has been disconnected.
        /// </summary>
        event EventHandler Disconnected;

        /// <summary>
        /// Occurs when the <see cref="ConnectionState"/> property has been changed.
        /// </summary>
        event EventHandler ConnectionStateChanged;

        ConnectionState ConnectionState { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IConnection{TAddress}" /> is connected to the other end.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="IConnection{TAddress}" /> is connected; otherwise, <c>false</c>.
        /// </value>
        bool IsConnected { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IConnection{TAddress}" /> is activated.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="IConnection{TAddress}" /> is activated; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         Messages can only be sent or received if <see cref="IsActivated" /> is <c>true</c>.
        ///         Otherwise, an <see cref="InvalidOperationException" /> will be thrown.
        ///     </para>
        /// </remarks>
        bool IsActivated { get; }

        /// <summary>
        /// Gets the remote address of the other connection end.
        /// </summary>
        /// <value>
        ///     Contains the remote address of the other connection end.
        /// </value>
        TAddress RemoteAddress { get; }

        /// <summary>
        /// Activates the <see cref="IConnection{TAddress}" />.
        /// </summary>
        /// <remarks>
        ///     <para>Call this method to allow incoming and outgoing messages to be sent or received.</para>
        /// </remarks>
        void Activate();

        /// <summary>
        /// Disconnects the two end points.
        /// </summary>
        void Disconnect();
    }
}
