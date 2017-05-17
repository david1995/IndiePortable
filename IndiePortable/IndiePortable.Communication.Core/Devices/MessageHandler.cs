// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandler.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the MessageHandler{T} delegate.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices
{
    using Messages;

    /// <summary>
    /// Handles a message of type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the message.
    ///     Must derive from <see cref="MessageBase" />.
    /// </typeparam>
    /// <param name="message">
    ///     The message of type <typeparamref name="T" /> that has been received.
    /// </param>
    public delegate void MessageHandler<T>(T message) where T : MessageBase;
}
