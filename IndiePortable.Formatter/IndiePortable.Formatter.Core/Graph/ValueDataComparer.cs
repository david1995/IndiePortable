// <copyright file="ValueDataComparer.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Formatter.Core.Graph
{
    using System;
    using System.Collections.Generic;

    public sealed class ValueDataComparer
        : IEqualityComparer<ValueData>
    {
        private ValueDataComparer()
        {
        }

        public static IEqualityComparer<ValueData> EqualityInstance { get; } = new ValueDataComparer();

        public bool Equals(ValueData x, ValueData y)
            => x?.Value.Equals(y?.Value)
            ?? y == null;

        public int GetHashCode(ValueData obj)
            => obj is null ? throw new ArgumentNullException(nameof(obj))
             : obj.Value.GetHashCode();
    }
}
