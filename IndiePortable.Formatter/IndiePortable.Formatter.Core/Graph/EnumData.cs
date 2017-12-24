// <copyright file="EnumData.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Formatter.Core.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class EnumData
        : INodeData
    {
        public EnumData(Enum value)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Enum Value { get; }

        public void AcceptVisitor(INodeDataVisitor visitor)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.Visit(this);
        }

        public bool Equals(INodeData other)
            => other is EnumData e
            && this.Value.GetType() == e.Value.GetType()
            && this.Value == e.Value;
    }
}
