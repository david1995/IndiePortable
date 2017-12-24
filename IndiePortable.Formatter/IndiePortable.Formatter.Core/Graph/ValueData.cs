// <copyright file="ValueData.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Formatter.Core.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ValueData
        : INodeData
    {
        public ValueData(ValueType value)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ValueType Value { get; }

        public void AcceptVisitor(INodeDataVisitor visitor)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.Visit(this);
        }

        public bool Equals(INodeData other)
            => other is ValueData v
            && ValueDataComparer.EqualityInstance.Equals(this, v);
    }
}
