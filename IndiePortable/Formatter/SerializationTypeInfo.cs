// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationTypeInfo.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the SerializationTypeInfo class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides serialization information about a type.
    /// </summary>
    public class SerializationTypeInfo
    {
        /// <summary>
        /// The type information of the represented <see cref="Type" />.
        /// </summary>
        private TypeInfo typeInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationTypeInfo" /> class.
        /// </summary>
        /// <param name="type">
        ///     The type for which the serialization information shall be collected.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if the type implements <see cref="ISerializable" />
        ///     but does not contain a constructor with an <see cref="ObjectDataCollection" /> parameter.
        /// </exception>
        public SerializationTypeInfo(Type type)
        {
            this.ViewedType = type ?? throw new ArgumentNullException(nameof(type));
            this.typeInfo = type.GetTypeInfo();

            // determine whether type is marked with legacy SerializableAttribute attribute
            this.DetermineIsMarkedLegacySerializable();

            // determine whether type implements ISerializable interface
            this.DetermineImplementsISerializable();

            // determine whether type is marked with SerializableAttribute attribute
            this.DetermineIsMarkedSerializable();

            // set is marked with DataContractAttribute attribute
            this.IsMarkedDataContract = this.typeInfo.CustomAttributes.Any(a => a.AttributeType == typeof(DataContractAttribute));

            // set version
            this.Version = (from a in this.typeInfo.GetCustomAttributes()
                            where a.GetType() == typeof(SerializedVersionAttribute)
                            select a as SerializedVersionAttribute).FirstOrDefault()?.Version;

            if (!this.ImplementsISerializable && !this.IsMarkedSerializable && !this.IsMarkedDataContract)
            {
                // else search for paramterless constructor
                this.DeserializationConstructor = (from c in this.typeInfo.DeclaredConstructors
                                                   where c.GetParameters().Length == 0
                                                   select c).FirstOrDefault();
            }

            // if no constructor could be found -> not (de-)serializable type
            this.IsSerializable = this.DeserializationConstructor != null;

            this.IsICollection = this.typeInfo.ImplementedInterfaces.Any(
                i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        /// <summary>
        /// Gets the <see cref="Type" /> of which the <see cref="SerializationTypeInfo" /> is built from.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="Type" /> of which the <see cref="SerializationTypeInfo" /> is built from.
        /// </value>
        public Type ViewedType { get; private set; }

        /// <summary>
        /// Gets the version of the <see cref="ViewedType" />.
        /// </summary>
        /// <value>
        ///     Contains the version of the <see cref="ViewedType" />.
        /// </value>
        public Version Version { get; private set; }

        /// <summary>
        /// Gets the de-serialization constructor for the <see cref="ViewedType" />.
        /// </summary>
        /// <value>
        ///     Contains the deserialization constructor for the <see cref="ViewedType" />.
        /// </value>
        public ConstructorInfo DeserializationConstructor { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ViewedType" /> is serializable.
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether the <see cref="ViewedType" /> is serializable.
        /// </value>
        public bool IsSerializable { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ViewedType" /> is marked with the <see cref="SerializableAttribute" /> attribute.
        /// </summary>
        /// <value>
        ///     <c>true</c> if <see cref="ViewedType" /> is marked with the <see cref="SerializableAttribute" /> attribute; otherwise <c>false</c>.
        /// </value>
        public bool IsMarkedSerializable { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ViewedType" /> implements <see cref="ISerializable" />.
        /// </summary>
        /// <value>
        ///   <c>true</c> if <see cref="ViewedType" /> implements <see cref="ISerializable" />; otherwise <c>false</c>.
        /// </value>
        public bool ImplementsISerializable { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ViewedType" /> is marked with the <see cref="DataContractAttribute" /> attribute.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the <see cref="ViewedType" /> is marked with the <see cref="DataContractAttribute" /> attribute; otherwise <c>false</c>.
        /// </value>
        public bool IsMarkedDataContract { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ViewedType" /> is an implementation of the <see cref="ICollection{T}" /> interface.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the <see cref="ViewedType" /> is an implementation of the <see cref="ICollection{T}" /> interface; otherwise, <c>false</c>.
        /// </value>
        public bool IsICollection { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ViewedType" /> is marked with the legacy serializable attribute.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="ViewedType" /> is marked with the legacy serializable attribute; otherwise, <c>false</c>.
        /// </value>
        public bool IsMarkedLegacySerializable { get; private set; }

        /// <summary>
        /// Determines whether the <see cref="ViewedType" /> is marked with the legacy serializable attribute.
        /// </summary>
        private void DetermineIsMarkedLegacySerializable()
        {
            this.IsMarkedLegacySerializable = this.typeInfo.CustomAttributes.Any(
                c => c.AttributeType != typeof(SerializableAttribute) &&
                     c.AttributeType.FullName == "System.SerializableAttribute");

            this.DeserializationConstructor = this.typeInfo.DeclaredConstructors.FirstOrDefault(c => c.GetParameters().Length == 0);
        }

        /// <summary>
        /// Determines whether the type implements the <see cref="ISerializable" /> interface.
        /// </summary>
        private void DetermineImplementsISerializable()
        {
            // set implements ISerializable
            this.ImplementsISerializable = this.typeInfo.ImplementedInterfaces.Any(i => i == typeof(ISerializable));
            if (this.ImplementsISerializable)
            {
                // search for constructor with ObjectData argument
                this.DeserializationConstructor = (from c in this.typeInfo.DeclaredConstructors
                                                   let p = c.GetParameters()
                                                   where p.Length == 2
                                                   where p[0].ParameterType == typeof(SerializationInfo)
                                                   where p[1].ParameterType == typeof(StreamingContext)
                                                   select c).Single();
            }
        }

        /// <summary>
        /// Determines whether the type is marked with the <see cref="DataContractAttribute" /> attribute.
        /// </summary>
        private void DetermineIsMarkedSerializable()
        {
            // set is marked with SerializableAttribute
            this.IsMarkedSerializable = this.typeInfo.CustomAttributes.Any(a => a.AttributeType == typeof(SerializableAttribute));
            if (this.IsMarkedSerializable)
            {
                // only search for a constructor with DeserializationConstructorAttribute when type is marked serializable
                // set deserialization constructor -> must not be null
                this.DeserializationConstructor = this.typeInfo.DeclaredConstructors
                                                               .Where(c => !c.GetParameters().Any())
                                                               .FirstOrDefault();

                // throw exception if no constructor with attribute and no parameterless constructor could be found
                if (this.DeserializationConstructor == null)
                {
                    throw new InvalidOperationException(
                        $"{this.ViewedType} does not contain a parameterless constructor.");
                }
            }
        }
    }
}
