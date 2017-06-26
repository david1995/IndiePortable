// <copyright file="AsymmetricConnectionDecorator{TSend,TReceive,TAddress}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core.Devices.Connections.Decorators
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public abstract class AsymmetricConnectionDecorator<TSend, TReceive, TAddress>
        : AsymmetricConnectionBase<TSend, TReceive, TAddress>
        where TSend : class
        where TReceive : class
    {
        protected AsymmetricConnectionDecorator(StreamConnectionBase<TAddress> decoratedConnection)
        {
            this.DecoratedConnection = decoratedConnection ?? throw new ArgumentNullException(nameof(decoratedConnection));
        }

        protected StreamConnectionBase<TAddress> DecoratedConnection { get; }
    }
}
