// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializedAttribute.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the SerializedAttribute attribute.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;

    /// <summary>
    /// Provides an attribute for specifying fields that shall be serialized in a type marked with <see cref="SerializableAttribute" /> attribute.
    /// </summary>
    /// <remarks>
    ///     Derives from <see cref="Attribute" />.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class SerializedAttribute
        : Attribute
    {
    }
}
