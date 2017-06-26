// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IIntervalTransmitter.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IIntervalTransmitter class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface for recurring message sending mechanisms.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of the message. Must be a class.
    /// </typeparam>
    /// <typeparam name="TAddress">
    /// The type of the address.
    /// </typeparam>
    public interface IIntervalTransmitter<TMessage, TAddress>
        where TMessage : class
    {
        /// <summary>
        /// Gets a value indicating whether the <see cref="IIntervalTransmitter{TAddress}" /> is started.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="IIntervalTransmitter{TAddress}" /> is started; otherwise <c>false</c>.
        /// </value>
        bool IsStarted { get; }

        TimeSpan Interval { get; }

        Func<TMessage> MessageGenerator { get; }

        /// <summary>
        /// Starts the interval transmitter.
        /// </summary>
        /// <param name="targets">
        /// The targets of the transmission.
        /// Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if:</para>
        /// <para>  - <paramref name="messageGenerator"/> is <c>null</c>.</para>
        /// <para>  - <paramref name="targets" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>The <see cref="IIntervalTransmitter{TMessage, TAddress}" /> has already been started.</para>
        /// </exception>
        void Start(IEnumerable<TAddress> targets);

        /// <summary>
        /// Stops the interval transmitter.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the <see cref="IIntervalTransmitter{TMessage, TAddress}" /> is not started.</para>
        /// </exception>
        void Stop();
    }
}
