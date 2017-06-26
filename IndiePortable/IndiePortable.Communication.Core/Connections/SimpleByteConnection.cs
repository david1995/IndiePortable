// <copyright file="SimpleByteConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core.Devices.Connections
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using IndiePortable.Communication.Core.Devices;
    using IndiePortable.Communication.Core.Devices.Connections;

    public class SimpleByteConnection
        : MessageConnectionBase<byte[], IPEndPoint>
    {
        private readonly object messageIdLock = new object();
        private ulong messageId;

        public SimpleByteConnection(StreamConnectionBase<IPEndPoint> decoratedConnection)
        {
            this.DecoratedConnection = decoratedConnection ?? throw new ArgumentNullException(nameof(decoratedConnection));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SimpleByteConnection"/> class.
        /// </summary>
        ~SimpleByteConnection()
        {
            this.Dispose(false);
        }

        protected event Action<(MessageType Type, ulong MessageId, byte[] Message)> StreamMessageReceived;

        /// <inheritdoc/>
        public override IPEndPoint RemoteAddress => this.DecoratedConnection.RemoteAddress;

        protected StreamConnectionBase<IPEndPoint> DecoratedConnection { get; }

        protected ulong NewMessageId
        {
            get
            {
                lock (this.messageIdLock)
                {
                    return unchecked(this.messageId++);
                }
            }
        }

        protected void OnStreamMessageReceived(MessageType type, ulong messageId, byte[] message)
            => this.StreamMessageReceived?.Invoke((type, messageId, message));

        /// <inheritdoc/>
        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();
            this.DecoratedConnection.Dispose();
        }

        /// <inheritdoc/>
        protected override void ActivateOverride()
        {
        }

        /// <inheritdoc/>
        protected override void DisconnectOverride()
        {
            using (var autoReset = new AutoResetEvent(false))
            {
                ulong id = 0;
                byte[] msg;
                MessageType type;

                this.StreamMessageReceived += UnlockAcknowledgeAwaiter;
                (msg, type, id) = this.SendStreamMessage(new byte[0], MessageType.Disconnect);

                void UnlockAcknowledgeAwaiter((MessageType Type, ulong MessageId, byte[] Message) e)
                {
                    if (e.Type == MessageType.Acknowledge &&
                        e.Message.Length == sizeof(ulong) &&
                        BitConverter.ToUInt64(e.Message, 0) == id)
                    {
                        var messageId = BitConverter.ToUInt64(e.Message, 0);
                        autoReset.Set();
                    }
                }

                // local disconnect -> remote acknowledge
                autoReset.WaitOne();
                this.StreamMessageReceived -= UnlockAcknowledgeAwaiter;

                // remote disconnect -> local acknowledge
                void UnlockDisconnectAwaiter((MessageType Type, ulong MessageId, byte[] Message) e)
                {
                    if (e.Type == MessageType.Disconnect &&
                        e.Message.Length == 0)
                    {
                        this.SendStreamMessage(BitConverter.GetBytes(e.MessageId), MessageType.Acknowledge);
                        autoReset.Set();
                    }
                }

                this.StreamMessageReceived += UnlockDisconnectAwaiter;
                autoReset.WaitOne();
                this.StreamMessageReceived -= UnlockDisconnectAwaiter;

                this.DecoratedConnection.Disconnect();
            }
        }

        /// <inheritdoc/>
        protected override async Task DisconnectAsyncOverride()
        {
            using (var autoReset = new AutoResetEvent(false))
            {
                ulong id = 0;
                byte[] msg;
                MessageType type;

                this.StreamMessageReceived += UnlockAcknowledgeAwaiter;
                (msg, type, id) = await this.SendStreamMessageAsync(new byte[0], MessageType.Disconnect);

                void UnlockAcknowledgeAwaiter((MessageType Type, ulong MessageId, byte[] Message) e)
                {
                    if (e.Type == MessageType.Acknowledge &&
                        e.Message.Length == sizeof(ulong) &&
                        BitConverter.ToUInt64(e.Message, 0) == id)
                    {
                        var messageId = BitConverter.ToUInt64(e.Message, 0);
                        autoReset.Set();
                    }
                }

                // local disconnect -> remote acknowledge
                await Task.Factory.StartNew(() => autoReset.WaitOne());
                this.StreamMessageReceived += UnlockDisconnectAwaiter;
                this.StreamMessageReceived -= UnlockAcknowledgeAwaiter;

                // remote disconnect -> local acknowledge
                async void UnlockDisconnectAwaiter((MessageType Type, ulong MessageId, byte[] Message) e)
                {
                    if (e.Type == MessageType.Disconnect &&
                        e.Message.Length == 0)
                    {
                        await this.SendStreamMessageAsync(BitConverter.GetBytes(e.MessageId), MessageType.Acknowledge);
                        autoReset.Set();
                    }
                }

                await Task.Factory.StartNew(() => autoReset.WaitOne());
                this.StreamMessageReceived -= UnlockDisconnectAwaiter;

                // logical layer connection resolved
                // -> disconnecting lower layer connection
                await this.DecoratedConnection.DisconnectAsync();
            }
        }

        /// <inheritdoc/>
        protected override void SendOverride(byte[] message)
        {
            using (var str = new MemoryStream())
            {
                var lengthBytes = BitConverter.GetBytes(message.Length);
                str.Write(lengthBytes, 0, sizeof(int));
                str.Write(message, 0, message.Length);

                str.WriteTo(this.DecoratedConnection.DataStream);
            }
        }

        /// <inheritdoc/>
        protected override async Task SendAsyncOverride(byte[] message)
        {
            using (var str = new MemoryStream())
            {
                var lengthBytes = BitConverter.GetBytes(message.Length);
                await str.WriteAsync(lengthBytes, 0, sizeof(int));
                await str.WriteAsync(message, 0, message.Length);

                str.WriteTo(this.DecoratedConnection.DataStream);
            }
        }

        private (byte[] Message, MessageType Type, ulong MessageId) SendStreamMessage(byte[] message, MessageType type)
        {
            var msg = (Message: message, Type: type, MessageId: this.NewMessageId);
            using (var str = new MemoryStream())
            {
                var lengthBytes = BitConverter.GetBytes(msg.Message.Length);
                var idBytes = BitConverter.GetBytes(msg.MessageId);

                str.WriteByte((byte)msg.Type);
                str.Write(idBytes, 0, sizeof(ulong));
                str.Write(lengthBytes, 0, sizeof(int));
                str.Write(message, 0, msg.Message.Length);

                str.WriteTo(this.DecoratedConnection.DataStream);

                return msg;
            }
        }

        private async Task<(byte[] Message, MessageType Type, ulong MessageId)> SendStreamMessageAsync(byte[] message, MessageType type)
        {
            var msg = (Message: message, Type: type, MessageId: this.NewMessageId);
            using (var str = new MemoryStream())
            {
                var lengthBytes = BitConverter.GetBytes(msg.Message.Length);
                var idBytes = BitConverter.GetBytes((byte)msg.Type);

                str.WriteByte((byte)msg.Type);
                await str.WriteAsync(idBytes, 0, sizeof(ulong));
                await str.WriteAsync(lengthBytes, 0, sizeof(int));
                await str.WriteAsync(message, 0, msg.Message.Length);

                str.WriteTo(this.DecoratedConnection.DataStream);

                return msg;
            }
        }

        [Flags]
        protected enum MessageType
            : byte
        {
            Message = 0b00_00_00_00,

            Acknowledge = 0b00_00_00_01,

            Disconnect = 0b00_00_00_10,

            Heartbeat = 0b00_00_01_00,

            HeartbeatReturn = 0b00_00_10_00
        }
    }
}
