// <copyright file="MessageConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core.Devices.Connections.Decorators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using IndiePortable.Communication.Core.Devices.ConnectionMessages;
    using IndiePortable.Communication.Core.Messages;
    using IndiePortable.Formatter;

    public class MessageConnection<TAddress>
        : ConnectionBase<MessageBase, TAddress>
    {
        private ConnectionBase<byte[], TAddress> decoratedConnection;

        private BinaryFormatter formatter = BinaryFormatter.CreateWithCoreSurrogates();

        public MessageConnection(ConnectionBase<byte[], TAddress> decoratedConnection)
        {
            this.decoratedConnection = decoratedConnection ?? throw new ArgumentNullException(nameof(decoratedConnection));

            this.decoratedConnection.Disconnected += (s, e) => this.OnDisconnected();
            this.decoratedConnection.ConnectionStateChanged += (s, e) => this.OnConnectionStateChanged();
            this.decoratedConnection.MessageReceived += (s, e) =>
            {
                using (var str = new MemoryStream(e.ReceivedMessage, false))
                {
                    var lengthBytes = new byte[sizeof(int)];
                    str.Read(lengthBytes, 0, sizeof(int));
                    var length = BitConverter.ToInt32(lengthBytes, 0);

                    if (this.formatter.TryDeserialize<MessageBase>(str, out var msg))
                    {
                        this.OnMessageReceived(msg);
                    }
                }
            };
        }

        public override TAddress RemoteAddress => this.decoratedConnection.RemoteAddress;

        protected override void ActivateOverride()
        {
            throw new NotImplementedException();
        }

        protected override Task DisconnectAsyncOverride()
        {
            throw new NotImplementedException();
        }

        protected override void DisconnectOverride()
        {
            throw new NotImplementedException();
        }

        protected override void SendOverride(MessageBase message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.SendConnectionMessage(new ConnectionContentMessage(message));
        }

        protected override async Task SendAsyncOverride(MessageBase message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            await this.SendConnectionMessageAsync(new ConnectionContentMessage(message));
        }

        protected void SendConnectionMessage(ConnectionMessageBase message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            using (var res = new MemoryStream())
            {
                using (var str = new MemoryStream())
                {
                    if (this.formatter.TrySerialize(str, message))
                    {
                        var lengthBytes = BitConverter.GetBytes((int)str.Length);
                        res.Write(lengthBytes, 0, sizeof(int));
                        str.WriteTo(res);

                        this.decoratedConnection.Send(res.ToArray());
                    }
                }
            }
        }

        protected async Task SendConnectionMessageAsync(ConnectionMessageBase message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            using (var res = new MemoryStream())
            {
                using (var str = new MemoryStream())
                {
                    if (await Task.Run(() => this.formatter.TrySerialize(str, message)))
                    {
                        var lengthBytes = BitConverter.GetBytes((int)str.Length);
                        res.Write(lengthBytes, 0, sizeof(int));
                        str.WriteTo(res);

                        await this.decoratedConnection.SendAsync(res.ToArray());
                    }
                }
            }
        }
    }
}
