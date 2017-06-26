// <copyright file="HttpServerConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Connections.Http
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using IndiePortable.Communication.Core.Devices.Connections;
    using IndiePortable.Communication.Core.Devices.Connections.Decorators;

    public class HttpServerConnection
        : ConnectionDecorator<byte[], IPEndPoint>
    {
        public HttpServerConnection(MessageConnectionBase<byte[], IPEndPoint> decoratedConnectoin)
            : base(decoratedConnectoin)
        {
            // TODO: create RequestResponseConnectionBase<TRequest, TResponse, TAddress> class
        }

        protected override void SendOverride(byte[] message)
        {
            base.SendOverride(message);
        }

        protected override Task SendAsyncOverride(byte[] message)
        {
            return base.SendAsyncOverride(message);
        }
    }
}
