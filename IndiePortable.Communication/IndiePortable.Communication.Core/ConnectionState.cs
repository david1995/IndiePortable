// <copyright file="ConnectionState.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core
{
    public enum ConnectionState
    {

        Constructed = 0,

        Initializing = 1,

        Initialized = 2,

        Activating = 3,

        Activated = 4,

        Disconnecting = 5,

        Disconnected = 6,

        Lost = 7,

        Disposing = 8,

        Disposed = 9
    }
}
