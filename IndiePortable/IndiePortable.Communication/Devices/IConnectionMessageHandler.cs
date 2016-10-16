// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionMessageHandler.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IConnectionMessageHandler interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices
{
    using System;
    using ConnectionMessages;

    /// <summary>
    /// Represents a message handler for a connection message.
    /// </summary>
    public interface IConnectionMessageHandler
    {
        /// <summary>
        /// Gets the type of the handled messages.
        /// </summary>
        /// <value>
        ///     Contains the type of the handled messages.
        /// </value>
        Type MessageType { get; }

        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="ConnectionMessageBase" /> that shall be handled.
        ///     Must not be <c>null</c>.
        ///     Must be of type <see cref="MessageType" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="message" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if <paramref name="message" /> is not of type <see cref="MessageType" />.</para>
        /// </exception>
        void HandleMessage(ConnectionMessageBase message);
    }
}
