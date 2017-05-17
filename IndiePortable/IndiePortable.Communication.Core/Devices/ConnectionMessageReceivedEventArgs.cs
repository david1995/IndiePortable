// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionMessageReceivedEventArgs.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionMessageReceivedEventArgs class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices
{
    using System;
    using ConnectionMessages;

    /// <summary>
    /// Provides data for an event concerning <see cref="ConnectionMessageBase" /> instances.
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class ConnectionMessageReceivedEventArgs
        : EventArgs
    {
        /// <summary>
        /// The backing field for the <see cref="Message" /> property.
        /// </summary>
        private readonly ConnectionMessageBase messageBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMessageReceivedEventArgs" /> class.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="ConnectionMessageBase" /> that has been received.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="message" /> is <c>null</c>.</para>
        /// </exception>
        public ConnectionMessageReceivedEventArgs(ConnectionMessageBase message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.messageBacking = message;
        }

        /// <summary>
        /// Gets the <see cref="ConnectionMessageBase" /> that has been received.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="ConnectionMessageBase" /> that has been received.
        /// </value>
        public ConnectionMessageBase Message => this.messageBacking;
    }
}
