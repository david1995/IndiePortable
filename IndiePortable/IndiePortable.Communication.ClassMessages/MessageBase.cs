// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBase.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the abstract MessageBase class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.ClassMessages
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the base class of all messages.
    /// This class is abstract.
    /// </summary>
    /// <seealso cref="ISerializable" />
    [Serializable]
    public abstract class MessageBase
        : ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase" /> class.
        /// </summary>
        protected MessageBase()
        {
            this.Identifier = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase" /> class.
        /// </summary>
        /// <param name="identifier">
        /// The unique identifier of the <see cref="MessageBase" />.
        /// </param>
        protected MessageBase(Guid identifier)
        {
            this.Identifier = identifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase"/> class.
        /// </summary>
        /// <param name="data">
        /// The <see cref="SerializationInfo" /> populated with data for the <see cref="MessageBase" />.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="ctx">
        /// The streaming context of the deserialization mechanism.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        protected MessageBase(SerializationInfo data, StreamingContext ctx)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.Identifier = (Guid)data.GetValue(nameof(this.Identifier), typeof(Guid));
        }

        /// <summary>
        /// Gets the unique identifier of the <see cref="MessageBase" />.
        /// </summary>
        /// <value>
        ///     Contains the unique identifier of the <see cref="MessageBase" />.
        /// </value>
        public Guid Identifier { get; }

        /// <summary>
        /// Populates a specified <see cref="SerializationInfo" /> instance with data from the <see cref="MessageBase" /> instance.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="SerializationInfo" /> that shall be populated.
        /// </param>
        /// <param name="ctx">
        ///     The streaming context of the serialization mechanism.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        public virtual void GetObjectData(SerializationInfo data, StreamingContext ctx)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            data.AddValue(nameof(this.Identifier), this.Identifier);
        }
    }
}
