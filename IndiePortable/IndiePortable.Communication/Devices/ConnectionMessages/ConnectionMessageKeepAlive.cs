// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionMessageKeepAlive.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionMessageKeepAlive class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices.ConnectionMessages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Formatter;


    [Serializable]
    public sealed class ConnectionMessageKeepAlive
        : ConnectionMessageBase
    {

        public ConnectionMessageKeepAlive()
        {
        }


        private ConnectionMessageKeepAlive(ObjectDataCollection data)
            : base(data)
        {
        }
    }
}
