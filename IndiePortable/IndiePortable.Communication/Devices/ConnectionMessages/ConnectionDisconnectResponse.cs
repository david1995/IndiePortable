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
        ///     This constructor should not be called by a constructor of a deriving class
        ///     other than the parameterless constructor. It is used for de-serializing an object.
        /// </remarks>
        private ConnectionDisconnectResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionDisconnectResponse"/> class.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> that is used
        ///     to populate the <see cref="ConnectionMessageResponseBase{T}" /> instance.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     <para>Thrown if <paramref name="data" /> does not contain the necessary data.</para>
        /// </exception>
        private ConnectionDisconnectResponse(ObjectDataCollection data)
            : base(data)
        {
        }
    }
}
