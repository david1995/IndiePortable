// <copyright file="HttpHeaderField.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Connections.Http
{
    using System;

    public abstract class HttpHeaderField
    {
        protected HttpHeaderField(string fieldName)
        {
            this.FieldName = string.IsNullOrEmpty(fieldName)
                      ? throw new ArgumentNullException(nameof(fieldName))
                      : fieldName;
        }

        public string FieldName { get; }

        public abstract string GetRawValue();

        public abstract void SetRawValue(string value);
    }
}
