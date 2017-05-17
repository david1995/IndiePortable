// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionMessageKeepAlive.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionMessageKeepAlive class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices.ConnectionMessages
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a keep-alive connection message.
    /// This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ConnectionMessageBase" />
    [Serializable]
    public sealed class ConnectionMessageKeepAlive
        : ConnectionMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMessageKeepAlive" /> class.
        /// </summary>
        public ConnectionMessageKeepAlive()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMessageKeepAlive"/> class.
        /// </summary>
        /// <param name="data">
        /// The <see cref="SerializationInfo" /> populated with data for initializing the <see cref="ConnectionMessageKeepAlive" />.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="ctx">
        /// The streaming context of the deserialization mechanism.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        private ConnectionMessageKeepAlive(SerializationInfo data, StreamingContext ctx)
            : base(data, ctx)
        {
        }
    }
}
