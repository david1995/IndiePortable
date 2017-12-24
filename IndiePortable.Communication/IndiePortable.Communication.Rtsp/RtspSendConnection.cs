// <copyright file="RtspSendConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IndiePortable.Communication.Message;

namespace IndiePortable.Communication.Rtsp
{

    public class RtspSendConnection
        : SendMessageConnection<RtspRequest>
    {
        protected override void DisconnectOverride()
        {
            throw new NotImplementedException();
        }

        protected override void DisposeUnmanaged()
        {
            throw new NotImplementedException();
        }

        protected override void DisposeManaged()
        {
            throw new NotImplementedException();
        }

        protected override void SendOverride(RtspRequest message)
        {
            throw new NotImplementedException();
        }

        protected override Task SendAsyncOverride(RtspRequest message)
        {
            throw new NotImplementedException();
        }
    }
}
