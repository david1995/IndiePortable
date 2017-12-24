// <copyright file="StreamMessageDecorator{TMessage}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using IndiePortable.Communication.Binary;
using IndiePortable.Communication.Message;

namespace IndiePortable.Communication.BinaryWrappers
{
    /// <summary>
    /// Represents a decorator around a <see cref="StreamConnection"/>, and operates as a message-oriented connection.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <seealso cref="DuplexMessageConnection{TMessage}" />
    public abstract class StreamMessageDecorator<TMessage>
        : DuplexMessageConnection<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamMessageDecorator{TMessage}"/> class.
        /// </summary>
        /// <param name="decoratedConnection">The decorated connection.</param>
        /// <exception cref="ArgumentNullException"><paramref name="decoratedConnection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <list type="bullet">
        /// <item><paramref name="decoratedConnection"/> has already been activated.</item>
        /// <item><paramref name="decoratedConnection"/> is not connected to a remote end point.</item>
        /// </list>
        /// </exception>
        protected StreamMessageDecorator(StreamConnection decoratedConnection)
        {
            // TODO: test ResourceManager
            this.DecoratedConnection = decoratedConnection is null
                                     ? throw new ArgumentNullException(nameof(decoratedConnection))
                                     : decoratedConnection.IsActivated
                                     ? throw new ArgumentException("The decorated connection must not be activated.", nameof(decoratedConnection))
                                     : !decoratedConnection.IsConnected
                                     ? throw new ArgumentException("The decorated connection must be connected.", nameof(decoratedConnection))
                                     : decoratedConnection;
        }

        /// <summary>
        /// Gets the decorated connection.
        /// </summary>
        /// <value>
        /// The decorated connection.
        /// </value>
        public StreamConnection DecoratedConnection { get; }

        /// <inheritdoc/>
        protected override void ActivateOverride() => this.DecoratedConnection.Activate();

        /// <inheritdoc/>
        protected override async Task DisconnectAsyncOverride() => await this.DecoratedConnection.DisconnectAsync();

        /// <inheritdoc/>
        protected override void DisconnectOverride() => this.DecoratedConnection.Disconnect();
    }
}
