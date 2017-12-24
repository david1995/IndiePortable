// <copyright file="DefaultFormatterConverter.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Formatter.Core
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;

    public sealed class DefaultFormatterConverter
        : IFormatterConverter
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="DefaultFormatterConverter"/> class from being created.
        /// </summary>
        private DefaultFormatterConverter()
        {
        }

        public static IFormatterConverter Instance { get; } = new DefaultFormatterConverter();

        public object Convert(object value, Type type)
            => value is null
             ? throw new ArgumentNullException(nameof(type))
             : type is null
             ? throw new ArgumentNullException(nameof(type))
             : type.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo())
             ? value
             : throw new InvalidOperationException();

        public object Convert(object value, TypeCode typeCode) => throw new NotImplementedException();

        public bool ToBoolean(object value) => value is bool b ? b : throw new ArgumentException("value must be a boolean.", nameof(value));

        public byte ToByte(object value) => value is byte b ? b : throw new ArgumentException("value must be a byte.", nameof(value));

        public char ToChar(object value) => value is char c ? c : throw new ArgumentException("value must be a char.", nameof(value));

        public DateTime ToDateTime(object value) => value is DateTime d ? d : throw new ArgumentException("value must be a DateTime.", nameof(value));

        public decimal ToDecimal(object value) => value is decimal d ? d : throw new ArgumentException("value must be a decimal.", nameof(value));

        public double ToDouble(object value) => value is double d ? d : throw new ArgumentException("value must be a double.", nameof(value));

        public short ToInt16(object value) => value is short s ? s : throw new ArgumentException("value must be an int16.", nameof(value));

        public int ToInt32(object value) => value is int i ? i : throw new ArgumentException("value must be an int32.", nameof(value));

        public long ToInt64(object value) => value is long l ? l : throw new ArgumentException("value must be an int64.", nameof(value));

        public sbyte ToSByte(object value) => value is sbyte s ? s : throw new ArgumentException("value must be an sbyte.", nameof(value));

        public float ToSingle(object value) => value is float f ? f : throw new ArgumentException("value must be a float.", nameof(value));

        public string ToString(object value) => value is string s ? s : value?.ToString() ?? string.Empty;

        public ushort ToUInt16(object value) => value is ushort u ? u : throw new ArgumentException("value must be a uint16.", nameof(value));

        public uint ToUInt32(object value) => value is uint u ? u : throw new ArgumentException("value must be a uint32.", nameof(value));

        public ulong ToUInt64(object value) => value is ulong u ? u : throw new ArgumentException("value must be a uint64.", nameof(value));
    }
}
