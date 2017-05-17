// <copyright file="ConnectionState.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core.Devices
{
    public enum ConnectionState
    {

        Initializing = 0,

        Initialized = 1,

        Activating = 2,

        Activated = 3,

        Disconnecting = 4,

        Disconnected = 5,

        Lost = 6,

        Disposing = 7,

        Disposed = 8
    }
}
