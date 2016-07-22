// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializableAttribute.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the SerializableAttribute attribute.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Provides an attribute for marking types as serializable.
    /// </summary>
    /// <remarks>
    ///     Derives from <see cref="Attribute" />.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false)]
    [ComVisible(true)]
    public sealed class SerializableAttribute
        : Attribute
    {
    }
}
