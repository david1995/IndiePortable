// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageReceivedEventArgs{TMessage}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the MessageReceivedEventArgs class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices
{
    using System;

    /// <summary>
    /// Provides information about a received message.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of the received message.
    /// Must be a reference type.
    /// </typeparam>
    /// <seealso cref="EventArgs" />
    public class MessageReceivedEventArgs<TMessage>
        : EventArgs
        where TMessage : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs{TMessage}" /> class.
        /// </summary>
        /// <param name="receivedMessage">
        /// The <see cref="MessageBase" /> that has been received.
        /// Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="receivedMessage" /> is <c>null</c>.</para>
        /// </exception>
        public MessageReceivedEventArgs(TMessage receivedMessage)
            : base()
        {
            this.ReceivedMessage = receivedMessage ?? throw new ArgumentNullException(nameof(receivedMessage));
        }

        /// <summary>
        /// Gets the <see cref="TMessage" /> that has been received.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="TMessage" /> that has been received.
        /// </value>
        public TMessage ReceivedMessage { get; }
    }
}
