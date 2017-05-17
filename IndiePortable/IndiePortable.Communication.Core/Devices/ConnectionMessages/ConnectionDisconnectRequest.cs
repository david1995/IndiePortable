// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionDisconnectRequest.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionDisconnectRequest class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices.ConnectionMessages
{
    using System;
    using System.Runtime.Serialization;
    using Formatter;

    /// <summary>
    /// Represents a request for a connection disconnect.
    /// This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ConnectionMessageRequestBase" />
    [Serializable]
    public sealed class ConnectionDisconnectRequest
        : ConnectionMessageRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionDisconnectRequest" /> class.
        /// </summary>
        public ConnectionDisconnectRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionDisconnectRequest" /> class.
        /// </summary>
        /// <param name="data">
        /// The <see cref="SerializationInfo" /> that is used to populate the <see cref="ConnectionMessageRequestBase" /> instance.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="ctx">
        /// The streaming context of the deserialization mechanism.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        private ConnectionDisconnectRequest(SerializationInfo data, StreamingContext ctx)
            : base(data, ctx)
        {
        }
    }
}
