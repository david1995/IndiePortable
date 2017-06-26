// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionReceivedEventArgs.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionReceivedEventArgs&lt;TConnection, TAddress&gt; class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices.Connections
{
    using System;

    /// <summary>
    /// Provides information for a "connection received" event.
    /// </summary>
    /// <typeparam name="TConnection">
    /// The type of the connection.
    /// Must derive from <see cref="MessageConnectionBase{TMessage, TAddress}" />.
    /// </typeparam>
    /// <typeparam name="TMessage">
    /// The type of the connection's messages.
    /// Must be a reference type.
    /// </typeparam>
    /// <typeparam name="TAddress">
    /// The type of the address.
    /// </typeparam>
    public sealed class ConnectionReceivedEventArgs<TConnection, TMessage, TAddress>
        where TConnection : MessageConnectionBase<TMessage, TAddress>
        where TMessage : class
    {
        /// <summary>
        /// The backing field for the <see cref="ReceivedConnection" /> property.
        /// </summary>
        private readonly TConnection receivedConnectionBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionReceivedEventArgs{TConnection, TMessage, TAddress}" /> class.
        /// </summary>
        /// <param name="receivedConnection">
        ///     The <typeparamref name="TConnection" /> that has been received.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="receivedConnection" /> is <c>null</c>.</para>
        /// </exception>
        public ConnectionReceivedEventArgs(TConnection receivedConnection)
        {
            if (object.ReferenceEquals(receivedConnection, null))
            {
                throw new ArgumentNullException(nameof(receivedConnection));
            }

            this.receivedConnectionBacking = receivedConnection;
        }

        /// <summary>
        /// Gets the connection that has been received.
        /// </summary>
        /// <value>
        ///     Contains the connection that has been received.
        /// </value>
        public TConnection ReceivedConnection => this.receivedConnectionBacking;
    }
}
