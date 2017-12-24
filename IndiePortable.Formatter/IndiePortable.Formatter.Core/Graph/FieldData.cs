// <copyright file="FieldData.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Formatter.Core.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class FieldData
        : IEquatable<FieldData>
    {
        public FieldData(string fieldName, INodeData value)
        {
            this.FieldName = string.IsNullOrEmpty(fieldName)
                           ? throw new ArgumentNullException(nameof(fieldName))
                           : fieldName;

            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string FieldName { get; }

        public INodeData Value { get; }

        public static bool Equals(FieldData x, FieldData y)
            => FieldDataComparer.EqualityInstance.Equals(x, y);

        public bool Equals(FieldData other) => Equals(this, other);

        public void Deconstruct(out string fieldName, out INodeData value)
        {
            fieldName = this.FieldName;
            value = this.Value;
        }
    }
}
