// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionMessageRequestBase.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionMessageRequestBase class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices.ConnectionMessages
{
    using System;
    using System.Runtime.Serialization;
    using Formatter;

    /// <summary>
    /// Represents a request connection message.
    /// This class is abstract.
    /// </summary>
    /// <seealso cref="ConnectionMessageBase" />
    [Serializable]
    public abstract class ConnectionMessageRequestBase
        : ConnectionMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMessageRequestBase" /> class.
        /// </summary>
        protected ConnectionMessageRequestBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMessageRequestBase"/> class.
        /// </summary>
        /// <param name="data">
        /// The <see cref="SerializationInfo" /> that is used
        /// to populate the <see cref="ConnectionMessageRequestBase" /> instance.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="ctx">
        /// The streaming context of the deserialization.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>Thrown if <paramref name="data" /> does not contain the necessary information.</para>
        /// </exception>
        protected ConnectionMessageRequestBase(SerializationInfo data, StreamingContext ctx)
            : base(data, ctx)
        {
        }
    }
}
