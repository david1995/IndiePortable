// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionMessageHandler.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionMessageHandler{T} class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices
{
    using System;
    using ConnectionMessages;


    public class ConnectionMessageHandler<T>
        : IConnectionMessageHandler
        where T : ConnectionMessageBase
    {

        private Action<T> messageHandler;


        public ConnectionMessageHandler(Action<T> messageHandler)
        {
            if (object.ReferenceEquals(messageHandler, null))
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }

            this.messageHandler = messageHandler;
        }


        public Type MessageType => typeof(T);


        public void HandleMessage(ConnectionMessageBase message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            var msg = message as T;
            if (object.ReferenceEquals(msg, null))
            {
                throw new ArgumentException();
            }

            this.HandleMessage(msg);
        }


        public void HandleMessage(T message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.messageHandler(message);
        }
    }
}
