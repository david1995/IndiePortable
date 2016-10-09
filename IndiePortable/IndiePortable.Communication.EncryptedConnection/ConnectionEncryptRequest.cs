// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionEncryptRequest.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionEncryptRequest class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.EncryptedConnection
{
    using System;
    using Devices.ConnectionMessages;
    using Formatter;

    /// <summary>
    /// Represents an encryption request.
    /// </summary>
    /// <seealso cref="ConnectionMessageRequestBase" />
    [Serializable]
    public sealed class ConnectionEncryptRequest
        : ConnectionMessageRequestBase
    {
        /// <summary>
        /// The backing field for the <see cref="PublicKey" /> property.
        /// </summary>
        private readonly PublicKeyInfo publicKeyBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionEncryptRequest" /> class.
        /// </summary>
        /// <param name="publicKey">
        ///     The <see cref="PublicKeyInfo" /> of the local connection end.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="publicKey" /> is <c>null</c>.</para>
        /// </exception>
        public ConnectionEncryptRequest(PublicKeyInfo publicKey)
        {
            if (object.ReferenceEquals(publicKey, null))
            {
                throw new ArgumentNullException(nameof(publicKey));
            }

            this.publicKeyBacking = publicKey;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ConnectionEncryptRequest" /> class from being created.
        /// </summary>
        private ConnectionEncryptRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionEncryptRequest" /> class.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> containing the data to populate the <see cref="ConnectionEncryptRequest" />.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>
        ///         Thrown if <paramref name="data" /> does not contain the necessary information
        ///         to build the <see cref="ConnectionEncryptRequest" />.
        ///     </para>
        /// </exception>
        private ConnectionEncryptRequest(ObjectDataCollection data)
            : base(data)
        {
            if (!data.TryGetValue(nameof(this.PublicKey), out this.publicKeyBacking))
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Gets the public key to encrypt the messages.
        /// </summary>
        /// <value>
        ///     Contains the public key to encrypt the message.
        /// </value>
        public PublicKeyInfo PublicKey => this.publicKeyBacking;

        /// <summary>
        /// Populates a specified <see cref="ObjectDataCollection" /> instance with data
        /// from the <see cref="ConnectionEncryptRequest" /> instance.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> that shall be populated.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        public override void GetObjectData(ObjectDataCollection data)
        {
            base.GetObjectData(data);

            data.AddValue(nameof(this.PublicKey), this.PublicKey);
        }
    }
}
