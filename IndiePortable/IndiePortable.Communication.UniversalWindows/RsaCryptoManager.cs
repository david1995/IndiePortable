// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="RsaCryptoManager.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the RsaCryptoManager class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.UniversalWindows
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Security.Cryptography;
    using EncryptedConnection;
    using Windows.Security.Cryptography;
    using Windows.Security.Cryptography.Core;

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
        private readonly AsymmetricKeyAlgorithmProvider remoteRSA;


        private readonly CryptographicKey localKeyPair;

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


        private CryptographicKey remoteCryptographicKey;

        /// <summary>
        /// Indicates whether the <see cref="RsaCryptoManager" /> is disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RsaCryptoManager" /> class.
        /// </summary>
        public RsaCryptoManager()
        {
            this.remoteRSA = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);

            // get local key pair
            var localRSA = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);
            this.localKeyPair = localRSA.CreateKeyPair(4096);

            using (var binaryLocalKeyStream = this.localKeyPair.ExportPublicKey(CryptographicPublicKeyBlobType.Pkcs1RsaPublicKey).AsStream())
            {
                byte[] m = new byte[localKeyPair.KeySize / 8];
                binaryLocalKeyStream.Read(m, 0, m.Length);

                byte[] e = new byte[binaryLocalKeyStream.Length - m.Length];
                binaryLocalKeyStream.Read(e, 0, e.Length);
                this.localPublicKeyBacking = new PublicKeyInfo(e, m);
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RsaCryptoManager" /> class.
        /// </summary>
        ~RsaCryptoManager()
        {
            this.Dispose(false);
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


        public CryptographicKey RemoteKey => this.remoteCryptographicKey;


        public CryptographicKey LocalKey => this.localKeyPair;

        /// <summary>
        /// Starts an encryption session.
        /// </summary>
        /// <param name="remotePublicKey">
        ///     The public key of the remote client.</param>
        public override void StartSession(PublicKeyInfo remotePublicKey)
        {
            this.remotePublicKey = remotePublicKey;
            using (var memstr = new MemoryStream())
            {
                memstr.Write(remotePublicKey.Modulus, 0, remotePublicKey.Modulus.Length);
                memstr.Write(remotePublicKey.Exponent, 0, remotePublicKey.Exponent.Length);

                this.remoteCryptographicKey = this.remoteRSA.ImportPublicKey(memstr.ToArray().AsBuffer(), CryptographicPublicKeyBlobType.Pkcs1RsaPublicKey);
            }

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

            var buffer = CryptographicBuffer.CreateFromByteArray(data);
            return CryptographicEngine.Encrypt(this.remoteCryptographicKey, data.AsBuffer(), null).ToArray();
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

            var buffer = CryptographicBuffer.CreateFromByteArray(data);
            return CryptographicEngine.Decrypt(this.localKeyPair, buffer, null).ToArray();
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
                    this.remotePublicKey = default(PublicKeyInfo);
                    this.remoteCryptographicKey = null;
                }

                this.isDisposed = true;
            }
        }
    }
}
