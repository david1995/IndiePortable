// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionContentMessage.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionContentMessage class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices.ConnectionMessages
{
    using System;
    using Formatter;
    using Messages;

    /// <summary>
    /// Represents a connection message with an encapsulated message.
    /// </summary>
    /// <seealso cref="ConnectionMessageBase" />
    [Serializable]
    public class ConnectionContentMessage
        : ConnectionMessageBase
    {
        /// <summary>
        /// The backing field for the <see cref="Content" /> property.
        /// </summary>
        private readonly MessageBase contentBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionContentMessage" /> class.
        /// </summary>
        /// <param name="content">
        ///     The <see cref="MessageBase" /> that shall be encapsulated.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="content" /> is <c>null</c>.</para>
        /// </exception>
        public ConnectionContentMessage(MessageBase content)
        {
            if (object.ReferenceEquals(content, null))
            {
                throw new ArgumentNullException(nameof(content));
            }

            this.contentBacking = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionContentMessage" /> class.
        /// </summary>
        protected ConnectionContentMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionContentMessage"/> class.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> instance populated with data
        ///     for initializing the <see cref="ConnectionContentMessage" /> instance.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        protected ConnectionContentMessage(ObjectDataCollection data)
            : base(data)
        {
            if (!data.TryGetValue(nameof(this.Content), out this.contentBacking))
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Gets the encapsulated <see cref="MessageBase" />.
        /// </summary>
        /// <value>
        ///     Contains the encapsulated <see cref="MessageBase" />.
        /// </value>
        public MessageBase Content => this.contentBacking;

        /// <summary>
        /// Populates a specified <see cref="ObjectDataCollection" /> instance with data from the <see cref="ConnectionMessageBase" /> instance.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> that shall be populated.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        ///     <para>Overrides <see cref="ConnectionMessageBase.GetObjectData(ObjectDataCollection)" />.</para>
        /// </remarks>
        public override void GetObjectData(ObjectDataCollection data)
        {
            base.GetObjectData(data);

            data.AddValue(nameof(this.Content), this.Content);
        }
    }
}
