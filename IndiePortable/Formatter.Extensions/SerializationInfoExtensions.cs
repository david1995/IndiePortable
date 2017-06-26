// <copyright file="SerializationInfoExtensions.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace Formatter.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;

    public static class SerializationInfoExtensions
    {

        public static T GetValue<T>(this SerializationInfo self, string name)
            => self is null
             ? throw new ArgumentNullException(nameof(self))
             : string.IsNullOrEmpty(name)
             ? throw new ArgumentNullException(nameof(name))
             : self.TryGetValue(name, out T t)
             ? t
             : throw new ArgumentException(
                 $"The element specified by {nameof(name)} is not of the specified generic type",
                 nameof(T));

        public static (bool Success, T Value) TryGetValue<T>(this SerializationInfo self, string name)
            => self is null
             ? throw new ArgumentNullException(nameof(self))
             : self.TryGetValue(name, out T t)
             ? (true, t)
             : (false, default(T));

        public static bool TryGetValue<T>(this SerializationInfo self, string name, out T result)
            => !object.Equals(
                result = self is null
                       ? throw new ArgumentNullException(nameof(self))
                       : string.IsNullOrEmpty(name)
                       ? throw new ArgumentNullException(nameof(name))
                       : self.GetValue(name, typeof(object)) is T t
                       ? t
                       : default(T),
                default(T));
    }
}
