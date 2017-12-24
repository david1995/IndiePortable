// <copyright file="StreamTcpConnectionClient.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// TODO: make documentation

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using IndiePortable.Communication.Binary;
using Ccs = IndiePortable.Communication.Core.ConnectionClientState;

namespace IndiePortable.Communication.Tcp
{

    public class StreamTcpConnectionClient
        : StreamConnectionClient<IPEndPoint, StreamTcpConnection>
    {
        protected override (bool Success, StreamTcpConnection Result) ConnectOverride(IPEndPoint address)
        {
            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (this.ClientState == Ccs.Connected)
            {
                throw new InvalidOperationException(
                    $"The state is invalid. {Ccs.Initialized}, {Ccs.Disconnected} or {Ccs.ConnectionLost} expected.");
            }

            try
            {
                var client = new TcpClient();
                client.Connect(address);

                var conn = new StreamTcpConnection(client);

                return (true, conn);
            }
            catch (SocketException)
            {
                return (false, null);
            }
        }

        protected override void InitializeOverride()
        {
        }
    }
}
