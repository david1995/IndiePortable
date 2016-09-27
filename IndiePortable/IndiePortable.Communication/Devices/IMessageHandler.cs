// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandler.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IMessageHandler interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices
{
    using System;
    using Messages;

    /// <summary>
    /// Encapsulates a message handler callback for the <see cref="MessageDispatcher" />.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Gets the CLR type of <see cref="MessageBase" /> instances that shall be handled.
        /// </summary>
        /// <value>
        ///     Contains the CLR type of the <see cref="MessageBase" /> instances that shall be handled.
        /// </value>
        Type MessageClrType { get; }

        /// <summary>
        /// Handles an incoming <see cref="MessageBase" />.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="MessageBase" /> that shall be handled.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="message" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if the type of <paramref name="message" /> does not match <see cref="MessageClrType" />.</para>
        /// </exception>
        void HandleMessage(MessageBase message);
    }

}
