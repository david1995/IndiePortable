// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpMessageReceivedEventArgs.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the TcpMessageReceivedEventArgs class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Tcp
{
    using System;

    /// <summary>
    /// Provides data for an event concerning <see cref="TcpConnectionMessage" /> instances.
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class TcpMessageReceivedEventArgs
        : EventArgs
    {
        /// <summary>
        /// The backing field for the <see cref="Message" /> property.
        /// </summary>
        private readonly TcpConnectionMessage messageBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpMessageReceivedEventArgs" /> class.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="TcpConnectionMessage" /> that has been received.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="message" /> is <c>null</c>.</para>
        /// </exception>
        public TcpMessageReceivedEventArgs(TcpConnectionMessage message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.messageBacking = message;
        }

        /// <summary>
        /// Gets the <see cref="TcpConnectionMessage" /> that has been received.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="TcpConnectionMessage" /> that has been received.
        /// </value>
        public TcpConnectionMessage Message => this.messageBacking;
    }
}
