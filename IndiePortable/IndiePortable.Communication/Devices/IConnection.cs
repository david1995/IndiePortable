// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnection.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IConnection&lt;T&gt; interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    public interface IConnection<TAddress>
        : IMessageTransciever, IDisposable
        where TAddress : IAddressInfo
    {

        bool IsConnected { get; }


        bool IsActivated { get; }


        TAddress RemoteAddress { get; }


        void Activate();


        void Disconnect();
    }
}
