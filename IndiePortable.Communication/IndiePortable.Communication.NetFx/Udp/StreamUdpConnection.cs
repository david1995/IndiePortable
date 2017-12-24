// <copyright file="StreamUdpConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using IndiePortable.AdvancedTasks;
using IndiePortable.Communication.Binary;

namespace IndiePortable.Communication.Udp
{
    public class StreamUdpConnection
        : StreamConnection
    {
        private readonly MemoryStream dataStream;

        private readonly UdpClient client;

        private readonly StateTask readerTask;

        public StreamUdpConnection(UdpClient client, IPEndPoint remoteEp)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.RemoteAddress = remoteEp ?? throw new ArgumentNullException(nameof(remoteEp));

            this.dataStream = new MemoryStream();

            this.readerTask = new StateTask(this.Reader);
        }

        public override Stream PayloadStream => this.dataStream;

        public IPEndPoint RemoteAddress { get; }

        public static StreamUdpConnection ConnectTo(IPEndPoint remoteEp)
        {
            if (remoteEp is null)
            {
                throw new ArgumentNullException(nameof(remoteEp));
            }

            var c = new UdpClient();
            return new StreamUdpConnection(c, remoteEp);
        }

        protected override void InitializeOverride()
        {
            throw new NotImplementedException();
        }

        protected override void ActivateOverride()
        {
        }

        protected override void DisconnectOverride()
        {
            this.readerTask.StopAndAwait();
            this.client.Client.Disconnect(false);
            this.client.Client.Dispose();
        }

        protected override async Task DisconnectAsyncOverride()
        {
            await Task.Factory.StartNew(
                () =>
                {
                    this.client.Client.Disconnect(false);
                    this.client.Client.Dispose();
                });
        }

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();

            this.dataStream.Dispose();
            this.client.Dispose();
        }

        protected async void Reader(ITaskConnection conn)
        {
            while (!conn.MustFinish)
            {
                var res = await this.client.ReceiveAsync();

                if (res.RemoteEndPoint.Equals(this.RemoteAddress))
                {
                    this.dataStream.Write(res.Buffer, 0, res.Buffer.Length);
                }
            }
        }
    }
}
