// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionDisconnectRequest.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionDisconnectRequest class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices.ConnectionMessages
{
    using Formatter;


    [Serializable]
    public sealed class ConnectionDisconnectRequest
        : ConnectionMessageRequestBase
    {

        public ConnectionDisconnectRequest()
        {
        }


        private ConnectionDisconnectRequest(ObjectDataCollection data)
            : base(data)
        {
        }
    }
}
