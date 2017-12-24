// <copyright file="RtspRequest.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Rtsp
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class RtspRequest
    {

        public RtspRequest()
        {

        }

        public RtspRequestType RequestType { get; }

        public dynamic Headers { get; }

        public byte[] Body { get; }
    }
}
