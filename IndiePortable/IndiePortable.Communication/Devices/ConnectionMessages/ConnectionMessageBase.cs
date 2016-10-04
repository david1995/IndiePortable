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


        public DateTime SendTime => this.sendTimeBacking;


        public Guid MessageIdentifier => this.messageIdentifierBacking;


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
