// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableQueue.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ObservableQueue&lt;T&gt; class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Formatter;


    [Serializable]
    public class ObservableQueue<T>
        : ICollection, IEnumerable<T>, IEnumerable, ISerializable
    {

        private readonly Queue<T> source;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}" /> class.
        /// </summary>
        /// <param name="source">
        ///     The <see cref="Queue{T}" /> that shall be wrapped.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="source" /> is <c>null</c>.</para>
        /// </exception>
        public ObservableQueue(Queue<T> source)
        {
            if (object.ReferenceEquals(source, null))
            {
                throw new ArgumentNullException(nameof(source));
            }

            this.source = source;
        }

        /// <summary>
        /// Raised when an item has been enqueued to the <see cref="ObservableQueue{T}" />.
        /// </summary>
        public event EventHandler ItemEnqueued;

        /// <summary>
        /// Raised when an item has been dequeued from the <see cref="ObservableQueue{T}" />.
        /// </summary>
        public event EventHandler ItemDequeued;

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ObservableQueue{T}" />.
        /// </summary>
        /// <value>
        ///     Contains the number of elements contained in the <see cref="ObservableQueue{T}" />.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection.Count" /> implicitly.</para>
        /// </remarks>
        public int Count => this.source.Count;

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ObservableQueue{T}" /> is synchronized (thread safe).
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether access to the <see cref="ObservableQueue{T}" /> is synchronized (thread safe).
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection.IsSynchronized" /> implicitly.</para>
        /// </remarks>
        public bool IsSynchronized => (this.source as ICollection)?.IsSynchronized ?? false;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="ObservableQueue{T}" />.
        /// </summary>
        /// <value>
        ///     Contains an object that can be used to synchronize access to the <see cref="ObservableQueue{T}" />.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection.SyncRoot" /> implicitly.</para>
        /// </remarks>
        public object SyncRoot => (this.source as ICollection)?.SyncRoot;

        /// <summary>
        /// Inserts the item at the end of the <see cref="ObservableQueue{T}" />.
        /// </summary>
        /// <param name="item">
        ///     The item that shall be enqueued.
        /// </param>
        public void Enqueue(T item)
        {
            this.source.Enqueue(item);
            this.RaiseItemEnqueued();
        }

        /// <summary>
        /// Removes the first item of the <see cref="ObservableQueue{T}" /> and returns it.
        /// </summary>
        /// <returns>
        ///     Returns the dequeued item.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the <see cref="ObservableQueue{T}" /> is empty.</para>
        /// </exception>
        public T Dequeue()
        {
            var ret = this.source.Dequeue();
            this.RaiseItemDequeued();
            return ret;
        }

        /// <summary>
        /// Returns the first item of the <see cref="ObservableQueue{T}" /> without removing it.
        /// </summary>
        /// <returns>
        ///     Returns the peeked item.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the <see cref="ObservableQueue{T}" /> is empty.</para>
        /// </exception>
        public T Peek() => this.source.Peek();

        /// <summary>
        /// Determines whether the <see cref="ObservableQueue{T}" /> contains a specified item.
        /// </summary>
        /// <param name="item">
        ///     The item that shall be looked for.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <see cref="ObservableQueue{T}" /> contains <paramref name="item" />; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T item) => this.source.Contains(item);

        /// <summary>
        /// Clears the <see cref="ObservableQueue{T}" />.
        /// </summary>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection{T}.Clear()" /> implicitly.</para>
        /// </remarks>
        public void Clear()
        {
            this.source.Clear();
            this.RaiseItemDequeued();
        }

        /// <summary>
        /// Copies the elements of the <see cref="ObservableQueue{T}" /> to an <see cref="Array" />, starting at a particular <see cref="Array" /> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="Array" /> that is the destination of the elements copied from <see cref="ObservableQueue{T}" />.
        ///     The <see cref="Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="index">
        ///     The zero-based index in <paramref name="array" /> at which copying begins.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="array" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if <paramref name="index" /> is less than <c>0</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if at least one of the following conditions applies:</para>
        ///     <list type="bullet">
        ///         <item><paramref name="array" /> is multidimensional.</item>
        ///         <item>
        ///             The number of elements in the <see cref="ObservableQueue{T}" /> is greater than the available space
        ///             from <paramref name="index" /> to the end of <paramref name="array" />.
        ///         </item>
        ///         <item>
        ///             The type of the <see cref="ObservableQueue{T}" /> cannot be cast
        ///             automatically to the type of <paramref name="array" />.
        ///         </item>
        ///     </list>
        /// </exception>
        public void CopyTo(Array array, int index) => (this.source as ICollection)?.CopyTo(array, index);

        /// <summary>
        /// Populates a specified <see cref="ObjectDataCollection" /> instance
        /// with data from the <see cref="ObservableQueue{T}" /> instance.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> that shall be populated.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        public virtual void GetObjectData(ObjectDataCollection data)
        {
            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            var index = 0;
            foreach (var item in this.source)
            {
                data.AddValue(index.ToString(), item);
                index++;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="IEnumerator{T}" /> that can be used to iterate through the collection.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="IEnumerable{T}.GetEnumerator()" /> implicitly.</para>
        /// </remarks>
        public IEnumerator<T> GetEnumerator() => this.source.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="IEnumerable.GetEnumerator()" /> implicitly.</para>
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Raises the <see cref="ItemEnqueued" /> event.
        /// </summary>
        protected void RaiseItemEnqueued() => this.ItemEnqueued?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Raises the <see cref="ItemDequeued" /> event.
        /// </summary>
        protected void RaiseItemDequeued() => this.ItemDequeued?.Invoke(this, EventArgs.Empty);
    }
}
