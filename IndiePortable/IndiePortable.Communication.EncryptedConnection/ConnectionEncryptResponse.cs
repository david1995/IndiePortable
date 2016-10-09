// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionEncryptResponse.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionEncryptResponse class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.EncryptedConnection
{
    using System;
    using Devices.ConnectionMessages;
    using Formatter;

    /// <summary>
    /// Represents the response to a <see cref="ConnectionEncryptRequest" /> message.
    /// </summary>
    /// <seealso cref="ConnectionMessageResponseBase{T}" />
    [Serializable]
    public sealed class ConnectionEncryptResponse
        : ConnectionMessageResponseBase<ConnectionEncryptRequest>
    {
        /// <summary>
        /// The backing field for the <see cref="PublicKey" /> property.
        /// </summary>
        private readonly PublicKeyInfo publicKeyBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionEncryptResponse"/> class.
        /// </summary>
        /// <param name="publicKey">
        ///     The public key of the responding connection end.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <param name="request">
        ///     The request message that shall be responded to.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if at least one of the following conditions applies:</para>
        ///     <list type="bullet">
        ///         <item><paramref name="publicKey" /> is <c>null</c>.</item>
        ///         <item><paramref name="request" /> is <c>null</c>.</item>
        ///     </list>
        /// </exception>
        public ConnectionEncryptResponse(PublicKeyInfo publicKey, ConnectionEncryptRequest request)
            : base(request)
        {
            if (object.ReferenceEquals(publicKey, null))
            {
                throw new ArgumentNullException(nameof(publicKey));
            }

            this.publicKeyBacking = publicKey;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ConnectionEncryptResponse"/> class from being created.
        /// </summary>
        private ConnectionEncryptResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionEncryptResponse"/> class.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> that is used
        ///     to populate the <see cref="ConnectionEncryptResponse" /> instance.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if <paramref name="data" /> does not contain the necessary information.</para>
        /// </exception>
        private ConnectionEncryptResponse(ObjectDataCollection data)
            : base(data)
        {
            if (!data.TryGetValue(nameof(this.PublicKey), out this.publicKeyBacking))
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Gets the public key of the responding connection end.
        /// </summary>
        /// <value>
        ///     Contains the public key of the responding connection end.
        /// </value>
        public PublicKeyInfo PublicKey => this.publicKeyBacking;

        /// <summary>
        /// Populates a specified <see cref="ObjectDataCollection" /> instance
        /// with data from the <see cref="ConnectionEncryptResponse" /> instance.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> that shall be populated.
        ///     Must not be <c>null</c>.
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
