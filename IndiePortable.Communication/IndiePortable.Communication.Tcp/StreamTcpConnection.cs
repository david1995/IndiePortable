// <copyright file="StreamTcpConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using IndiePortable.Communication.Binary;

namespace IndiePortable.Communication.Tcp
{
    public class StreamTcpConnection
        : StreamConnection
    {
        private readonly TcpClient client;
        private readonly NetworkStream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamTcpConnection"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="client"/> is <c>null</c>.</exception>
        public StreamTcpConnection(TcpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.stream = this.client.GetStream();
        }

        /// <summary>
        /// Gets the data stream carried by the <see cref="StreamTcpConnection"/>.
        /// </summary>
        /// <value>
        /// The data stream carried by the <see cref="StreamTcpConnection"/>.
        /// </value>
        public override Stream PayloadStream => this.stream;

        public IPEndPoint RemoteAddress => this.client.Client.RemoteEndPoint as IPEndPoint ?? throw new IOException();

        public static StreamTcpConnection ConnectTo(IPEndPoint ep)
        {
            if (ep is null)
            {
                throw new ArgumentNullException(nameof(ep));
            }

            var cl = new TcpClient();
            cl.Connect(ep);

            var conn = new StreamTcpConnection(cl);
            return conn;
        }

        protected override void InitializeOverride()
        {
        }

        protected override void ActivateOverride()
        {
        }

        protected override void DisconnectOverride()
        {
            this.PayloadStream.Dispose();
            this.client.Client.Disconnect(false);
            this.client.Close();
        }

        protected override Task DisconnectAsyncOverride()
        {
            this.PayloadStream.Dispose();
            this.client.Client.Disconnect(false);
            this.client.Close();
            return Task.CompletedTask;
        }

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();

            this.PayloadStream.Dispose();
            this.client.Client.Dispose();
            this.client.Dispose();
        }
    }
}
