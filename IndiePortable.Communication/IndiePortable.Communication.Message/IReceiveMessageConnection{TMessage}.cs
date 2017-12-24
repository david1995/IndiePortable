// <copyright file="IReceiveMessageConnection{TMessage}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using IndiePortable.Communication.Core;

namespace IndiePortable.Communication.Message
{
    public interface IReceiveMessageConnection<out TMessage>
        : IConnection
        where TMessage : class
    {
        event Action<IReceiveMessageConnection<TMessage>, TMessage> MessageReceived;
    }
}
