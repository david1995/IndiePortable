// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpMessageKeepAlive.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the TcpMessageKeepAlive class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Tcp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Formatter;

    [Serializable]
    public sealed class TcpMessageKeepAlive
        : TcpConnectionMessage
    {

        public TcpMessageKeepAlive()
        {
        }


        private TcpMessageKeepAlive(ObjectDataCollection data)
            : base(data)
        {
        }
    }
}
