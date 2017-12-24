// <copyright file="ObjectData.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Formatter.Core.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    public class ObjectData
        : INodeData
    {
        public ObjectData(IEnumerable<FieldData> fields)
        {
            this.Fields = fields ?? throw new ArgumentNullException(nameof(fields));
        }

        public IEnumerable<FieldData> Fields { get; }

        public void AcceptVisitor(INodeDataVisitor visitor)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.Visit(this);
        }

        public bool Equals(INodeData other)
            => other is ObjectData o
            && this.Fields.All(f => o.Fields.Contains(f));
    }
}
