// <copyright file="ISendMessageConnection{TMessage}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using IndiePortable.Communication.Core;

namespace IndiePortable.Communication.Message
{
    public interface ISendMessageConnection<TMessage>
        : IConnection
        where TMessage : class
    {

        void Send(TMessage message);

        Task SendAsync(TMessage message);
    }
}
