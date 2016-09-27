// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpContentMessage.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the TcpContentMessage class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Tcp
{
    using System;
    using Formatter;
    using Messages;


    [Serializable]
    public class TcpContentMessage
        : TcpConnectionMessage
    {
        /// <summary>
        /// The backing field for the <see cref="Content" /> property.
        /// </summary>
        private readonly MessageBase contentBacking;


        public TcpContentMessage(MessageBase content)
        {
            if (object.ReferenceEquals(content, null))
            {
                throw new ArgumentNullException(nameof(content));
            }

            this.contentBacking = content;
        }


        protected TcpContentMessage()
        {
        }


        protected TcpContentMessage(ObjectDataCollection data)
            : base(data)
        {
            if (!data.TryGetValue(nameof(this.Content), out this.contentBacking))
            {
                throw new ArgumentException();
            }
        }


        public MessageBase Content => this.contentBacking;


        public override void GetObjectData(ObjectDataCollection data)
        {
            base.GetObjectData(data);

            data.AddValue(nameof(this.Content), this.Content);
        }
    }
}
