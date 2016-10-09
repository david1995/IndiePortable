// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="RsaCryptoManager.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the RsaCryptoManager class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.NetClassic
{
    using System;
    using System.Security.Cryptography;
    using EncryptedConnection;

    /// <summary>
    /// Encapsulates properties and methods for en- and decrypting bytes with RSA encryption.
    /// </summary>
    /// <seealso cref="CryptoManagerBase{T}" />
    /// <seealso cref="IDisposable" />
    public class RsaCryptoManager
        : CryptoManagerBase<PublicKeyInfo>
    {
        /// <summary>
        /// The object responsible for decrypting data.
        /// </summary>
        private readonly RSACryptoServiceProvider remoteRSA;

        /// <summary>
        /// The object responsible for encrypting data.
        /// </summary>
        private readonly RSACryptoServiceProvider localRSA;


        private readonly AesCryptoServiceProvider aesSymmetricCrypter;


        private readonly ICryptoTransform aesEncryptor;


        private readonly ICryptoTransform aesDecryptor;

        /// <summary>
        /// The backing field for the <see cref="LocalPublicKey" /> property.
        /// </summary>
        private readonly PublicKeyInfo localPublicKeyBacking;

        /// <summary>
        /// The backing field for the <seealso cref="IsSessionStarted" /> property.
        /// </summary>
        private bool isSessionStartedBacking;

        /// <summary>
        /// The remote public key information.
        /// </summary>
        private PublicKeyInfo remotePublicKey;

        /// <summary>
        /// Indicates whether the <see cref="RsaCryptoManager" /> is disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RsaCryptoManager" /> class.
        /// </summary>
        public RsaCryptoManager()
        {
            this.localRSA = new RSACryptoServiceProvider(4096);
            this.remoteRSA = new RSACryptoServiceProvider(4096);
            this.aesSymmetricCrypter = new AesCryptoServiceProvider();
            this.aesSymmetricCrypter.GenerateKey();
            this.aesEncryptor = this.aesSymmetricCrypter.CreateEncryptor();
            this.aesDecryptor = this.aesSymmetricCrypter.CreateDecryptor();

            var param = this.localRSA.ExportParameters(false);
            this.localPublicKeyBacking = new PublicKeyInfo(param.Exponent, param.Modulus);
            
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="RsaCryptoManager" /> has started an encryption session.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="RsaCryptoManager" /> has started an encryption session; otherwise, <c>false</c>.
        /// </value>
        public override bool IsSessionStarted => this.isSessionStartedBacking;

        /// <summary>
        /// Gets the public key of the local client.
        /// </summary>
        /// <value>
        ///     Contains the public key of the local client.
        /// </value>
        /// <remarks>
        ///     <para>Overrides <see cref="CryptoManagerBase{T}.LocalPublicKey" />.</para>
        /// </remarks>
        public override PublicKeyInfo LocalPublicKey => this.localPublicKeyBacking;

        /// <summary>
        /// Starts an encryption session.
        /// </summary>
        /// <param name="remotePublicKey">
        ///     The public key of the remote client.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     <para>
        ///         Thrown if the <see cref="RsaCryptoManager" /> has already started an encryption session.
        ///         Check the <see cref="IsSessionStarted" /> property.
        ///     </para>
        /// </exception>
        public override void StartSession(PublicKeyInfo remotePublicKey)
        {
            if (this.IsSessionStarted)
            {
                throw new InvalidOperationException();
            }

            this.remotePublicKey = remotePublicKey;

            this.remoteRSA.ImportParameters(
                new RSAParameters
                {
                    Exponent = this.remotePublicKey.Exponent,
                    Modulus = this.remotePublicKey.Modulus
                });

            this.isSessionStartedBacking = true;
        }

        /// <summary>
        /// Encrypts the specified data by using the RSA algorithm.
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
        public override byte[] Encrypt(byte[] data)
        {
            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            // TODO: implement additional use of AES
            return this.aesEncryptor.TransformFinalBlock(data, 0, data.Length);
        }

        /// <summary>
        /// Decrypts the specified data by using the RSA algorithm.
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
        public override byte[] Decrypt(byte[] data)
        {
            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            return this.localRSA.Decrypt(data, false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                }

                this.localRSA.Dispose();
                this.remoteRSA.Dispose();
                this.aesDecryptor.Dispose();
                this.aesEncryptor.Dispose();
                this.aesSymmetricCrypter.Dispose();

                this.isDisposed = true;
            }
        }
    }
}
