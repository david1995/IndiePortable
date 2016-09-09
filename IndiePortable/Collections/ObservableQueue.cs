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


    public class ObservableQueue<T>
        : ICollection, IEnumerable<T>, IEnumerable
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


        public int Count => this.source.Count;


        public bool IsSynchronized => (this.source as ICollection)?.IsSynchronized ?? false;


        public object SyncRoot => (this.source as ICollection)?.SyncRoot;


        public void Enqueue(T item) => this.source.Enqueue(item);


        public T Dequeue() => this.source.Dequeue();


        public T Peek() => this.source.Peek();


        public bool Contains(T item) => this.source.Contains(item);


        public void Clear() => this.source.Clear();


        public void CopyTo(Array array, int index) => (this.source as ICollection)?.CopyTo(array, index);


        public IEnumerator<T> GetEnumerator() => this.source.GetEnumerator();


        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Raises the <see cref="ItemEnqueued" /> event.
        /// </summary>
        protected void RaiseItemEnqueued() => this.ItemEnqueued?.Invoke(this, EventArgs.Empty);
    }
}
