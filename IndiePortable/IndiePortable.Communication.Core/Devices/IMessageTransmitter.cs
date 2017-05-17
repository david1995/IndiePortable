// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageTransmitter.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IMessageTransmitter interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices
{
    using System.Threading.Tasks;
    using Messages;

    /// <summary>
    /// Represents a logical device that is capable of sending <see cref="MessageBase" /> objects.
    /// </summary>
    public interface IMessageTransmitter
    {
        /// <summary>
        /// Sends a <see cref="MessageBase" /> object.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="MessageBase" /> that shall be sent.
        /// </param>
        void SendMessage(MessageBase message);

        /// <summary>
        /// Sends a <see cref="MessageBase" /> object asynchronously.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="MessageBase" /> that shall be sent.
        /// </param>
        /// <returns>
        ///     The task processing the method.
        /// </returns>
        Task SendMessageAsync(MessageBase message);
    }
}
