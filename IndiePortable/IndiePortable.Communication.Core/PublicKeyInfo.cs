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

    /// <summary>
    /// Represents a public key.
    /// </summary>
    public struct PublicKeyInfo
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
            this.KeyBlob = keyBlob ?? throw new ArgumentNullException(nameof(keyBlob));
        }

        /// <summary>
        /// Gets the public key formatted in a CAPI-compatible CSP format.
        /// </summary>
        /// <value>
        /// Contains the public key formatted in a CAPI-compatible CSP format.
        /// </value>
        public byte[] KeyBlob { get; }
    }
}
