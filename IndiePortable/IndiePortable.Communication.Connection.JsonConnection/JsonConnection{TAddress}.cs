// <copyright file="JsonConnection{TAddress}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Connection.JsonConnection
{
    using System;
    using System.Threading.Tasks;
    using IndiePortable.Communication.ClassMessages;
    using IndiePortable.Communication.Core.Devices.Connections;

    public class JsonConnection<TAddress>
        : MessageConnectionBase<MessageBase, TAddress>
    {
        public override TAddress RemoteAddress => throw new NotImplementedException();

        protected override void ActivateOverride() => throw new NotImplementedException();

        protected override void DisconnectOverride() => throw new NotImplementedException();

        protected override async Task DisconnectAsyncOverride() => throw new NotImplementedException();

        protected override void SendOverride(MessageBase message) => throw new NotImplementedException();

        protected override async Task SendAsyncOverride(MessageBase message) => throw new NotImplementedException();
    }
}
