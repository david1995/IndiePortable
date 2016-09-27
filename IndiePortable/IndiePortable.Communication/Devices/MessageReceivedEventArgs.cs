// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageReceivedEventArgs.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the MessageReceivedEventArgs class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices
{
    using System;
    using Messages;

    /// <summary>
    /// Provides information about a received message.
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class MessageReceivedEventArgs
        : EventArgs
    {
        /// <summary>
        /// The backing field for the <see cref="ReceivedMessage" /> property.
        /// </summary>
        private readonly MessageBase receivedMessageBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs" /> class.
        /// </summary>
        /// <param name="receivedMessage">
        ///     The <see cref="MessageBase" /> that has been received.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="receivedMessage" /> is <c>null</c>.</para>
        /// </exception>
        public MessageReceivedEventArgs(MessageBase receivedMessage)
            : base()
        {
            if (object.ReferenceEquals(receivedMessage, null))
            {
                throw new ArgumentNullException(nameof(receivedMessage));
            }

            this.receivedMessageBacking = receivedMessage;
        }

        /// <summary>
        /// Gets the <see cref="MessageBase" /> that has been received.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="MessageBase" /> that has been received.
        /// </value>
        public MessageBase ReceivedMessage => this.receivedMessageBacking;
    }
}
