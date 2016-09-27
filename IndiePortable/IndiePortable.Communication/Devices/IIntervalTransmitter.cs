// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IIntervalTransmitter.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IIntervalTransmitter class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices
{
    using System;
    using System.Collections.Generic;
    using Messages;

    /// <summary>
    /// Provides an interface for recurring message sending mechanisms.
    /// </summary>
    /// <typeparam name="TAddress">
    ///     The type of the address.
    /// </typeparam>
    public interface IIntervalTransmitter<TAddress>
        where TAddress : IAddressInfo
    {
        /// <summary>
        /// Gets a value indicating whether the <see cref="IIntervalTransmitter{TAddress}" /> is started.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="IIntervalTransmitter{TAddress}" /> is started; otherwise <c>false</c>.
        /// </value>
        bool IsStarted { get; }

        /// <summary>
        /// Starts the interval transmitter.
        /// </summary>
        /// <param name="interval">
        ///     The interval specifying when a message should be sent.
        ///     Must be greater than <see cref="TimeSpan.Zero" />.
        /// </param>
        /// <param name="messageGenerator">
        ///     The method that generates the message to be sent.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <param name="targets">
        ///     The targets of the transmission.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if:</para>
        ///     <para>  - <paramref name="messageGenerator"/> is <c>null</c>.</para>
        ///     <para>  - <paramref name="targets" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if <paramref name="interval" /> is smaller than or equals <see cref="TimeSpan.Zero" />.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the <see cref="IIntervalTransmitter{TAddress}" /> is already started.</para>
        /// </exception>
        void Start(TimeSpan interval, Func<MessageBase> messageGenerator, IEnumerable<TAddress> targets);

        /// <summary>
        /// Stops the interval transmitter.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the <see cref="IIntervalTransmitter{TAddress}" /> is not started.</para>
        /// </exception>
        void Stop();
    }
}
