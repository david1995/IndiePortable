// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBase.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the abstract MessageBase class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Messages
{
    using System;
    using Formatter;

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
        /// The backing field for the <see cref="Identifier" /> property.
        /// </summary>
        private readonly Guid identifierBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase" /> class.
        /// </summary>
        protected MessageBase()
        {
            this.identifierBacking = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase" /> class.
        /// </summary>
        /// <param name="identifier">
        ///     The unique identifier of the <see cref="MessageBase" />.
        /// </param>
        protected MessageBase(Guid identifier)
        {
            this.identifierBacking = identifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase"/> class.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> populated with data for the <see cref="MessageBase" />.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if <paramref name="data" /> does not contain all necessary data to deserialize the object.</para>
        /// </exception>
        protected MessageBase(ObjectDataCollection data)
        {
            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.TryGetValue(nameof(this.Identifier), out this.identifierBacking))
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Gets the unique identifier of the <see cref="MessageBase" />.
        /// </summary>
        /// <value>
        ///     Contains the unique identifier of the <see cref="MessageBase" />.
        /// </value>
        public Guid Identifier
        {
            get { return this.identifierBacking; }
        }

        /// <summary>
        /// Populates a specified <see cref="ObjectDataCollection" /> instance with data from the <see cref="MessageBase" /> instance.
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

            data.AddValue(nameof(this.Identifier), this.Identifier);
        }
    }
}
