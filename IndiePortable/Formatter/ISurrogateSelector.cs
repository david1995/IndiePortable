// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ISurrogateSelector.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ISurrogateSelector interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;

    /// <summary>
    /// Provides managing and selecting mechanisms for surrogates.
    /// </summary>
    public interface ISurrogateSelector
    {
        /// <summary>
        /// Gets the surrogate for the specified type.
        /// </summary>
        /// <param name="type">
        ///     The type for which the surrogate shall be returned.
        /// </param>
        /// <returns>
        ///     Returns the <see cref="ISurrogate" /> for the specified type.
        /// </returns>
        ISurrogate GetSurrogate(Type type);

        /// <summary>
        /// Determines whether the <see cref="ISurrogateSelector" /> contains a surrogate for the specified <see cref="Type" />.
        /// </summary>
        /// <param name="type">
        ///     The type that shall be tested.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the <see cref="ISurrogateSelector" /> contains a surrogate for the specified <see cref="Type" />.
        /// </returns>
        bool ContainsSurrogate(Type type);
    }
}
