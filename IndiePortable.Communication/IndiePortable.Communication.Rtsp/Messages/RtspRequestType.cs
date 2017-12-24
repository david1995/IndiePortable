// <copyright file="RtspRequestType.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Rtsp
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public enum RtspRequestType
    {

        Options = 0,

        Describe = 1,

        Setup = 2,

        Play = 3,

        Pause = 4,

        Record = 5,

        Announce = 6,

        Teardown = 7,

        GetParameter = 8,

        SetParameter = 9,

        Redirect = 10
    }
}
