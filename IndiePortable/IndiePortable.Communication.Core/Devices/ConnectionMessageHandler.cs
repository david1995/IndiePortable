// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionMessageHandler.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionMessageHandler{T} class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices
{
    using System;
    using ConnectionMessages;

    /// <summary>
    /// Represents a handler for a connection message.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the connection message.
    ///     Must derive from <see cref="ConnectionMessageBase" />.
    /// </typeparam>
    /// <seealso cref="IConnectionMessageHandler" />
    public class ConnectionMessageHandler<T>
        : IConnectionMessageHandler
        where T : ConnectionMessageBase
    {
        /// <summary>
        /// The message handler executed when handling messages.
        /// </summary>
        private Action<T> messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMessageHandler{T}"/> class.
        /// </summary>
        /// <param name="messageHandler">
        ///     The method handling the message.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     <para>Thrown if <paramref name="messageHandler" /> is <c>null</c>.</para>
        /// </exception>
        public ConnectionMessageHandler(Action<T> messageHandler)
        {
            if (object.ReferenceEquals(messageHandler, null))
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }

            this.messageHandler = messageHandler;
        }

        /// <summary>
        /// Gets the type of the handled messages.
        /// </summary>
        /// <value>
        ///     Contains the type of the handled messages.
        /// </value>
        public Type MessageType => typeof(T);

        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="message">
        ///     The message that shall be handled.
        ///     Its type must be <typeparamref name="T" />.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="message" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if <paramref name="message" /> is not of type <typeparamref name="T" />.</para>
        /// </exception>
        public void HandleMessage(ConnectionMessageBase message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            var msg = message as T;
            if (object.ReferenceEquals(msg, null))
            {
                throw new ArgumentException();
            }

            this.HandleMessage(msg);
        }

        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="message">
        ///     The message of type <typeparamref name="T" /> that shall be handled.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="message" /> is <c>null</c>.</para>
        /// </exception>
        public void HandleMessage(T message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.messageHandler(message);
        }
    }
}
