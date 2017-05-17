// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializedAttribute.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
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
