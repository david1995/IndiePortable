// <copyright file="DuplexMessageConnection{TMessage}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Message
{
    public abstract class DuplexMessageConnection<TMessage>
        : DuplexMessageConnection<TMessage, TMessage>
        where TMessage : class
    {
    }
}
