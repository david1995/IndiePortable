﻿// ----------------------------------------------------------------------------------------------------------------------------------------
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


        private readonly byte[] aesEncryptionKeyBytes;

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
            this.remoteRSA = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaOaepSha1);

            this.aesProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);

            // get local key pair
            var localRSA = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaOaepSha1);
            this.rsaLocalKeyPair = localRSA.CreateKeyPair(1024);

            var aesKeyBuffer = CryptographicBuffer.GenerateRandom(32);
            this.aesEncryptionKeyBytes = aesKeyBuffer.ToArray();
            this.aesSendKey = this.aesProvider.CreateSymmetricKey(aesKeyBuffer);

            this.localPublicKeyBacking = new PublicKeyInfo(this.rsaLocalKeyPair.ExportPublicKey(CryptographicPublicKeyBlobType.Capi1PublicKey).ToArray());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RsaCryptoManager" /> class.
        /// </summary>
        /// <param name="localRsaKeyPairBlob">
        ///     The local key pair stored in a byte array formatted with the legacy Cryptographic API format.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="localRsaKeyPairBlob" /> is <c>null</c>.</para>
        /// </exception>
        public RsaCryptoManager(byte[] localRsaKeyPairBlob)
        {
            if (object.ReferenceEquals(localRsaKeyPairBlob, null))
            {
                throw new ArgumentNullException(nameof(localRsaKeyPairBlob));
            }

            this.remoteRSA = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaOaepSha1);

            this.aesProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);

            // get local key pair
            var localRSA = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaOaepSha1);
            this.rsaLocalKeyPair = localRSA.ImportKeyPair(localRsaKeyPairBlob.AsBuffer(), CryptographicPrivateKeyBlobType.Capi1PrivateKey);

            this.aesProvider.CreateSymmetricKey(CryptographicBuffer.GenerateRandom(32));

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
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the session has not been started. Check the <see cref="IsSessionStarted" /> property.</para>
        /// </exception>
        public override byte[] Encrypt(byte[] data)
        {
            if (!this.IsSessionStarted)
            {
                throw new InvalidOperationException();
            }

            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            var rawLength = data.Length;
            var rawLengthBytes = BitConverter.GetBytes(rawLength);
            NormalizeLength(ref data, (int)this.aesProvider.BlockLength);

            var buffer = CryptographicBuffer.CreateFromByteArray(data);
            var iv = CryptographicBuffer.GenerateRandom(16);
            
            var dataAesEncrypted = CryptographicEngine.Encrypt(this.aesSendKey, buffer, iv).ToArray();
            using (var memstr = new MemoryStream())
            {
                var aesKey = this.aesEncryptionKeyBytes;

                // encrypt aes key
                var aesEncryptedKey = CryptographicEngine.Encrypt(
                    this.remoteRsaPublicKey,
                    aesKey.AsBuffer(),
                    null).ToArray();
                
                var aesKeyLength = BitConverter.GetBytes(aesEncryptedKey.Length);

                var aesEncryptedLength = BitConverter.GetBytes(dataAesEncrypted.Length);

                // encrypt aes iv
                var aesIVEncrypted = CryptographicEngine.Encrypt(
                    this.remoteRsaPublicKey,
                    iv,
                    null).ToArray();

                var aesIVEncryptedLengthBytes = BitConverter.GetBytes(aesIVEncrypted.Length);

                // encrypt raw length
                var rawLengthEncryptedBytes = CryptographicEngine.Encrypt(this.remoteRsaPublicKey, rawLengthBytes.AsBuffer(), null).ToArray();
                var rawLengthEncryptedLength = BitConverter.GetBytes(rawLengthEncryptedBytes.Length);

                // write rsa-encrypted aes key
                memstr.Write(aesKeyLength, 0, sizeof(int));
                memstr.Write(aesEncryptedKey, 0, aesEncryptedKey.Length);

                // write rsa-encrypted iv
                memstr.Write(aesIVEncryptedLengthBytes, 0, sizeof(int));
                memstr.Write(aesIVEncrypted, 0, aesIVEncrypted.Length);

                // write rsa-encrypted raw length
                memstr.Write(rawLengthEncryptedLength, 0, sizeof(int));
                memstr.Write(rawLengthEncryptedBytes, 0, rawLengthEncryptedBytes.Length);

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
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the session has not been started. Check the <see cref="IsSessionStarted" /> property.</para>
        /// </exception>
        public override byte[] Decrypt(byte[] data)
        {
            if (!this.IsSessionStarted)
            {
                throw new InvalidOperationException();
            }

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
                memstr.Read(aesKeyEncryptedBytes, 0, aesLength);
                var aesKeyDecryptedBuffer = CryptographicEngine.Decrypt(this.rsaLocalKeyPair, aesKeyEncryptedBytes.AsBuffer(), null);

                // aes iv length
                var aesIVLengthBytes = new byte[sizeof(int)];
                memstr.Read(aesIVLengthBytes, 0, sizeof(int));
                var aesIVLength = BitConverter.ToInt32(aesIVLengthBytes, 0);

                // aes iv
                var aesIVEncryptedBytes = new byte[aesIVLength];
                memstr.Read(aesIVEncryptedBytes, 0, aesIVLength);
                var aesIVBuffer = CryptographicEngine.Decrypt(this.rsaLocalKeyPair, aesIVEncryptedBytes.AsBuffer(), null);

                // aes raw length
                var aesRawLengthLengthBytes = new byte[sizeof(int)];
                memstr.Read(aesRawLengthLengthBytes, 0, sizeof(int));
                var aesRawLengthLength = BitConverter.ToInt32(aesRawLengthLengthBytes, 0);

                var aesRawLengthBytes = new byte[aesRawLengthLength];
                memstr.Read(aesRawLengthBytes, 0, aesRawLengthLength);
                var aesRawLength = BitConverter.ToInt32(
                    CryptographicEngine.Decrypt(
                        this.rsaLocalKeyPair,
                        aesRawLengthBytes.AsBuffer(),
                        null).ToArray(),
                    0);

                // create aes key instance
                var aesDecryptKey = this.aesProvider.CreateSymmetricKey(aesKeyDecryptedBuffer);

                // read content length
                var contentLengthBytes = new byte[sizeof(int)];
                memstr.Read(contentLengthBytes, 0, sizeof(int));
                var contentLength = BitConverter.ToInt32(contentLengthBytes, 0);

                // read content
                var contentEncryptedBytes = new byte[contentLength];
                memstr.Read(contentEncryptedBytes, 0, contentLength);

                // decrypt content
                using (var tempstr = CryptographicEngine.Decrypt(aesDecryptKey, contentEncryptedBytes.AsBuffer(), aesIVBuffer).AsStream())
                {
                    var content = new byte[aesRawLength];
                    tempstr.Read(content, 0, aesRawLength);
                    return content;
                }
            }
        }

        /// <summary>
        /// Exports the local RSA key pair to a strem.
        /// </summary>
        /// <param name="target">
        ///     The <see cref="Stream" /> to which the RSA key pair shall be written.
        ///     Must not be <c>null</c>.
        ///     Must be a writable <see cref="Stream" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="target" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if <paramref name="target" /> is not writable. Check the <see cref="Stream.CanWrite" /> property.</para>
        /// </exception>
        public void ExportLocalKeyPair(Stream target)
        {
            if (object.ReferenceEquals(target, null))
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (!target.CanWrite)
            { 
                throw new ArgumentException("The target stream cannot be written to.", nameof(target));
            }

            var key = this.rsaLocalKeyPair.Export(CryptographicPrivateKeyBlobType.Capi1PrivateKey).ToArray();
            target.Write(key, 0, key.Length);
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


        private static void NormalizeLength(ref byte[] data, int blockSize)
        {
            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }
            
            if (data.Length % blockSize != 0)
            {
                var m = data.Length / blockSize;
                var r = blockSize * (m + 1);

                Array.Resize(ref data, r);
            }
        }
    }
}
