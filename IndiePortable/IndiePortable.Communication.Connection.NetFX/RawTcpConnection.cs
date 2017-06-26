// <copyright file="RawTcpConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Connection.NetFX
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using IndiePortable.Communication.Core.Devices.Connections;

    public class RawTcpConnection
        : MessageConnectionBase<byte[], IPEndPoint>
    {
        private readonly TcpClient client;

        private readonly NetworkStream clientStream;

        public RawTcpConnection(TcpClient client, IPEndPoint remoteAddress)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.RemoteAddress = remoteAddress ?? throw new ArgumentNullException(nameof(remoteAddress));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RawTcpConnection"/> class.
        /// </summary>
        ~RawTcpConnection()
        {
            this.Dispose(false);
        }

        public override IPEndPoint RemoteAddress { get; }

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();
            this.client.Dispose();
            this.client.Client.Dispose();
        }

        protected override void ActivateOverride()
        {
        }

        protected override void DisconnectOverride()
        {
            this.client.Client.Disconnect(true);
        }

        protected override async Task DisconnectAsyncOverride()
        {
            var ea = new SocketAsyncEventArgs { DisconnectReuseSocket = true };

            using (var ev = new AutoResetEvent(false))
            {
                ea.Completed += (s, e) => ev.Set();
                this.client.Client.DisconnectAsync(ea);
                await Task.Factory.StartNew(() => ev.WaitOne());
            }
        }

        protected override void SendOverride(byte[] message)
        {
            this.clientStream.Write(message, 0, message.Length);
        }

        protected override async Task SendAsyncOverride(byte[] message)
        {
            await this.clientStream.WriteAsync(message, 0, message.Length);
        }
    }
}
