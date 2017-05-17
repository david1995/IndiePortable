// <copyright file="Crypter.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core.Devices
{
    using System;
    using System.IO;

    public abstract class Crypter
        : IDisposable
    {
        private readonly Guid signature;

        protected Crypter(Guid signature)
        {
            this.signature = signature;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Crypter"/> class.
        /// </summary>
        ~Crypter()
        {
            this.Dispose(false);
        }

        public bool IsSessionStarted { get; protected set; }

        public abstract PublicKeyInfo LocalPublicKey { get; }

        public abstract void StartSession(PublicKeyInfo remotePublicKey);

        public abstract byte[] Encrypt(byte[] input);

        public abstract byte[] Decrypt(byte[] input);

        public void ExportPublicKey(Stream output, Guid signature)
        {
            if (signature != this.signature)
            {
                throw new InvalidOperationException();
            }

            this.ExportPublicKeyOverride(output ?? throw new ArgumentNullException(nameof(output)));
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);

        protected abstract void ExportPublicKeyOverride(Stream output);
    }
}
