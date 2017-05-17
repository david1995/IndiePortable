// <copyright file="TcpConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Connection.NetFX
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using IndiePortable.Communication.Core.Devices;

    public class TcpConnection
        : ConnectionBase<byte[], IPEndPoint>
    {
        private readonly TcpClient client;

        public TcpConnection(TcpClient client, IPEndPoint remoteAddress)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.RemoteAddress = remoteAddress ?? throw new ArgumentNullException(nameof(remoteAddress));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TcpConnection"/> class.
        /// </summary>
        ~TcpConnection()
        {
            this.Dispose(false);
        }

        public override IPEndPoint RemoteAddress { get; }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();
        }

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();

            this.client.Dispose();
        }

        protected override void ActivateOverride()
        {
            throw new NotImplementedException();
        }

        protected override void DisconnectOverride()
        {
            throw new NotImplementedException();
        }

        protected override Task DisconnectAsyncOverride()
        {
            throw new NotImplementedException();
        }

        protected override void SendOverride(byte[] message)
        {
            throw new NotImplementedException();
        }

        protected override async Task SendAsyncOverride(byte[] message)
        {
            throw new NotImplementedException();
        }
    }
}
