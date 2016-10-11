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
    using System.Linq;
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

        private readonly SymmetricKeyAlgorithmProvider aesProvider;

        /// <summary>
        /// The object responsible for decrypting data.
        /// </summary>
        private readonly AsymmetricKeyAlgorithmProvider remoteRSA;


        private readonly CryptographicKey rsaLocalKeyPair;


        private readonly CryptographicKey aesSendKey;


        private readonly CryptographicKey aesReceiveKey;

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


        private CryptographicKey remoteRsaPublicKey;

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

            this.aesProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbc);

            // get local key pair
            var localRSA = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);
            this.rsaLocalKeyPair = localRSA.CreateKeyPair(4096);
            this.aesProvider.CreateSymmetricKey(CryptographicBuffer.GenerateRandom(256));

            this.localPublicKeyBacking = new PublicKeyInfo(this.rsaLocalKeyPair.ExportPublicKey(CryptographicPublicKeyBlobType.Capi1PublicKey).ToArray());
        }


        public RsaCryptoManager(byte[] localRsaKeyPairBlob)
        {
            if (object.ReferenceEquals(localRsaKeyPairBlob, null))
            {
                throw new ArgumentNullException(nameof(localRsaKeyPairBlob));
            }

            this.remoteRSA = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);

            this.aesProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbc);

            // get local key pair
            var localRSA = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);
            this.rsaLocalKeyPair = localRSA.ImportKeyPair(localRsaKeyPairBlob.AsBuffer(), CryptographicPrivateKeyBlobType.Capi1PrivateKey);

            this.aesProvider.CreateSymmetricKey(CryptographicBuffer.GenerateRandom(256));

            this.localPublicKeyBacking = new PublicKeyInfo(this.rsaLocalKeyPair.ExportPublicKey(CryptographicPublicKeyBlobType.Capi1PublicKey).ToArray());
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

        /// <summary>
        /// Starts an encryption session.
        /// </summary>
        /// <param name="remotePublicKey">
        ///     The public key of the remote client.
        /// </param>
        public override void StartSession(PublicKeyInfo remotePublicKey)
        {
            if (this.IsSessionStarted)
            {
                throw new InvalidOperationException();
            }

            this.remotePublicKey = remotePublicKey;
            this.remoteRsaPublicKey = this.remoteRSA.ImportPublicKey(this.remotePublicKey.KeyBlob.AsBuffer(), CryptographicPublicKeyBlobType.Capi1PublicKey);

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

            var buffer = CryptographicBuffer.CreateFromByteArray(data);
            var dataAesEncrypted = CryptographicEngine.Encrypt(this.aesSendKey, buffer, null).ToArray();
            using (var memstr = new MemoryStream())
            {
                // encrypt aes key
                var aesKey = CryptographicEngine.Encrypt(
                    this.remoteRsaPublicKey,
                    this.aesSendKey.Export(CryptographicPrivateKeyBlobType.Capi1PrivateKey),
                    null).ToArray();

                var aesKeyLength = BitConverter.GetBytes(aesKey.Length);

                var aesEncryptedLength = BitConverter.GetBytes(dataAesEncrypted.Length);

                // write rsa-encrypted aes key
                memstr.Write(aesKeyLength, 0, sizeof(int));
                memstr.Write(aesKey, 0, aesKey.Length);

                // write aes-encrypted content
                memstr.Write(aesEncryptedLength, 0, sizeof(int));
                memstr.Write(dataAesEncrypted, 0, dataAesEncrypted.Length);

                return memstr.ToArray();
            }
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

            using (var memstr = new MemoryStream(data, false))
            {
                // aes key length
                var aesLengthBytes = new byte[sizeof(int)];
                memstr.Read(aesLengthBytes, 0, sizeof(int));
                var aesLength = BitConverter.ToInt32(aesLengthBytes, 0);

                // aes key encrypted
                var aesKeyEncryptedBytes = new byte[aesLength];
                var aesKeyDecryptedBuffer = CryptographicEngine.Decrypt(this.rsaLocalKeyPair, aesKeyEncryptedBytes.AsBuffer(), null);

                // create aes key instance
                var aesKey = this.aesProvider.CreateSymmetricKey(aesKeyDecryptedBuffer);

                // read content length
                var contentLengthBytes = new byte[sizeof(int)];
                memstr.Read(contentLengthBytes, 0, sizeof(int));
                var contentLength = BitConverter.ToInt32(contentLengthBytes, 0);

                // read content
                var contentEncryptedBytes = new byte[contentLength];
                memstr.Read(contentEncryptedBytes, 0, contentLength);

                // decrypt content
                return CryptographicEngine.Decrypt(aesKey, contentEncryptedBytes.AsBuffer(), null).ToArray();
            }
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
                    this.remoteRsaPublicKey = null;
                }

                this.isDisposed = true;
            }
        }
    }
}
