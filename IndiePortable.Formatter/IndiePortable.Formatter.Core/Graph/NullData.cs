// <copyright file="NullData.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Formatter.Core.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class NullData
        : INodeData
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="NullData"/> class from being created.
        /// </summary>
        private NullData()
        {
        }

        public static INodeData Instance { get; } = new NullData();

        public void AcceptVisitor(INodeDataVisitor visitor)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.Visit(this);
        }

        public bool Equals(INodeData other) => other is NullData;
    }
}
