// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="MscorlibSurrogateSelector.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the MscorlibSurrogateSelector class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.MscorlibSurrogates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides surrogates for the serializable types in the .NET library.
    /// </summary>
    /// <seealso cref="ISurrogateSelector" />
    public class MscorlibSurrogateSelector
        : ISurrogateSelector
    {
        /// <summary>
        /// The list containing all surrogates.
        /// </summary>
        private readonly List<ISurrogate> surrogates;

        /// <summary>
        /// Initializes a new instance of the <see cref="MscorlibSurrogateSelector" /> class.
        /// </summary>
        public MscorlibSurrogateSelector()
        {
            this.surrogates = new List<ISurrogate>
            {
                new GuidSurrogate(),
                new CultureInfoSurrogate(),
                new DateTimeSurrogate(),
                new TimeSpanSurrogate()
            };
        }

        /// <summary>
        /// Determines whether the <see cref="MscorlibSurrogateSelector" />
        /// contains a surrogate for the specified <see cref="Type" />.
        /// </summary>
        /// <param name="type">
        ///     The type that shall be tested.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the <see cref="MscorlibSurrogateSelector" />
        ///     contains a surrogate for <paramref name="type" />.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="ISurrogateSelector.ContainsSurrogate(Type)" /> implicitly.</para>
        /// </remarks>
        public bool ContainsSurrogate(Type type) => this.surrogates.Any(s => s.TargetType == type);

        /// <summary>
        /// Gets the <see cref="ISurrogate" /> associated with the specified <see cref="Type" />.
        /// </summary>
        /// <param name="type">
        ///     The <see cref="Type" /> associated with the desired <see cref="ISurrogate" />.
        /// </param>
        /// <returns>
        ///     Returns the <see cref="ISurrogate" /> associated with <paramref name="type" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <para>
        ///         Thrown if the <see cref="MscorlibSurrogateSelector" /> does not contain a surrogate for <paramref name="type" />.
        ///     </para>
        /// </exception>
        /// <remarks>
        ///     <para>Implements <see cref="ISurrogateSelector.GetSurrogate(Type)" /> implicitly.</para>
        /// </remarks>
        public ISurrogate GetSurrogate(Type type) => this.surrogates.First(s => s.TargetType == type);
    }
}
