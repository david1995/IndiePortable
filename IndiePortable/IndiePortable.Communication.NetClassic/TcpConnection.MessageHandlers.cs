﻿// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpConnection.MessageHandlers.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the TcpConnection class' message handlers.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.NetClassic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Devices;
    using Devices.ConnectionMessages;
    using EncryptedConnection;


    public partial class TcpConnection
    {
        
        private readonly ConnectionMessageHandler<ConnectionDisconnectRequest> handlerDisconnect;


        private readonly ConnectionMessageHandler<ConnectionMessageKeepAlive> handlerKeepAlive;


        private readonly ConnectionMessageHandler<ConnectionContentMessage> handlerContent;


        private readonly ConnectionMessageHandler<ConnectionEncryptRequest> handlerEncryptRequest;


        private void HandleDisconnect(ConnectionDisconnectRequest rq)
        {
            if (object.ReferenceEquals(rq, null))
            {
                throw new ArgumentNullException(nameof(rq));
            }

            this.SendConnectionMessage(new ConnectionDisconnectResponse(rq));
        }


        private void HandleKeepAlive(ConnectionMessageKeepAlive message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (this.keepAliveSemaphore.CurrentCount == 0)
            {
                this.keepAliveSemaphore.Release();
            }
        }

        
        private void HandleContent(ConnectionContentMessage message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.RaiseMessageReceived(message.Content);
        }


        private void HandleEncryptRequest(ConnectionEncryptRequest rq)
        {
            if (object.ReferenceEquals(rq, null))
            {
                throw new ArgumentNullException(nameof(rq));
            }

            this.SendConnectionMessage(new ConnectionEncryptResponse(this.cryptoManager.LocalPublicKey, rq));
        }
    }
}
