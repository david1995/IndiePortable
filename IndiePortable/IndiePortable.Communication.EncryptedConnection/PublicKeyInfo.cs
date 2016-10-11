// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicKeyInfo.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contians the PublicKeyInfo struct.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.EncryptedConnection
{
    using System;
    using Formatter;

    /// <summary>
    /// Represents a public key.
    /// </summary>
    /// <seealso cref="ISerializable" />
    [Serializable]
    public struct PublicKeyInfo
        : ISerializable
    {
        /// <summary>
        /// The backing field for the <see cref="KeyBlob" /> property.
        /// </summary>
        private readonly byte[] keyBlobBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicKeyInfo"/> struct.
        /// </summary>
        /// <param name="keyBlob">
        ///     The public key formatted in a CAPI-compatible CSP format.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if one of these conditions is fulfilled:</para>
        ///     <list type="bullet">
        ///         <item><paramref name="keyBlob" /> is <c>null</c>.</item>
        ///         <item><paramref name="modulus" /> is <c>null</c>.</item>
        ///     </list>
        /// </exception>
        public PublicKeyInfo(byte[] keyBlob)
        {
            if (object.ReferenceEquals(keyBlob, null))
            {
                throw new ArgumentNullException(nameof(keyBlob));
            }

            this.keyBlobBacking = keyBlob;
        }


        private PublicKeyInfo(ObjectDataCollection data)
        {
            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.TryGetValue(nameof(this.KeyBlob), out this.keyBlobBacking))
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Gets the public key formatted in a CAPI-compatible CSP format.
        /// </summary>
        /// <value>
        ///     Contains the public key formatted in a CAPI-compatible CSP format.
        /// </value>
        public byte[] KeyBlob => this.keyBlobBacking;

        /// <summary>
        /// Populates a specified <see cref="ObjectDataCollection" /> instance with data from the <see cref="PublicKeyInfo" /> instance.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> that shall be populated.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        ///     <para>Implements <see cref="ISerializable.GetObjectData(ObjectDataCollection)" /> implicitly.</para>
        /// </remarks>
        public void GetObjectData(ObjectDataCollection data)
        {
            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            data.AddValue(nameof(this.KeyBlob), this.KeyBlob);
        }
    }
}
