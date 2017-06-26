// <copyright file="HttpHeaderCollection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Connections.Http
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    public class HttpHeaderCollection
        : IEnumerable<HttpHeaderField>
    {
        private readonly Dictionary<string, HttpHeaderField> backingStorage;

        public string GetRawValue(string key)
        {
            if (this.backingStorage.ContainsKey(key))
            {
                return this.backingStorage[key].GetRawValue();
            }

            throw new KeyNotFoundException();
        }

        public T GetValue<T>(string key)
        {
            if (!this.backingStorage.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }

            object value;
            switch (this.backingStorage[key])
            {
                case DateTimeHeaderField dateTime:
                    value = dateTime.Value;
                    break;

                default:
                    throw new NotImplementedException();
            }

            return value is T t
                 ? t
                 : throw new ArgumentException();
        }

        public (bool Success, T Value) TryGetValue<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void SetRawValue(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void SetValue<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public object this[string key]
        {
            get => this.backingStorage.ContainsKey(key)
                 ? this.backingStorage[key].Value
                 : null;

            set
            {
                if (this.backingStorage.ContainsKey(key))
                {
                    if (value is null)
                    {
                        this.backingStorage.Remove(key);
                    }
                    else
                    {
                        this.backingStorage[key].Value = value;
                    }
                }
                else if (!(value is null))
                {
                    var type = value.GetType();
                    var typeInfo = type.GetTypeInfo();

                    var targetType = typeof(HttpHeaderField<>).MakeGenericType(type);
                    var headerField = (IHttpHeaderField)Activator.CreateInstance(targetType, key);
                    headerField.HasValue = true;
                    headerField.Value = value;

                    this.backingStorage.Add(key, headerField);
                }
            }
        }

        public void Clear() => this.backingStorage.Clear();

        /// <inheritdoc/>
        public IEnumerator<HttpHeaderField> GetEnumerator() => this.backingStorage.Values.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
