// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionDisconnectResponse.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionDisconnectResponse class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices.ConnectionMessages
{
    using Formatter;


    [Serializable]
    public sealed class ConnectionDisconnectResponse
        : ConnectionMessageResponseBase<ConnectionDisconnectRequest>
    {
        
        public ConnectionDisconnectResponse(ConnectionDisconnectRequest request)
            : base(request)
        {
        }


        private ConnectionDisconnectResponse()
        {
        }


        private ConnectionDisconnectResponse(ObjectDataCollection data)
            : base(data)
        {
        }
    }
}
