// <copyright file="INodeData.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Formatter.Core.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface INodeData
        : IEquatable<INodeData>
    {
        void AcceptVisitor(INodeDataVisitor visitor);
    }
}
