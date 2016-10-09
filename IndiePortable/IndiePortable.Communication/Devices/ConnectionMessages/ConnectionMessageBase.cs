// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionMessageBase.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contians the ConnectionMessageBase class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices.ConnectionMessages
{
    using System;
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
        /// The backing field for the <see cref="SendTime" /> property.
        /// </summary>
        private readonly DateTime sendTimeBacking;

        /// <summary>
        /// The backing field for the <see cref="MessageIdentifier" /> property.
        /// </summary>
        private readonly Guid messageIdentifierBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMessageBase" /> class.
        /// </summary>
        protected ConnectionMessageBase()
        {
            this.messageIdentifierBacking = Guid.NewGuid();
            this.sendTimeBacking = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMessageBase" /> class.
        /// </summary>
        /// <param name="data">
        ///     
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>-</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if <paramref name="data" /> does not contain the necessary information.</para>
        /// </exception>
        protected ConnectionMessageBase(ObjectDataCollection data)
        {
            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.TryGetValue(nameof(this.SendTime), out this.sendTimeBacking) ||
                !data.TryGetValue(nameof(this.MessageIdentifier), out this.messageIdentifierBacking))
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Gets the send time of the <see cref="ConnectionMessageBase" />.
        /// </summary>
        /// <value>
        ///     Contains the send time of the <see cref="ConnectionMessageBase" />.
        /// </value>
        public DateTime SendTime => this.sendTimeBacking;

        /// <summary>
        /// Gets the unique identifier of the <see cref="ConnectionMessageBase" />.
        /// </summary>
        /// <value>
        ///     Contains the unique identifier of hte <see cref="ConnectionMessageBase" />.
        /// </value>
        public Guid MessageIdentifier => this.messageIdentifierBacking;

        /// <summary>
        /// Populates a specified <see cref="ObjectDataCollection" /> instance with data from the <see cref="ConnectionMessageBase" /> instance.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> that shall be populated.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        public virtual void GetObjectData(ObjectDataCollection data)
        {
            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            data.AddValue(nameof(this.SendTime), this.sendTimeBacking);
            data.AddValue(nameof(this.MessageIdentifier), this.messageIdentifierBacking);
        }
    }
}
