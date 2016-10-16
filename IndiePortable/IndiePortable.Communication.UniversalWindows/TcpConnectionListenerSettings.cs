// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpConnectionListenerSettings.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the TcpConnectionListenerSettings class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.UniversalWindows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Tcp;

    public sealed class TcpConnectionListenerSettings
    {
        /// <summary>
        /// The backing field for the <see cref="ListenerNetworkAdapters" /> property.
        /// </summary>
        private readonly IEnumerable<IPPortAddressInfo> listenerNetworkAdaptersBacking;


        public TcpConnectionListenerSettings(ushort listenerPort)
        {
            this.listenerNetworkAdaptersBacking = new[] { new IPPortAddressInfo(new byte[4], listenerPort) };
        }


        public TcpConnectionListenerSettings(params IPPortAddressInfo[] listenerNetworkAdapters)
        {
            if (object.ReferenceEquals(listenerNetworkAdapters, null))
            {
                throw new ArgumentNullException(nameof(listenerNetworkAdapters));
            }

            this.listenerNetworkAdaptersBacking = listenerNetworkAdapters.ToArray();
        }


        public TcpConnectionListenerSettings(IEnumerable<IPPortAddressInfo> listenerNetworkAdapters)
        {
            if (object.ReferenceEquals(listenerNetworkAdapters, null))
            {
                throw new ArgumentNullException(nameof(listenerNetworkAdapters));
            }

            this.listenerNetworkAdaptersBacking = listenerNetworkAdapters.ToArray();
        }


        public IEnumerable<IPPortAddressInfo> ListenerNetworkAdapters => this.listenerNetworkAdaptersBacking;
    }
}
