// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="CryptoManagerBase.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the abstract CryptoManagerBase{T} class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.EncryptedConnection
{
    using System;

    /// <summary>
    /// Represents the base class for all implementations of a cryptographic managers.
    /// This class is abstract.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the publicly visible key.
    /// </typeparam>
    /// <seealso cref="IDisposable" />
    public abstract class CryptoManagerBase<T>
        : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoManagerBase{T}" /> class.
        /// </summary>
        protected CryptoManagerBase()
        {
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CryptoManagerBase{T}" /> class.
        /// </summary>
        ~CryptoManagerBase()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CryptoManagerBase{T}" /> has started an encryption session,
        /// when overriden in a derived class.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="CryptoManagerBase{T}" /> has started an encryption session; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsSessionStarted { get; }

        /// <summary>
        /// Gets the public key of the local client, when overriden in a derived class.
        /// </summary>
        /// <value>
        ///     Contains the public key of the local client.
        /// </value>
        public abstract T LocalPublicKey { get; }

        /// <summary>
        /// When overriden in a derived class, starts an encryption session.
        /// </summary>
        /// <param name="remotePublicKey">
        ///     The public key of the remote client.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the encryption session has already been started.</para>
        /// </exception>
        public abstract void StartSession(T remotePublicKey);

        /// <summary>
        /// When overriden in a derived class, encrypts the specified data by using the specified algorithm.
        /// </summary>
        /// <param name="data">
        ///     The data that shall be encrypted.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <returns>
        ///     The encrypted data.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        public abstract byte[] Encrypt(byte[] data);

        /// <summary>
        /// When overriden in a derived class, decrypts the specified data by using the specified algorithm.
        /// </summary>
        /// <param name="data">
        ///     The data that shall be encrypted.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <returns>
        ///     The decrypted data.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        public abstract byte[] Decrypt(byte[] data);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        ///     <para>Implements <see cref="IDisposable.Dispose()" /> implicitly.</para>
        /// </remarks>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// When overriden in a derived class, releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected abstract void Dispose(bool disposing);
    }
}
