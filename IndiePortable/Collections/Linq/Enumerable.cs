// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="Enumerable.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the static Enumerable class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Collections.Linq
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides extension methods to <see cref="IEnumerable{T}" /> implementations.
    /// </summary>
    public static class Enumerable
    {
        /// <summary>
        /// Executes an action for each element in a collection.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the element.
        /// </typeparam>
        /// <param name="source">
        ///     The source <see cref="IEnumerable{T}" />.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <param name="action">
        ///     The action that shall be executed for each element in <paramref name="source" />.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if:</para>
        ///     <para>  - <paramref name="source" /> is <c>null</c>.</para>
        ///     <para>  - <paramref name="action" /> is <c>null</c>.</para>
        /// </exception>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (object.ReferenceEquals(source, null))
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (object.ReferenceEquals(action, null))
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var e in source)
            {
                action(e);
            }
        }

        /// <summary>
        /// Executes an action for each element in a collection.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the element.
        /// </typeparam>
        /// <param name="source">
        ///     The source <see cref="IEnumerable{T}" />.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <param name="action">
        ///     The action that shall be executed for each element in <paramref name="source" />.
        ///     Gets the element itself and its index passed.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if:</para>
        ///     <para>  - <paramref name="source" /> is <c>null</c>.</para>
        ///     <para>  - <paramref name="action" /> is <c>null</c>.</para>
        /// </exception>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, long> action)
        {
            if (object.ReferenceEquals(source, null))
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (object.ReferenceEquals(action, null))
            {
                throw new ArgumentNullException(nameof(action));
            }

            var count = 0L;
            foreach (var e in source)
            {
                action(e, count++);
            }
        }
    }
}
