﻿// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializableSurrogate.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the SerializableSurrogate class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides a serialization surrogate for all types that are marked with the <see cref="SerializableAttribute" /> attribute
    /// and do not implement <see cref="ISerializable" />.
    /// </summary>
    /// <remarks>
    ///     Implements <see cref="ISurrogate{T}" />.
    /// </remarks>
    public sealed class SerializableSurrogate
        : ISurrogate
    {
        /// <summary>
        /// The backing field for the static <see cref="Default" /> property.
        /// </summary>
        private static readonly ISurrogate DefaultBacking = new SerializableSurrogate();

        /// <summary>
        /// Gets the default <see cref="SerializableSurrogate" />.
        /// </summary>
        /// <value>
        ///     Contains the default <see cref="SerializableSurrogate" />.
        /// </value>
        public static ISurrogate Default
        {
            get { return DefaultBacking; }
        }


        public Type TargetType { get { return typeof(object); } }

        /// <summary>
        /// Retrieves data from an <see cref="object" /> whose type is marked with the <see cref="SerializableAttribute" /> attribute.
        /// </summary>
        /// <param name="value">
        ///     The <see cref="object" /> populated with data.
        /// </param>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> instance that shall be populated with data from <paramref name="value" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown when:</para>
        ///     <para>  - <paramref name="value" /> is <c>null</c>.</para>
        ///     <para>  - <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown when:</para>
        ///     <para>  - the type of <paramref name="value" /> is not marked with the <see cref="SerializableAttribute" /> attribute.</para>
        ///     <para>  - the type of <paramref name="value" /> implements the <see cref="ISerializable" /> interface.</para>
        /// </exception>
        /// <remarks>
        ///     <para>Implements <see cref="ISurrogate.GetData(T, ObjectDataCollection)" /> explicitly.</para>
        ///     <para>
        ///         The type of <paramref name="value" /> must be marked with the <see cref="SerializableAttribute" /> attribute,
        ///         but must not implement the <see cref="ISerializable" /> interface.
        ///     </para>
        /// </remarks>
        void ISurrogate.GetData(object value, ObjectDataCollection data)
        {
            // throw exception if value is null
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // throw exception if data is null
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var type = value?.GetType().GetTypeInfo();

            // throw exception if type is not marked with SerializableAttribute
            if (!GraphIterator.IsMarkedSerializable(type))
            {
                throw new ArgumentException(
                    $"The type of {nameof(value)} must be marked with the {nameof(SerializableAttribute)} attribute.",
                    nameof(value));
            }

            // throw exception if type implement ISerializable
            if (GraphIterator.ImplementsISerializable(type))
            {
                throw new ArgumentException(
                    $"The type of {nameof(value)} must not implement the {nameof(ISerializable)} interface.",
                    nameof(value));
            }

            var typeVersion = GetTypeVersion(type);

            // if type does not implement ISerializable
            foreach (var field in type.DeclaredFields.Where(f => f.CustomAttributes.Any(a => a.AttributeType == typeof(SerializedAttribute))))
            {
                var fieldVersionSpans = GetFieldVersionSpans(field);
                var versionSpans = (from vs in fieldVersionSpans
                                    where vs.Item1 <= typeVersion && typeVersion <= vs.Item2
                                    select vs).ToArray();

                // if the field has no version spans or at least one version span could be found
                // if no matching version span could be found -> simply don't use the field's value
                if (fieldVersionSpans.Count() == 0 || versionSpans.Length >= 1)
                {
                    data.AddValue(field.Name, field.GetValue(value));
                }
            }
        }

        /// <summary>
        /// Retrieves data from an <see cref="ObjectDataCollection" /> instance and uses it for populating an object.
        /// </summary>
        /// <param name="value">
        ///     The target <see cref="object" /> that shall be populated with data.
        /// </param>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> populated with data.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown when:</para>
        ///     <para>  - <paramref name="value" /> is <c>null</c>.</para>
        ///     <para>  - <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown when:</para>
        ///     <para>  - the type of <paramref name="value" /> is not marked with the <see cref="SerializableAttribute" /> attribute.</para>
        ///     <para>  - the type of <paramref name="value" /> implements the <see cref="ISerializable" /> interface.</para>
        /// </exception>
        /// <remarks>
        ///     <para>Implements <see cref="ISurrogate.SetData(T, ObjectDataCollection)" /> explicitly.</para>
        ///     <para>
        ///         The type of <paramref name="value" /> must be marked with the <see cref="SerializableAttribute" /> attribute,
        ///         but must not implement the <see cref="ISerializable" /> interface.
        ///     </para>
        /// </remarks>
        void ISurrogate.SetData(ref object value, ObjectDataCollection data)
        {
            // throw exception if value is null
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // throw exception if data is null
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var type = value?.GetType().GetTypeInfo();
            
            // throw exception if type is not marked with SerializableAttribute
            if (!GraphIterator.IsMarkedSerializable(type))
            {
                throw new ArgumentException(
                    $"The type of {nameof(value)} must be marked with the {nameof(SerializableAttribute)} attribute.",
                    nameof(value));
            }

            // throw exception if type implement ISerializable
            if (GraphIterator.ImplementsISerializable(type))
            {
                throw new ArgumentException(
                    $"The type of {nameof(value)} must not implement the {nameof(ISerializable)} interface.",
                    nameof(value));
            }

            var typeVersion = GetTypeVersion(type);
            foreach (var field in type.DeclaredFields)
            {
                var fieldVersionSpans = GetFieldVersionSpans(field);
                var versionSpans = (from vs in fieldVersionSpans
                                    where vs.Item1 <= typeVersion && typeVersion <= vs.Item2
                                    select vs).ToArray();

                // if the field has no version spans or at least one version span could be found
                // if no matching version span could be found -> simply don't set the field's value
                if (fieldVersionSpans.Count() == 0 || versionSpans.Length >= 1)
                {
                    // the deserialization data MUST contain each field (with correct version) of the type
                    if (!data.ContainsKey(field.Name))
                    {
                        throw new InvalidOperationException(
                            $@"The field ""{field.Name}"" of the type {type.AssemblyQualifiedName} could not be found in the specified {nameof(ObjectDataCollection)}.");
                    }

                    field.SetValue(value, data.GetValue(field.Name));
                }
            }
        }

        /// <summary>
        /// Gets the serialization version of a <see cref="Type" />.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="TypeInfo" /> providing more information about a <see cref="Type" />.
        /// </param>
        /// <returns>
        ///     Returns the version of the specified <see cref="Type" />.
        /// </returns>
        private static Version GetTypeVersion(TypeInfo info)
            => info.CustomAttributes.Any(a => a.AttributeType == typeof(SerializedVersionAttribute))
                    ? info.GetCustomAttribute<SerializedVersionAttribute>().Version
                    : null;

        /// <summary>
        /// Retrieves the <see cref="Version" /> spans of a specified field.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="FieldInfo" /> that shall be used.
        /// </param>
        /// <returns>
        ///     Returns the <see cref="Version" /> spans of a specified field.
        /// </returns>
        private static IEnumerable<Tuple<Version, Version>> GetFieldVersionSpans(FieldInfo info)
            => from attr in info.GetCustomAttributes<SerializedFieldVersionAttribute>()
               select new Tuple<Version, Version>(attr.MinVersion, attr.MaxVersion);
    }
}