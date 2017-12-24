// <copyright file="IMessageConnectionListener{TMessage}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Message
{
    public interface IMessageConnectionListener<TMessage>
        : IMessageConnectionListener<TMessage, TMessage>
        where TMessage : class
    {
    }
}
