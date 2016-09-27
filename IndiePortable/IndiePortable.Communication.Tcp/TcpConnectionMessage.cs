// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpConnectionMessage.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contians the TcpConnectionMessage class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Tcp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Formatter;
    using Messages;


    [Serializable]
    public abstract class TcpConnectionMessage
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
        /// Initializes a new instance of the <see cref="TcpConnectionMessage" /> class.
        /// </summary>
        protected TcpConnectionMessage()
        {
            this.messageIdentifierBacking = Guid.NewGuid();
            this.sendTimeBacking = DateTime.Now;
        }


        protected TcpConnectionMessage(ObjectDataCollection data)
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
