// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandler.Generic.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IMessageHandler{T} interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices
{
    /// <summary>
    /// Encapsulates a message handler callback for a message of a specific type.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The type of the message.
    ///     Must derive from <see cref="MessageBase" />.
    /// </typeparam>
    /// <seealso cref="IMessageHandler" />
    public interface IMessageHandler<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Determines whether the <see cref="IMessageHandler{TMessage}"/> can handle a specified message.
        /// </summary>
        /// <param name="message">The message that shall be tested. Must not be <c>null</c>.</param>
        /// <returns><c>true</c> if <paramref name="message"/> can be handled; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <para><paramref name="message"/> is <c>null</c>.</para>
        /// </exception>
        bool CanHandleMessage(TMessage message);

        /// <summary>
        /// Handles the message specified message.
        /// </summary>
        /// <param name="message">The message that shall be handled. Must not be <c>null</c>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <para><paramref name="message"/> is <c>null</c>.</para>
        /// </exception>
        void HandleMessage(TMessage message);
    }
}
