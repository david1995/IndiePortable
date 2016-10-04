// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IPPortAddressInfo.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IPPortAddressInfo class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Tcp
{
    using System;
    using Devices;
    using Formatter;


    [Serializable]
    public sealed class IPPortAddressInfo
        : IAddressInfo
    {
        /// <summary>
        /// The backing field for the <see cref="Address" /> property.
        /// </summary>
        private readonly byte[] addressBacking;

        /// <summary>
        /// The backing field for the <see cref="Port" /> property.
        /// </summary>
        private readonly uint portBacking;


        public IPPortAddressInfo(byte[] address, uint port)
        {
            if (object.ReferenceEquals(address, null))
            {
                throw new ArgumentNullException(nameof(address));
            }

            this.addressBacking = address;
            this.portBacking = port;
        }



        public byte[] Address => this.addressBacking;


        public uint Port => this.portBacking;
    }
}
