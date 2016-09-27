// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionReceivedEventArgs.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionReceivedEventArgs&lt;TConnection, TAddress&gt; class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices
{
    using System;

    /// <summary>
    /// Provides information for a "connection received" event.
    /// </summary>
    /// <typeparam name="TConnection">
    ///     The type of the connection.
    ///     Must implement <see cref="IConnection{TAddress}" />.
    /// </typeparam>
    /// <typeparam name="TAddress">
    ///     The type of the address.
    ///     Must implement <see cref="IAddressInfo" />.
    /// </typeparam>
    public sealed class ConnectionReceivedEventArgs<TConnection, TAddress>
        where TConnection : IConnection<TAddress>
        where TAddress : IAddressInfo
    {
        /// <summary>
        /// The backing field for the <see cref="ReceivedConnection" /> property.
        /// </summary>
        private readonly TConnection receivedConnectionBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionReceivedEventArgs{TConnection, TAddress}" /> class.
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
