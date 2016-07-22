// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewSelector.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IViewSelector interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Collections
{
    /// <summary>
    /// Provides a selector interface for types that provide mechanisms for creating view types.
    /// </summary>
    public interface IViewSelector
    {
        /// <summary>
        /// Gets the view for the specified object.
        /// </summary>
        /// <param name="value">
        ///     The value that shall be viewed.
        /// </param>
        /// <returns>
        ///     Returns the <see cref="IViewType{T}" /> that provides a view on <paramref name="value" />.
        /// </returns>
        object GetView(object value);
    }
}
