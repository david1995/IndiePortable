// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionMessageBase.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contians the ConnectionMessageBase class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices.ConnectionMessages
{
    using System;
    using System.Runtime.Serialization;
    using Formatter;

    /// <summary>
    /// Represents a message that is used for the communication between
    /// two <see cref="IConnection{TAddress}" /> instances.
    /// This class is abstract.
    /// </summary>
    /// <seealso cref="ISerializable" />
    [Serializable]
    public abstract class ConnectionMessageBase
        : ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMessageBase" /> class.
        /// </summary>
        protected ConnectionMessageBase()
        {
            this.MessageIdentifier = Guid.NewGuid();
            this.SendTime = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMessageBase" /> class.
        /// </summary>
        /// <param name="data">
        /// The <see cref="SerializationInfo"/> providing the values for the <see cref="ConnectionMessageBase"/>.
        /// </param>
        /// <param name="ctx">
        /// The streaming context of the deserialization.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="data" /> is <c>null</c>-</para>
        /// </exception>
        protected ConnectionMessageBase(SerializationInfo data, StreamingContext ctx)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.SendTime = (DateTime)data.GetDateTime(nameof(this.SendTime));
            this.MessageIdentifier = (Guid)data.GetValue(nameof(this.MessageIdentifier), typeof(Guid));
        }

        /// <summary>
        /// Gets the send time of the <see cref="ConnectionMessageBase" />.
        /// </summary>
        /// <value>
        /// Contains the send time of the <see cref="ConnectionMessageBase" />.
        /// </value>
        public DateTime SendTime { get; }

        /// <summary>
        /// Gets the unique identifier of the <see cref="ConnectionMessageBase" />.
        /// </summary>
        /// <value>
        /// Contains the unique identifier of hte <see cref="ConnectionMessageBase" />.
        /// </value>
        public Guid MessageIdentifier { get; }

        /// <summary>
        /// Populates a specified <see cref="SerializationInfo" /> instance with data from the <see cref="ConnectionMessageBase" /> instance.
        /// </summary>
        /// <param name="data">
        /// The <see cref="SerializationInfo" /> that shall be populated.
        /// </param>
        /// <param name="ctx">
        /// The streaming context of the serialization.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        public virtual void GetObjectData(SerializationInfo data, StreamingContext ctx)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            data.AddValue(nameof(this.SendTime), this.SendTime);
            data.AddValue(nameof(this.MessageIdentifier), this.MessageIdentifier);
        }
    }
}
