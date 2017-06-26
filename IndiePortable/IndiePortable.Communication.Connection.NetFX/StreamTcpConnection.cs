// <copyright file="StreamTcpConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Connection.NetFX
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using IndiePortable.Communication.Core.Devices.Connections;

    public class StreamTcpConnection
        : StreamConnectionBase<IPEndPoint>
    {
        private readonly TcpClient client;

        private readonly NetworkStream stream;

        public StreamTcpConnection(TcpClient client)
            : base()
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.stream = this.client.GetStream();
        }

        public override Stream DataStream => this.stream;

        public override IPEndPoint RemoteAddress => this.client.Client.RemoteEndPoint is IPEndPoint ep
                                                  ? ep
                                                  : throw new IOException();

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

        protected override void ActivateOverride()
        {
        }

        protected override void DisconnectOverride()
        {
            this.DataStream.Dispose();
            this.client.Client.Disconnect(false);
            this.client.Close();
        }

        protected override Task DisconnectAsyncOverride()
        {
            this.DataStream.Dispose();
            this.client.Client.Disconnect(false);
            this.client.Close();
            return Task.CompletedTask;
        }

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();

            this.DataStream.Dispose();
            this.client.Client.Dispose();
            this.client.Dispose();
        }
    }
}
