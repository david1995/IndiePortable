﻿// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpConnection.MessageHandlers.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the TcpConnection class' connection message handlers.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.UniversalWindows
{
    using System;
    using Devices;
    using Devices.ConnectionMessages;


    public partial class TcpConnection
    {

        private readonly ConnectionMessageHandler<ConnectionDisconnectRequest> handlerDisconnect;


        private readonly ConnectionMessageHandler<ConnectionMessageKeepAlive> handlerKeepAlive;


        private readonly ConnectionMessageHandler<ConnectionContentMessage> handlerContent;


        private void HandleDisconnect(ConnectionDisconnectRequest req)
        {
            if (object.ReferenceEquals(req, null))
            {
                throw new ArgumentNullException(nameof(req));
            }

            this.SendConnectionMessage(new ConnectionDisconnectResponse(req));
        }


        private void HandleKeepAlive(ConnectionMessageKeepAlive message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.lastKeepAlive = DateTime.Now;
        }


        private void HandleContent(ConnectionContentMessage message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.RaiseMessageReceived(message.Content);
        }
    }
}