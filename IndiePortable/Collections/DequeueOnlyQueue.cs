// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="DequeueOnlyQueue.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the DequeueOnlyQueue&lt;T&gt; class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a queue where items can only be dequeued.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the items in the <see cref="DequeqeOnlyQueue{T}" />.
    /// </typeparam>
    /// <seealso cref="IEnumerable{T}" />
    /// <seealso cref="ICollection" />
    /// <seealso cref="IEnumerable" />
    public class DequeqeOnlyQueue<T>
        : IEnumerable<T>, ICollection, IEnumerable
    {
        /// <summary>
        /// The source <see cref="Queue{T}" />.
        /// </summary>
        private readonly Queue<T> source;

        /// <summary>
        /// Initializes a new instance of the <see cref="DequeqeOnlyQueue{T}" /> class.
        /// </summary>
        /// <param name="source">
        ///     The source <see cref="Queue{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="source" /> is <c>null</c>.</para>
        /// </exception>
        public DequeqeOnlyQueue(Queue<T> source)
        {
            if (object.ReferenceEquals(source, null))
            {
                throw new ArgumentNullException(nameof(source));
            }

            this.source = source;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="DequeqeOnlyQueue{T}" />.
        /// </summary>
        /// <value>
        ///     Contains the number of elements contained in the <see cref="DequeqeOnlyQueue{T}" />.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection.Count" /> implicitly.</para>
        /// </remarks>
        public int Count => this.source.Count;

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="DequeqeOnlyQueue{T}" /> is synchronized (thread safe).
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether access to the <see cref="DequeqeOnlyQueue{T}" /> is synchronized (thread safe).
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection.IsSynchronized" /> implicitly.</para>
        /// </remarks>
        public bool IsSynchronized => ((ICollection)source).IsSynchronized;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="DequeqeOnlyQueue{T}" />.
        /// </summary>
        /// <value>
        ///     Contains an object that can be used to synchronize access to the <see cref="DequeqeOnlyQueue{T}" />.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection.SyncRoot" /> implicitly.</para>
        /// </remarks>
        public object SyncRoot => ((ICollection)source).SyncRoot;

        /// <summary>
        /// Returns and removes the object at the beginning of the <see cref="DequeqeOnlyQueue{T}" />.
        /// </summary>
        /// <returns>
        ///     The object that is removed from the beginning of the <see cref="DequeqeOnlyQueue{T}" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the <see cref="DequeqeOnlyQueue{T}" /> is empty.</para>
        /// </exception>
        public T Dequeue() => this.source.Dequeue();

        /// <summary>
        /// Returns the object at the beginning of the<see cref="DequeqeOnlyQueue{T}" /> without removing it.
        /// </summary>
        /// <returns>
        ///     The object at the beginning of the <see cref="DequeqeOnlyQueue{T}" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the <see cref="DequeqeOnlyQueue{T}" /> is empty.</para>
        /// </exception>
        public T Peek() => this.source.Peek();

        /// <summary>
        /// Copies the elements of the <see cref="DequeqeOnlyQueue{T}" /> to an <see cref="Array" />,
        /// starting at the specified <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        ///     from <see cref="DequeqeOnlyQueue{T}" />. The <see cref="Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        ///     The zero-based index in array at which copying begins.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="array" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if <paramref name="arrayIndex" /> is less than <c>0</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>
        ///         Thrown if the number of elements in the source <see cref="DequeqeOnlyQueue{T}" />
        ///         is greater than the available space from index to the end of the destination array.
        ///     </para>
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex) => this.source.CopyTo(array, arrayIndex);

        /// <summary>
        /// Copies the elements of the <see cref="ICollection" /> to an <see cref="Array" />,
        /// starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        ///     from <see cref="ICollection" />. The <see cref="Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="index">
        ///     The zero-based index in array at which copying begins.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="array" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if <paramref name="index" /> is less than <c>0</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if:</para>
        ///     <para>
        ///         - <paramref name="array" /> is multidimensional.
        ///     </para>
        ///     <para>
        ///         - the number of elements in the source <see cref="ICollection" />
        ///         is greater than the available space from index to the end of the destination array.
        ///     </para>
        ///     <para>
        ///         - The type of the source <see cref="ICollection" /> cannot
        ///         be cast automatically to the type of the destination array.
        ///     </para>
        /// </exception>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection.CopyTo(Array, int)" /> explicitly.</para>
        /// </remarks>
        void ICollection.CopyTo(Array array, int index) => ((ICollection)this.source).CopyTo(array, index);

        /// <summary>
        /// Gets an enumerator that iterates through the <see cref="DequeqeOnlyQueue{T}" />.
        /// </summary>
        /// <returns>
        ///     Returns an enumerator that iterates through the <see cref="DequeqeOnlyQueue{T}" />.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="IEnumerable{T}.GetEnumerator()" /> implicitly.</para>
        /// </remarks>
        public IEnumerator<T> GetEnumerator() => this.source.GetEnumerator();

        /// <summary>
        /// Gets an enumerator that iterates through the <see cref="DequeqeOnlyQueue{T}" />.
        /// </summary>
        /// <returns>
        ///     Returns an enumerator that iterates through the <see cref="DequeqeOnlyQueue{T}" />.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="IEnumerable.GetEnumerator()" /> explicitly.</para>
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
