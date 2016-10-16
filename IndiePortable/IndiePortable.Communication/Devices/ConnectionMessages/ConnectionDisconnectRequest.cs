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
        ///     The <see cref="ObjectDataCollection" /> that is used
        ///     to populate the <see cref="ConnectionMessageRequestBase" /> instance.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     <para>Thrown if <paramref name="data" /> does not contain the necessary data.</para>
        /// </exception>
        private ConnectionDisconnectRequest(ObjectDataCollection data)
            : base(data)
        {
        }
    }
}
