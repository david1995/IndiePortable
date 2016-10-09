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
        /// The backing field for the <see cref="Exponent" /> property.
        /// </summary>
        private readonly byte[] exponentBacking;

        /// <summary>
        /// The backing field for the <see cref="Modulus" /> property.
        /// </summary>
        private readonly byte[] modulusBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicKeyInfo"/> struct.
        /// </summary>
        /// <param name="exponent">
        ///     The public key's exponent.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <param name="modulus">
        ///     The public key's modulus.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if one of these conditions is fulfilled:</para>
        ///     <list type="bullet">
        ///         <item><paramref name="exponent" /> is <c>null</c>.</item>
        ///         <item><paramref name="modulus" /> is <c>null</c>.</item>
        ///     </list>
        /// </exception>
        public PublicKeyInfo(byte[] exponent, byte[] modulus)
        {
            if (object.ReferenceEquals(exponent, null))
            {
                throw new ArgumentNullException(nameof(exponent));
            }

            if (object.ReferenceEquals(modulus, null))
            {
                throw new ArgumentNullException(nameof(modulus));
            }

            this.exponentBacking = exponent;
            this.modulusBacking = modulus;
        }


        private PublicKeyInfo(ObjectDataCollection data)
        {
            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.TryGetValue(nameof(this.Exponent), out this.exponentBacking) ||
                !data.TryGetValue(nameof(this.Modulus), out this.modulusBacking))
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Gets the exponent of the public key.
        /// </summary>
        /// <value>
        ///     Contains the exponent of the public key.
        /// </value>
        public byte[] Exponent => this.exponentBacking;

        /// <summary>
        /// Gets the modulus of the public key.
        /// </summary>
        /// <value>
        ///     Contains the modulus of the public key.
        /// </value>
        public byte[] Modulus => this.modulusBacking;

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

            data.AddValue(nameof(this.Exponent), this.Exponent);
            data.AddValue(nameof(this.Modulus), this.Modulus);
        }
    }
}
