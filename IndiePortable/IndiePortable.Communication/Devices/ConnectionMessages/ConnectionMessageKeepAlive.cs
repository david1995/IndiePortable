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
    using Formatter;

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
        ///     The <see cref="ObjectDataCollection" /> populated with data for initializing the <see cref="ConnectionMessageKeepAlive" />.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     <para>Thrown if <paramref name="data" /> does not contain the necessary information.</para>
        /// </exception>
        private ConnectionMessageKeepAlive(ObjectDataCollection data)
            : base(data)
        {
        }
    }
}
