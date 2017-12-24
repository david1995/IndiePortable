// <copyright file="RtspClientConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IndiePortable.Communication.Message;
using IndiePortable.Communication.Rtsp.Messages;

namespace IndiePortable.Communication.Rtsp
{
    public class RtspClientConnection
        : DuplexMessageConnection<RtspMessage>
    {
        protected RtspClientConnection()
        {
        }

        protected override void ActivateOverride() => throw new NotImplementedException();

        protected override Task DisconnectAsyncOverride() => throw new NotImplementedException();

        protected override void DisconnectOverride() => throw new NotImplementedException();

        protected override Task SendAsyncOverride(RtspMessage message) => throw new NotImplementedException();

        protected override void SendOverride(RtspMessage message) => throw new NotImplementedException();
    }
}
