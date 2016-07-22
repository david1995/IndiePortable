// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContractSurrogate.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the DataContractSurrogate class
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides a serialization surrogate for all types that are marked with the <see cref="DataContractAttribute" /> attribute.
    /// </summary>
    /// <remarks>
    ///     Implements <see cref="ISurrogate{T}" /> explicitly.
    /// </remarks>
    public sealed class DataContractSurrogate
        : ISurrogate
    {
        /// <summary>
        /// The backing field for the static <see cref="Default" /> property.
        /// </summary>
        private static readonly ISurrogate DefaultBacking = new DataContractSurrogate();

        /// <summary>
        /// Gets the default <see cref="DataContractSurrogate" />.
        /// </summary>
        /// <value>
        ///     Contains the default <see cref="DataContractSurrogate" />.
        /// </value>
        public static ISurrogate Default
        {
            get { return DefaultBacking; }
        }


        public Type TargetType { get { return typeof(object); } }

        /// <summary>
        /// Retrieves data from an <see cref="object" /> whose type is marked with the <see cref="DataContractAttribute" /> attribute.
        /// </summary>
        /// <param name="value">
        ///     The source <see cref="object" /> populated with data.
        /// </param>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> that shall be populated with data from <paramref name="value" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown when:</para>
        ///     <para>  - <paramref name="value" /> is null.</para>
        ///     <para>  - <paramref name="data" /> is null.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown when the type of <paramref name="value" /> is not marked with the <see cref="DataContractAttribute" /> attribute.</para>
        /// </exception>
        /// <exception cref="SerializationException">
        ///     <para>Thrown when a property does not have a getter and a setter method.</para>
        /// </exception>
        /// <remarks>
        ///     <para>Implements <see cref="ISurrogate.GetData(object, ObjectDataCollection)" /> explicitly.</para>
        ///     <para>
        ///         The type of <paramref name="value" /> must be marked with the <see cref="DataContractAttribute" /> attribute.
        ///         and properties and fields that shall be (de-)serialized must be marked with the <see cref="DataMemberAttribute" /> attribute.
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

            var type = value.GetType().GetTypeInfo();

            // throw exception if type is not data contract compatible
            if (!GraphIterator.IsMarkedDataContract(type))
            {
                throw new ArgumentException(
                    $"The type of {nameof(value)} must be marked with the {nameof(DataContractAttribute)} attribute.",
                    nameof(value));
            }

            // enumerate fields
            foreach (var field in type.DeclaredFields.Where(f => f.GetCustomAttribute<DataMemberAttribute>() != null))
            {
                data.AddValue(field.Name, field.GetValue(value));
            }

            // enumerate properties
            foreach (var property in type.DeclaredProperties.Where(p => p.GetCustomAttribute<DataMemberAttribute>() != null))
            {
                // throw exception if n
                if (property.GetMethod == null && property.SetMethod == null)
                {
                    throw new SerializationException(
                        $"The property {property.Name} of type {type.AssemblyQualifiedName} must have a getter as well as a setter method.");
                }

                data.AddValue(property.Name, property.GetValue(value));
            }
        }

        /// <summary>
        /// Retrieves data from an <see cref="ObjectDataCollection" /> instance and uses it for populating an object.
        /// </summary>
        /// <param name="value">
        ///     The target <see cref="object" /> that shall be populated with data from <paramref name="value" />.
        /// </param>
        /// <param name="data">
        ///     The source <see cref="ObjectDataCollection" /> populated with data.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown when:</para>
        ///     <para>  - <paramref name="value" /> is null.</para>
        ///     <para>  - <paramref name="data" /> is null.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown when the type of <paramref name="value" /> is not marked with the <see cref="DataContractAttribute" /> attribute.</para>
        /// </exception>
        /// <exception cref="SerializationException">
        ///     <para>Thrown when a property does not have a getter and a setter method.</para>
        /// </exception>
        /// <remarks>
        ///     <para>Implements <see cref="ISurrogate{T}.SetData(T, ObjectDataCollection)" /> explicitly.</para>
        ///     <para>
        ///         The type of <paramref name="value" /> must be marked with the <see cref="DataContractAttribute" /> attribute.
        ///         and properties and fields that shall be (de-)serialized must be marked with the <see cref="DataMemberAttribute" /> attribute.
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

            var type = value.GetType().GetTypeInfo();

            // throw exception if type is not data contract compatible
            if (!GraphIterator.IsMarkedDataContract(type))
            {
                throw new ArgumentException(
                    $"The type of {nameof(value)} must be marked with the {nameof(DataContractAttribute)} attribute.",
                    nameof(value));
            }

            // enumerate fields
            foreach (var field in type.DeclaredFields.Where(f => f.GetCustomAttribute<DataMemberAttribute>() != null))
            {
                field.SetValue(value, data.GetValue(field.Name));
            }

            // enumerate properties
            foreach (var property in type.DeclaredProperties.Where(p => p.GetCustomAttribute<DataMemberAttribute>() != null))
            {
                // throw exception if n
                if (property.GetMethod == null && property.SetMethod == null)
                {
                    throw new SerializationException(
                        $"The property {property.Name} of type {type.AssemblyQualifiedName} must have a getter as well as a setter method.");
                }

                property.SetValue(value, data.GetValue(property.Name));
            }
        }
    }
}
