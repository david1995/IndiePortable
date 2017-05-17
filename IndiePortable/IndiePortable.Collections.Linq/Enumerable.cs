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
    using System.Linq;

    /// <summary>
    /// Provides extension methods to <see cref="IEnumerable{T}" /> implementations.
    /// </summary>
    public static class Enumerable
    {
        /// <summary>
        /// Executes an action for each element in a collection.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the element.
        /// </typeparam>
        /// <param name="source">
        /// The source <see cref="IEnumerable{T}" />.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="action">
        /// The action that shall be executed for each element in <paramref name="source" />.
        /// Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if:</para>
        /// <item><paramref name="source" /> is <c>null</c>.</item>
        /// <item><paramref name="action" /> is <c>null</c>.</item>
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
        /// The type of the element.
        /// </typeparam>
        /// <param name="self">
        /// The source <see cref="IEnumerable{T}" />.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="action">
        /// The action that shall be executed for each element in <paramref name="self" />.
        /// Gets the element itself and its index passed.
        /// Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <list type="bullet">
        /// <item><paramref name="self" /> is <c>null</c>.</item>
        /// <item><paramref name="action" /> is <c>null</c>.</item>
        /// </list>
        /// </exception>
        public static void ForEach<T>(this IEnumerable<T> self, Action<T, long> action)
        {
            if (object.ReferenceEquals(self, null))
            {
                throw new ArgumentNullException(nameof(self));
            }

            if (object.ReferenceEquals(action, null))
            {
                throw new ArgumentNullException(nameof(action));
            }

            var count = 0L;
            foreach (var e in self)
            {
                action(e, count++);
            }
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> self, T appendix)
        {
            foreach (var e in self)
            {
                yield return e;
            }

            yield return appendix;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> self, T prependix)
        {
            yield return prependix;
            foreach (var e in self)
            {
                yield return e;
            }
        }

        public static (T Head, IEnumerable<T> Tail) HeadTail<T>(this IEnumerable<T> self)
            => self.Any()
             ? (self.First(), self.Skip(1))
             : throw new InvalidOperationException();
    }
}
