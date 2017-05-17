// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionContentMessage.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionContentMessage class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices.ConnectionMessages
{
    using System;
    using System.Runtime.Serialization;
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
        /// Initializes a new instance of the <see cref="ConnectionContentMessage" /> class.
        /// </summary>
        /// <param name="content">
        /// The <see cref="MessageBase" /> that shall be encapsulated.
        /// Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="content" /> is <c>null</c>.</para>
        /// </exception>
        public ConnectionContentMessage(MessageBase content)
        {
            this.Content = content ?? throw new ArgumentNullException(nameof(content));
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
        /// The <see cref="SerializationInfo" /> instance populated with data for initializing the <see cref="ConnectionContentMessage" /> instance.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="ctx">
        /// The streaming context of the deserialization mechanism.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        protected ConnectionContentMessage(SerializationInfo data, StreamingContext ctx)
            : base(data, ctx)
        {
            this.Content = (MessageBase)data.GetValue(nameof(this.Content), typeof(MessageBase));
        }

        /// <summary>
        /// Gets the encapsulated <see cref="MessageBase" />.
        /// </summary>
        /// <value>
        /// Contains the encapsulated <see cref="MessageBase" />.
        /// </value>
        public MessageBase Content { get; }

        /// <summary>
        /// Populates a specified <see cref="SerializationInfo" /> instance with data from the <see cref="ConnectionMessageBase" /> instance.
        /// </summary>
        /// <param name="data">
        /// The <see cref="SerializationInfo" /> that shall be populated.
        /// </param>
        /// <param name="ctx">
        /// The streaming context of the serialization mechanism.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        public override void GetObjectData(SerializationInfo data, StreamingContext ctx)
        {
            base.GetObjectData(data, ctx);

            data.AddValue(nameof(this.Content), this.Content);
        }
    }
}
