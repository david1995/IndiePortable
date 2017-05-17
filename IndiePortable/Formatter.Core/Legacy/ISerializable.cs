// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializable.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ISerializable interface.
// </summary>
// <remarks>All glory and honor to the risen one!</remarks>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    /// <summary>
    /// Provides an interface for types that provide a custom serialization and deserialization logic.
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Populates a specified <see cref="ObjectDataCollection" /> instance with data from the <see cref="ISerializable" /> instance.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> that shall be populated.
        /// </param>
        void GetObjectData(ObjectDataCollection data);
    }
}
