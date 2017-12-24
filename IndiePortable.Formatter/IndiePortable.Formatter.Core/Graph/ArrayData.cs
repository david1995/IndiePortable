// <copyright file="ArrayData.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Formatter.Core.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    public class ArrayData
        : INodeData
    {
        public ArrayData(int length, IEnumerable<INodeData> values)
        {
            this.Length = length < 0
                        ? throw new ArgumentOutOfRangeException(nameof(length))
                        : length;

            this.Values = ImmutableArray.CreateRange(
                values is null
                ? throw new ArgumentNullException(nameof(values))
                : values.Take(length));
        }

        public IImmutableList<INodeData> Values { get; }

        public int Length { get; }

        public void AcceptVisitor(INodeDataVisitor visitor)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.Visit(this);
        }

        public bool Equals(INodeData other)
            => other is ArrayData a
            && this.Length == a.Length
            && this.Values.SequenceEqual(a.Values);
    }
}
