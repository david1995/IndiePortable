// <copyright file="ContentTypeHeaderField.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Connections.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ContentTypeHeaderField
        : HttpHeaderField
    {
        public ContentTypeHeaderField(string fieldName)
            : base(fieldName)
        {
            this.Values = new List<ContentType>();
        }

        public IList<ContentType> Values { get; protected set; }

        public override string GetRawValue()
            => string.Join(", ", this.Values.Select(v => v.FullName));

        public override void SetRawValue(string value)
            => this.Values = new List<ContentType>(value.Split(',')
                                                        .Select(v => v.Trim())
                                                        .Where(v => ContentType.IsContentTypeString(v))
                                                        .Select(v => new ContentType(v)));
    }
}
