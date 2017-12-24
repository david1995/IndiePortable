// <copyright file="FieldDataComparer.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Formatter.Core.Graph
{
    using System;
    using System.Collections.Generic;

    public sealed class FieldDataComparer
        : IEqualityComparer<FieldData>
    {
        private FieldDataComparer()
        {
        }

        public static IEqualityComparer<FieldData> EqualityInstance { get; } = new FieldDataComparer();

        public bool Equals(FieldData x, FieldData y)
            => x?.FieldName == y?.FieldName
            && (x?.Value.Equals(y?.Value) ?? y?.Value.Equals(x?.Value) ?? true);

        public int GetHashCode(FieldData obj)
            => obj is null
             ? throw new ArgumentNullException(nameof(obj))
             : obj.FieldName.GetHashCode() + obj.Value.GetHashCode();
    }
}
