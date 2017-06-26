// <copyright file="StreamConnectionDecorator{TAddress}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core.Devices.Connections.Decorators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public abstract class StreamConnectionDecorator<TAddress>
        : StreamConnectionBase<TAddress>
    {
        protected StreamConnectionDecorator(StreamConnectionBase<TAddress> decoratedConnection)
        {
            this.DecoratedConnection = decoratedConnection ?? throw new ArgumentNullException(nameof(decoratedConnection));
            this.DataStream = this.DecoratedConnection.DataStream;
        }

        public override Stream DataStream { get; }

        public StreamConnectionBase<TAddress> DecoratedConnection { get; }
    }
}
