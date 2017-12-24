// <copyright file="Enumerable.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

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
        /// <param name="self">
        /// The source <see cref="IEnumerable{T}" />.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="action">
        /// The action that shall be executed for each element in <paramref name="self" />.
        /// Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if:</para>
        /// <item><paramref name="self" /> is <c>null</c>.</item>
        /// <item><paramref name="action" /> is <c>null</c>.</item>
        /// </exception>
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var e in self ?? throw new ArgumentNullException(nameof(self)))
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
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var count = 0L;
            foreach (var e in self ?? throw new ArgumentNullException(nameof(self)))
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
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            var enumerable = self as T[] ?? self.ToArray();
            return enumerable.Any()
                   ? (enumerable.First(), enumerable.Skip(1))
                   : throw new InvalidOperationException();
        }

        public static bool SetEquals<T>(this IEnumerable<T> self, IEnumerable<T> other)
            => self is null ? throw new ArgumentNullException(nameof(self))
             : other is null ? throw new ArgumentNullException(nameof(other))
             : self.All(other.Contains);

        public static bool OrderedEquals<T>(this IEnumerable<T> self, IEnumerable<T> other)
            => self is null ? throw new ArgumentNullException(nameof(self))
             : other is null ? throw new ArgumentNullException(nameof(other))
             : self.Zip(other, (a, b) => (a, b)).All(t => t.a.Equals(t.b));
    }
}
