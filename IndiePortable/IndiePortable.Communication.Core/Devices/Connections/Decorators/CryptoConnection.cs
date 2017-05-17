// <copyright file="CryptoConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core.Devices.Connections.Decorators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using IndiePortable.Communication.Core.Devices;

    public class CryptoConnection<TAddress>
        : ConnectionBase<byte[], TAddress>
    {
        private readonly ConnectionBase<byte[], TAddress> decoratedConnection;

        private readonly Crypter crypter;

        public CryptoConnection(ConnectionBase<byte[], TAddress> decoratedConnection, Crypter crypter)
        {
            this.decoratedConnection =
                decoratedConnection ?? throw new ArgumentNullException(nameof(decoratedConnection));

            this.crypter = crypter ?? throw new ArgumentNullException(nameof(crypter));

            this.decoratedConnection.MessageReceived += (s, e) =>
            {
                using (var str = new MemoryStream(e.ReceivedMessage))
                {
                    // type
                    var type = (CryptoMessageType)str.ReadByte();

                    // aes key length

                    // decrypt message
                    var decrypted = this.crypter.Decrypt(e.ReceivedMessage);
                    this.OnMessageReceived(decrypted);
                }
            };
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CryptoConnection{TAddress}"/> class.
        /// </summary>
        ~CryptoConnection()
        {
            this.Dispose(false);
        }

        public override TAddress RemoteAddress => throw new NotImplementedException();

        protected byte[] Encrypt(byte[] message)
        {
            // TODO: encrypt message
            throw new NotImplementedException();
        }

        protected override void ActivateOverride()
        {
            if (this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            this.decoratedConnection.Activate();

            // TODO: establish encryption
        }

        protected override void SendOverride(byte[] message)
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            this.decoratedConnection.Send(
                this.Encrypt(message ?? throw new ArgumentNullException(nameof(message))));
        }

        protected override async Task SendAsyncOverride(byte[] message)
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            await this.decoratedConnection.SendAsync(
                this.Encrypt(message ?? throw new ArgumentNullException(nameof(message))));
        }

        protected override void DisconnectOverride()
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            this.decoratedConnection.Disconnect();
        }

        protected override async Task DisconnectAsyncOverride()
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            await this.decoratedConnection.DisconnectAsync();
        }

        protected override void DisposeUnmanaged()
        {
            this.decoratedConnection.Dispose();
            base.DisposeUnmanaged();
        }

        private enum CryptoMessageType
            : byte
        {
            StartSession = 0b00_00_00_01,

            Message = 0b00_00_00_10
        }
    }
}
