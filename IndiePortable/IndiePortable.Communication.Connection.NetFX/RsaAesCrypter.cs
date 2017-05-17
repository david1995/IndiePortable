// <copyright file="RsaAesCrypter.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Connection.NetFX
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using IndiePortable.Communication.Core.Devices;

    public class RsaAesCrypter
        : Crypter
    {
        private readonly object sessionLock = new object();
        private RSACryptoServiceProvider localRSA;
        private RSACryptoServiceProvider remoteRSA;
        private AesCryptoServiceProvider aesSymmetricEncrypter;
        private ICryptoTransform aesEncryptor;
        private AesCryptoServiceProvider aesSymmetricDecrypter;
        private PublicKeyInfo remotePublicKey;

        public RsaAesCrypter(Guid signature)
            : base(signature)
        {
            this.localRSA = new RSACryptoServiceProvider(1024);
            this.remoteRSA = new RSACryptoServiceProvider(1024);

            this.aesSymmetricEncrypter = new AesCryptoServiceProvider();
            this.aesSymmetricEncrypter.GenerateKey();
            this.aesEncryptor = this.aesSymmetricEncrypter.CreateEncryptor();

            this.aesSymmetricDecrypter = new AesCryptoServiceProvider()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None,
            };

            this.LocalPublicKey = new PublicKeyInfo(this.localRSA.ExportCspBlob(false));
        }

        public override PublicKeyInfo LocalPublicKey { get; }

        public override byte[] Decrypt(byte[] input)
        {
            if (!this.IsSessionStarted)
            {
                throw new InvalidOperationException();
            }

            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            using (var memstr = new MemoryStream(input, false))
            {
                // aes key length
                var aesLengthBytes = new byte[sizeof(int)];
                memstr.Read(aesLengthBytes, 0, sizeof(int));
                var aesLength = BitConverter.ToInt32(aesLengthBytes, 0);

                // aes key encrypted
                var aesKeyEncryptedBytes = new byte[aesLength];
                memstr.Read(aesKeyEncryptedBytes, 0, aesLength);
                var aesKeyDecryptedBytes = this.localRSA.Decrypt(aesKeyEncryptedBytes, true);

                // aes iv length
                var aesIVLengthBytes = new byte[sizeof(int)];
                memstr.Read(aesIVLengthBytes, 0, sizeof(int));
                var aesIVLength = BitConverter.ToInt32(aesIVLengthBytes, 0);

                // aes iv
                var aesIVEncryptedBytes = new byte[aesIVLength];
                memstr.Read(aesIVEncryptedBytes, 0, aesIVLength);
                var aesIVBytes = this.localRSA.Decrypt(aesIVEncryptedBytes, true);

                // aes raw length
                var aesRawLengthLengthBytes = new byte[sizeof(int)];
                memstr.Read(aesRawLengthLengthBytes, 0, sizeof(int));
                var aesRawLengthLength = BitConverter.ToInt32(aesRawLengthLengthBytes, 0);

                var aesRawLengthBytes = new byte[aesRawLengthLength];
                memstr.Read(aesRawLengthBytes, 0, aesRawLengthLength);
                var aesRawLength = BitConverter.ToInt32(this.localRSA.Decrypt(aesRawLengthBytes, true), 0);

                // create aes key instance
                this.aesSymmetricDecrypter.Key = aesKeyDecryptedBytes;
                this.aesSymmetricDecrypter.IV = aesIVBytes;
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

                            return decryptedContent.Take(aesRawLength).ToArray();
                        }
                    }
                }
            }
        }

        public override byte[] Encrypt(byte[] input)
        {
            if (!this.IsSessionStarted)
            {
                throw new InvalidOperationException();
            }

            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var rawLength = input.Length;
            var rawLengthBytes = BitConverter.GetBytes(rawLength);

            var iv = this.aesSymmetricEncrypter.IV;

            // generate aes-encrypted content
            byte[] dataAesEncrypted;
            using (var tempstr = new MemoryStream())
            {
                using (var crstream = new CryptoStream(tempstr, this.aesEncryptor, CryptoStreamMode.Write))
                {
                    crstream.Write(input, 0, rawLength);
                    crstream.Flush();
                    crstream.FlushFinalBlock();

                    dataAesEncrypted = tempstr.ToArray();
                }
            }

            using (var memstr = new MemoryStream())
            {
                var aesKey = this.aesSymmetricEncrypter.Key;

                // encrypt aes key
                var aesEncryptedKey = this.remoteRSA.Encrypt(aesKey, true);
                var aesKeyLength = BitConverter.GetBytes(aesEncryptedKey.Length);

                // encrypt aes iv
                var aesIVEncrypted = this.remoteRSA.Encrypt(iv, true);
                var aesIVEncryptedLengthBytes = BitConverter.GetBytes(aesIVEncrypted.Length);

                // encrypt raw length
                var rawLengthEncryptedBytes = this.remoteRSA.Encrypt(rawLengthBytes, true);
                var rawLengthEncryptedLength = BitConverter.GetBytes(rawLengthEncryptedBytes.Length);

                var aesEncryptedLength = BitConverter.GetBytes(dataAesEncrypted.Length);

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

        public override void StartSession(PublicKeyInfo remotePublicKey)
        {
            lock (this.sessionLock)
            {
                if (this.IsSessionStarted)
                {
                    throw new InvalidOperationException();
                }

                this.remotePublicKey = remotePublicKey;
                this.remoteRSA.ImportCspBlob(this.remotePublicKey.KeyBlob);
                this.IsSessionStarted = true;
            }
        }

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

        protected override void ExportPublicKeyOverride(Stream output)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (!output.CanWrite)
            {
                throw new ArgumentException($"{nameof(output)} must be a writable stream.", nameof(output));
            }

            var data = this.localRSA.ExportCspBlob(true);
            output.Write(data, 0, data.Length);
        }
    }
}
