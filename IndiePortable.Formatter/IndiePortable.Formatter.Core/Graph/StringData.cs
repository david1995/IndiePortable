// <copyright file="StringData.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Formatter.Core.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class StringData
        : INodeData
    {
        public StringData(string value)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Value { get; }

        public void AcceptVisitor(INodeDataVisitor visitor)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.Visit(this);
        }

        public bool Equals(INodeData other)
            => other is StringData s
            && StringDataComparer.EqualityInstance.Equals(this, s);
    }
}
