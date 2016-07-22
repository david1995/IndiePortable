// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="DeserializationConstructorAttribute.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the DeserializationConstructorAttribute attribute.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides an attribute for marking a constructor for using it in the deserialization process
    /// of a type that is marked with the <see cref="SerializableAttribute" /> attribute.
    /// </summary>
    /// <remarks>
    ///     Derives from <see cref="Attribute" />.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    public sealed class DeserializationConstructorAttribute
        : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeserializationConstructorAttribute" /> class.
        /// </summary>
        public DeserializationConstructorAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeserializationConstructorAttribute" /> class.
        /// </summary>
        /// <param name="parameterSourceNames">
        ///     The names of the value sources for the constructor's parameter.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="parameterSourceNames" /> is <c>null</c>.</para>
        /// </exception>
        public DeserializationConstructorAttribute(params string[] parameterSourceNames)
        {
            // throw exception if parameter source names is null
            if (parameterSourceNames == null)
            {
                throw new ArgumentNullException(nameof(parameterSourceNames));
            }

            this.ParameterSourceNames = parameterSourceNames;
        }

        /// <summary>
        /// Gets the names of the value sources for the constructor's parameter.
        /// </summary>
        /// <value>
        ///     Contains the names of the value sources for the constructor's parameter.
        /// </value>
        public IEnumerable<string> ParameterSourceNames { get; private set; }
    }
}
