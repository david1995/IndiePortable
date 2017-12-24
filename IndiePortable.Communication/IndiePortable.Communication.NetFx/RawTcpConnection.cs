// <copyright file="RawTcpConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using IndiePortable.Communication.Message;

namespace IndiePortable.Communication
{
    public class RawTcpConnection
        : DuplexMessageConnection<byte[]>
    {
        private readonly TcpClient client;

        private readonly NetworkStream clientStream;

        public RawTcpConnection(TcpClient client)
        {
            this.client = client is null
                        ? throw new ArgumentNullException(nameof(client))
                        : !client.Connected
                        ? throw new ArgumentException("The specified client must be connected.", nameof(client))
                        : client;

            this.RemoteAddress = !(client.Client.RemoteEndPoint is IPEndPoint ep)
                               ? throw new ArgumentException($"The TCP client's end point must be an {nameof(IPEndPoint)} instance.", nameof(client))
                               : ep;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RawTcpConnection"/> class.
        /// </summary>
        ~RawTcpConnection()
        {
            this.Dispose(false);
        }

        public IPEndPoint RemoteAddress { get; }

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
