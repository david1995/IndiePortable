// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewType.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IViewType{T} interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace IndiePortable.Collections
{
    /// <summary>
    /// Defines a view type for a specified type.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the object that shall be viewed. Must be a class.
    /// </typeparam>
    public interface IViewType<T>
        where T : class
    {
        /// <summary>
        /// Gets the model associated with the view model type.
        /// </summary>
        /// <value>
        ///     Contains the model associated with the view model type.
        /// </value>
        T Model { get; }
    }
}
