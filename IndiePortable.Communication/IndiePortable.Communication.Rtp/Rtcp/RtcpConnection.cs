// <copyright file="RtcpConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using IndiePortable.Communication.Message;

namespace IndiePortable.Communication.Rtp.Rtcp
{
    public class RtcpConnection
        : DuplexMessageConnection<RtcpMessage>
    {
        protected override void ActivateOverride()
        {
            throw new NotImplementedException();
        }

        protected override void SendOverride(RtcpMessage message)
        {
            throw new NotImplementedException();
        }

        protected override Task SendAsyncOverride(RtcpMessage message)
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
    }
}
