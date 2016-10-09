// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ICryptableConnection.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ICryptableConnection{TAddress} interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.EncryptedConnection
{
    using System.Threading.Tasks;
    using Devices;

    /// <summary>
    /// Represents an <see cref="IConnection{TAddress}" /> that supports encryption.
    /// </summary>
    /// <typeparam name="TAddress">
    ///     The type of the addresses.
    /// </typeparam>
    /// <seealso cref="Devices.IConnection{TAddress}" />
    public interface ICryptableConnection<TAddress>
        : IConnection<TAddress>
        where TAddress : IAddressInfo
    {
        /// <summary>
        /// Gets a value indicating whether the current connection session is session encrypted.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the current connection session is encrypted; otherwise, <c>false</c>.
        /// </value>
        bool IsSessionEncrypted { get; }

        /// <summary>
        /// Starts the encryption session for the <see cref="ICryptableConnection{TAddress}" />.
        /// </summary>
        void StartEncryptionSession();

        /// <summary>
        /// Asynchronously starts the encryption session for the <see cref="ICryptableConnection{TAddress}" />.
        /// </summary>
        /// <returns>
        ///     The task representing the method.
        /// </returns>
        Task StartEncryptionSessionAsync();
    }
}
