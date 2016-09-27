// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandler.Generic.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IMessageHandler{T} interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices
{
    using Messages;

    /// <summary>
    /// Encapsulates a message handler callback for a message of a specific type.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the message.
    ///     Must derive from <see cref="MessageBase" />.
    /// </typeparam>
    /// <seealso cref="IMessageHandler" />
    public interface IMessageHandler<T>
        : IMessageHandler
        where T : MessageBase
    {
        /// <summary>
        /// Handles the message specified message.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="MessageBase" /> of type <typeparamref name="T" /> that shall be handled.
        ///     Must not be <c>null</c>.
        /// </param>
        void HandleMessage(T message);
    }
}
