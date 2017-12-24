// <copyright file="StringDataComparer.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Formatter.Core.Graph
{
    using System;
    using System.Collections.Generic;

    public sealed class StringDataComparer
        : IEqualityComparer<StringData>
    {
        /// <summary>
        /// The hash code to apply if the passed <see cref="StringData"/> instance is <c>null</c>.
        /// </summary>
        public const int NullHashCode = 0;

        private StringDataComparer()
        {
        }

        public static IEqualityComparer<StringData> EqualityInstance { get; } = new StringDataComparer();

        public bool Equals(StringData x, StringData y)
            => x?.Value == y?.Value;

        public int GetHashCode(StringData obj)
            => obj is null
             ? throw new ArgumentNullException(nameof(obj))
             : obj.Value?.GetHashCode() ?? NullHashCode;
    }
}
