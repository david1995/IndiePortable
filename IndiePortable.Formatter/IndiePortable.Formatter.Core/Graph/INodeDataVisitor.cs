// <copyright file="INodeDataVisitor.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Formatter.Core.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface INodeDataVisitor
    {
        void Visit(NullData data);

        void Visit(ObjectData data);

        void Visit(ValueData data);

        void Visit(StringData data);

        void Visit(EnumData data);

        void Visit(ArrayData data);
    }
}
