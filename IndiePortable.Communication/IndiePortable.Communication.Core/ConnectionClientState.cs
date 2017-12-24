// <copyright file="ConnectionClientState.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core
{
    public enum ConnectionClientState
    {
        Constructed = 0,

        Initializing = 1,

        Initialized = 2,

        Connecting = 3,

        Connected = 4,

        ConnectionLost = 5,

        Disconnecting = 6,

        Disconnected = 7,

        Disposing = 8,

        Disposed = 9
    }
}
