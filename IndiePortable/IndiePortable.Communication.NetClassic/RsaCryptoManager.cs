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
    using System.IO;
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


        private readonly AesCryptoServiceProvider aesSymmetricEncrypter;


        private readonly AesCryptoServiceProvider aesSymmetricDecrypter;


        private readonly ICryptoTransform aesEncryptor;

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
            this.aesSymmetricEncrypter = new AesCryptoServiceProvider();
            this.aesSymmetricEncrypter.GenerateKey();
            this.aesEncryptor = this.aesSymmetricEncrypter.CreateEncryptor();
            
            this.localPublicKeyBacking = new PublicKeyInfo(this.localRSA.ExportCspBlob(false));
        }


        public RsaCryptoManager(byte[] rsaKeyPairBlob)
        {
            if (object.ReferenceEquals(rsaKeyPairBlob, null))
            {
                throw new ArgumentNullException(nameof(rsaKeyPairBlob));
            }

            this.localRSA = new RSACryptoServiceProvider(4096);
            this.localRSA.ImportCspBlob(rsaKeyPairBlob);
            this.remoteRSA = new RSACryptoServiceProvider(4096);

            this.aesSymmetricEncrypter = new AesCryptoServiceProvider();
            this.aesSymmetricEncrypter.GenerateKey();
            this.aesEncryptor = this.aesSymmetricEncrypter.CreateEncryptor();

            this.localPublicKeyBacking = new PublicKeyInfo(this.localRSA.ExportCspBlob(false));
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
            this.remoteRSA.ImportCspBlob(this.remotePublicKey.KeyBlob);
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
            
            using (var memstr = new MemoryStream())
            {
                // encrypt aes key
                var aesEncryptedKey = this.localRSA.Encrypt(this.aesSymmetricEncrypter.Key, false);

                var aesEncryptedLength = BitConverter.GetBytes(aesEncryptedKey.Length);

                // write rsa-encrypted aes key
                memstr.Write(aesEncryptedLength, 0, sizeof(int));
                memstr.Write(aesEncryptedKey, 0, aesEncryptedKey.Length);

                using (var tempstr = new MemoryStream())
                {
                    using (var crstream = new CryptoStream(tempstr, this.aesEncryptor, CryptoStreamMode.Write))
                    {
                        // write aes-encrypted content
                        crstream.Write(data, 0, data.Length);
                        crstream.Flush();
                        crstream.FlushFinalBlock();

                        // content length
                        var contentEncryptedLengthBytes = BitConverter.GetBytes((int)tempstr.Length);
                        memstr.Write(contentEncryptedLengthBytes, 0, sizeof(int));

                        // write content
                        tempstr.CopyTo(memstr);

                        return memstr.ToArray();
                    }
                }
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
                var aesKeyDecryptedBytes = this.localRSA.Decrypt(aesKeyEncryptedBytes, false);

                // create aes key instance
                this.aesSymmetricDecrypter.Key = aesKeyDecryptedBytes;
                using (var aesDecryptor = this.aesSymmetricDecrypter.CreateDecryptor())
                {
                    // read content length
                    var contentLengthBytes = new byte[sizeof(int)];
                    memstr.Read(contentLengthBytes, 0, sizeof(int));
                    var contentLength = BitConverter.ToInt32(contentLengthBytes, 0);

                    // read content
                    var contentEncryptedBytes = new byte[contentLength];
                    memstr.Read(contentEncryptedBytes, 0, contentLength);

                    // decrypt content
                    using (var tempstr = new MemoryStream(contentEncryptedBytes, false))
                    {
                        using (var crstream = new CryptoStream(tempstr, aesDecryptor, CryptoStreamMode.Read))
                        {
                            var decryptedContent = new byte[contentLength];
                            crstream.Read(decryptedContent, 0, contentLength);

                            return decryptedContent;
                        }
                    }
                }
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
                }

                this.localRSA.Dispose();
                this.remoteRSA.Dispose();
                this.aesEncryptor.Dispose();
                this.aesSymmetricEncrypter.Dispose();

                this.isDisposed = true;
            }
        }
    }
}
