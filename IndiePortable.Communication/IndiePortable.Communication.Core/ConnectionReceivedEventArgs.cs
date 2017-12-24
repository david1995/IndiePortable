// <copyright file="ConnectionReceivedEventArgs.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core
{
    using System;

    /// <summary>
    /// Provides information for a "connection received" event.
    /// </summary>
    /// <typeparam name="TConnection">
    /// The type of the connection.
    /// Must be a class.
    /// </typeparam>
    public sealed class ConnectionReceivedEventArgs<TConnection>
        where TConnection : class
    {
        /// <summary>
        /// The backing field for the <see cref="ReceivedConnection" /> property.
        /// </summary>
        private readonly TConnection receivedConnectionBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionReceivedEventArgs{TConnection}" /> class.
        /// </summary>
        /// <param name="receivedConnection">
        /// The <typeparamref name="TConnection" /> that has been received.
        /// Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="receivedConnection" /> is <c>null</c>.</para>
        /// </exception>
        public ConnectionReceivedEventArgs(TConnection receivedConnection)
        {
            this.receivedConnectionBacking = receivedConnection ?? throw new ArgumentNullException(nameof(receivedConnection));
        }

        /// <summary>
        /// Gets the connection that has been received.
        /// </summary>
        /// <value>
        /// Contains the connection that has been received.
        /// </value>
        public TConnection ReceivedConnection => this.receivedConnectionBacking;
    }
}
