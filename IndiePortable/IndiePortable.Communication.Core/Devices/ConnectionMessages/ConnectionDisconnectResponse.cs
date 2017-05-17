// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionDisconnectResponse.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionDisconnectResponse class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices.ConnectionMessages
{
    using System;
    using System.Runtime.Serialization;
    using Formatter;

    /// <summary>
    /// Represents a response to a <see cref="ConnectionDisconnectRequest" /> message.
    /// This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ConnectionMessages.ConnectionMessageResponseBase{T}" />
    [Serializable]
    public sealed class ConnectionDisconnectResponse
        : ConnectionMessageResponseBase<ConnectionDisconnectRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionDisconnectResponse" /> class.
        /// </summary>
        /// <param name="request">
        ///     The request message that shall be responded to.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     <para>Thrown if <paramref name="request" /> is <c>null</c>.</para>
        /// </exception>
        public ConnectionDisconnectResponse(ConnectionDisconnectRequest request)
            : base(request)
        {
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ConnectionDisconnectResponse"/> class from being created.
        /// </summary>
        /// <remarks>
        /// This initializer should not be called by a constructor of a deriving class
        /// other than the parameterless initializer.
        /// It is intended to be used as the constructor for the empty object.
        /// </remarks>
        private ConnectionDisconnectResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionDisconnectResponse"/> class.
        /// </summary>
        /// <param name="data">
        /// The <see cref="SerializationInfo" /> that is used
        /// to populate the <see cref="ConnectionMessageResponseBase{T}" /> instance.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="ctx">
        /// The streaming context of the deserialization mechanism.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        private ConnectionDisconnectResponse(SerializationInfo data, StreamingContext ctx)
            : base(data, ctx)
        {
        }
    }
}
