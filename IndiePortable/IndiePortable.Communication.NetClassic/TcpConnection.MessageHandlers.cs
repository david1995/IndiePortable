// ----------------------------------------------------------------------------------------------------------------------------------------
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
    using Devices;
    using Devices.ConnectionMessages;
    using EncryptedConnection;

    /// <summary>
    /// Contains the message handlers for the <see cref="TcpConnection" /> class.
    /// </summary>
    /// <seealso cref="EncryptedConnection.ICryptableConnection{TAddress}" />
    /// <seealso cref="IDisposable" />
    public partial class TcpConnection
    {
        /// <summary>
        /// The <see cref="ConnectionMessageHandler{T}" /> that handles incoming <see cref="ConnectionDisconnectRequest" /> messages.
        /// </summary>
        private readonly ConnectionMessageHandler<ConnectionDisconnectRequest> handlerDisconnect;

        /// <summary>
        /// The <see cref="ConnectionMessageHandler{T}" /> that handles incoming <see cref="ConnectionMessageKeepAlive" /> messages.
        /// </summary>
        private readonly ConnectionMessageHandler<ConnectionMessageKeepAlive> handlerKeepAlive;

        /// <summary>
        /// The <see cref="ConnectionMessageHandler{T}" /> that handles incoming <see cref="ConnectionContentMessage" /> messages.
        /// </summary>
        private readonly ConnectionMessageHandler<ConnectionContentMessage> handlerContent;

        /// <summary>
        /// The <see cref="ConnectionMessageHandler{T}" /> that handles incoming <see cref="ConnectionEncryptRequest" /> messages.
        /// </summary>
        private readonly ConnectionMessageHandler<ConnectionEncryptRequest> handlerEncryptRequest;


        private void HandleDisconnect(ConnectionDisconnectRequest rq)
        {
            if (object.ReferenceEquals(rq, null))
            {
                throw new ArgumentNullException(nameof(rq));
            }

            try
            {
                this.SendConnectionMessage(new ConnectionDisconnectResponse(rq));
            }
            finally
            {
                this.keepAliveCheckerTask.Stop();
                this.messageReaderTask.Stop();
                this.isConnectedBacking = false;
                this.RaiseDisconnected();
                this.Dispose();
            }
        }


        private void HandleKeepAlive(ConnectionMessageKeepAlive message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.keepAliveWaitHandle.Set();
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
