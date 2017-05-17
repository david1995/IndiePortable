// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicKeyInfo.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contians the PublicKeyInfo struct.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices
{
    using System;
    using System.Runtime.Serialization;
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
        /// Initializes a new instance of the <see cref="PublicKeyInfo"/> struct.
        /// </summary>
        /// <param name="keyBlob">
        ///     The public key formatted in a CAPI-compatible CSP format.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="keyBlob" /> is <c>null</c>.</para>
        /// </exception>
        public PublicKeyInfo(byte[] keyBlob)
        {
            if (keyBlob is null)
            {
                throw new ArgumentNullException(nameof(keyBlob));
            }

            this.KeyBlob = keyBlob;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicKeyInfo"/> struct.
        /// </summary>
        /// <param name="data">
        /// The <see cref="SerializationInfo" /> populated with data for the <see cref="PublicKeyInfo" /> instance.
        /// </param>
        /// <param name="ctx">
        /// The streaming context of the deserialization mechanism.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>Thrown if <paramref name="data" /> does not contain information necessary for constructing the object.</para>
        /// </exception>
        private PublicKeyInfo(SerializationInfo data, StreamingContext ctx)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.KeyBlob = (byte[])data.GetValue(nameof(this.KeyBlob), typeof(byte[]));
        }

        /// <summary>
        /// Gets the public key formatted in a CAPI-compatible CSP format.
        /// </summary>
        /// <value>
        /// Contains the public key formatted in a CAPI-compatible CSP format.
        /// </value>
        public byte[] KeyBlob { get; }

        /// <summary>
        /// Populates a specified <see cref="ObjectDataCollection" /> instance with data from the <see cref="PublicKeyInfo" /> instance.
        /// </summary>
        /// <param name="data">
        /// The <see cref="SerializationInfo" /> that shall be populated.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="ctx">
        /// The streaming context of the serialization mechanism.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// <para>Implements <see cref="ISerializable.GetObjectData(ObjectDataCollection)" /> implicitly.</para>
        /// </remarks>
        public void GetObjectData(SerializationInfo data, StreamingContext ctx)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            data.AddValue(nameof(this.KeyBlob), this.KeyBlob);
        }
    }
}
