// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageReceiver.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IMessageReceiver interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices
{
    using System;

    /// <summary>
    /// Represents a logical device that is capable of receiving <see cref="Messages.MessageBase" /> objects.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IMessageReceiver
        : IDisposable
    {
        /// <summary>
        /// Raised when a <see cref="Messages.MessageBase" /> object has been received.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Gets the <see cref="MessageDispatcher" /> acting as a message cache.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="MessageDispatcher" /> acting as a message cache.
        /// </value>
        MessageDispatcher Cache { get; }
    }
}
