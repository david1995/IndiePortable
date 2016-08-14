// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableListWrapper.cs" company="David Eiwen">
// Copyright © 2015 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ObservableListWrapper&lt;T&gt; class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a wrapper for an <see cref="IList{T}" /> instance making it observable.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the elements.
    /// </typeparam>
    /// <remarks>
    ///     <para>
    ///         This is a wrapper for an <seealso cref="IList{T}" /> instance. Because of that,
    ///         the <seealso cref="CollectionChanged" /> event is only raised when the methods of the
    ///         <see cref="ObservableListWrapper{T}" /> are used for modifying the collection.
    ///         If you use the modification methods of the source <seealso cref="IList{T}" />,
    ///         the <seealso cref="CollectionChanged" /> event will not be raised.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IObservableList{T}" />
    public sealed class ObservableListWrapper<T>
        : IObservableList<T>
    {
        /// <summary>
        /// The source <see cref="IList{T}" /> that is wrapped.
        /// </summary>
        private readonly IList<T> source;

        /// <summary>
        /// The backing field for the <see cref="SyncRoot" /> property.
        /// </summary>
        private readonly object syncRootBacking = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableListWrapper{T}" /> class.
        /// </summary>
        /// <param name="source">
        ///     The source <see cref="IList{T}" /> that shall be wrapped.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="source" /> is <c>null</c>.</para>
        /// </exception>
        public ObservableListWrapper(IList<T> source)
        {
            if (object.ReferenceEquals(source, null))
            {
                throw new ArgumentNullException(nameof(source));
            }

            this.source = source;
        }

        /// <summary>
        /// Raised when the <see cref="ObservableListWrapper{T}" /> has been changed.
        /// </summary>
        /// <remarks>
        ///     <para>Implements <see cref="INotifyCollectionChanged.CollectionChanged" /> implicitly.</para>
        /// </remarks>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Raised when the value of a property has been changed.
        /// </summary>
        /// <remarks>
        ///     <para>Implements <see cref="INotifyPropertyChanged.PropertyChanged" /> implicitly.</para>
        /// </remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ObservableListWrapper{T}" />.
        /// </summary>
        /// <value>
        ///     Contains the number of elements contained in the <see cref="ObservableListWrapper{T}" />.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection{T}.Count" /> implicitly.</para>
        /// </remarks>
        public int Count
        {
            get { return this.source.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ObservableListWrapper{T}" /> has a fixed size.
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether the <see cref="ObservableListWrapper{T}" /> has a fixed size.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="IList.IsFixedSize" /> implicitly.</para>
        /// </remarks>
        public bool IsFixedSize
        {
            get { return this.source.IsReadOnly; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ObservableListWrapper{T}" /> is read-only.
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether the <see cref="ObservableListWrapper{T}" /> is read-only.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection{T}.IsReadOnly" /> implicitly.</para>
        /// </remarks>
        public bool IsReadOnly
        {
            get { return this.source.IsReadOnly; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ObservableListWrapper{T}" /> is synchronized (thread safe).
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether access to the <see cref="ObservableListWrapper{T}" /> is synchronized (thread safe).
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection.IsSynchronized" /> implicitly.</para>
        /// </remarks>
        public bool IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="ObservableListWrapper{T}" />.
        /// </summary>
        /// <value>
        ///     Contains an object that can be used to synchronize access to the <see cref="ObservableListWrapper{T}" />.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection.SyncRoot" /> implicitly.</para>
        /// </remarks>
        public object SyncRoot
        {
            get { return this.syncRootBacking; }
        }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <value>
        ///     Contains the item at the specified index.
        /// </value>
        /// <param name="index">
        ///     The zero-based index of the item to get or set.
        /// </param>
        /// <returns>
        ///     Returns the element at the specified index.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="IList{T}.this[int]" /> implicitly.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if <paramref name="index" /> is smaller than <c>0</c> or greater or equals <see cref="Count" />.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     <para>Thrown if the source <see cref="IList{T}" /> is read-only.</para>
        /// </exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                
                return this.source[index];
            }

            set
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                if (this.IsReadOnly)
                {
                    throw new NotSupportedException();
                }

                this.source[index] = value;
                this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, index));
            }
        }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <value>
        ///     Contains the item at the specified index.
        /// </value>
        /// <param name="index">
        ///     The zero-based index of the item to get or set.
        /// </param>
        /// <returns>
        ///     Returns the element at the specified index.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="IList.this[int]" /> implicitly.</para>
        /// </remarks>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if <paramref name="value" /> is not convertible to <typeparamref name="T" />.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if <paramref name="index" /> is smaller than <c>0</c> or greater or equals <see cref="Count" />.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     <para>Thrown if the source <see cref="IList{T}" /> is read-only.</para>
        /// </exception>
        object IList.this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return this.source[index];
            }

            set
            {
                if (!(value is T))
                {
                    throw new ArgumentException($"The {nameof(value)} must be of the generic type {nameof(T)}.", nameof(value));
                }

                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                if (this.IsReadOnly)
                {
                    throw new NotSupportedException();
                }

                this.source[index] = (T)value;
                this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, index));
            }
        }

        /// <summary>
        /// Adds a value to the source <see cref="IList{T}" />.
        /// </summary>
        /// <param name="value">
        ///     The item to add.
        /// </param>
        /// <returns>
        ///     Returns the index of the added item.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if <paramref name="value" /> is not of type <typeparamref name="T" />.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     <para>Thrown if the source <see cref="IList{T}" /> has a fixed size and/or is read-only.</para>
        /// </exception>
        public int Add(object value)
        {
            var pos = this.Count;
            this.Insert(pos, value);
            return pos;
        }

        /// <summary>
        /// Adds an item to the <see cref="ObservableListWrapper{T}" />.
        /// </summary>
        /// <param name="item">
        ///     The item to insert into the <see cref="ObservableListWrapper{T}" />.
        /// </param>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection{T}.Add(T)" /> implicitly.</para>
        /// </remarks>
        /// <exception cref="NotSupportedException">
        ///     <para>Thrown if the base list has a fixed size or is read-only.</para>
        /// </exception>
        public void Add(T item) => this.Insert(this.Count, item);

        /// <summary>
        /// Removes all items from the source <see cref="IList{T}" />.
        /// </summary>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection{T}.Clear()" /> implicitly.</para>
        /// </remarks>
        public void Clear()
        {
            this.source.Clear();
            this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Determines whether the source <see cref="IList{T}" /> contains a specified item.
        /// </summary>
        /// <param name="value">
        ///     The item to locate in the source <see cref="IList{T}" />.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the source <see cref="IList{T}" /> contains <paramref name="value" />; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="IList.Contains(object)" /> implicitly.</para>
        /// </remarks>
        public bool Contains(object value) => value is T ? this.source.Contains((T)value) : false;

        /// <summary>
        /// Determines whether the source <see cref="IList{T}" /> contains a specified item.
        /// </summary>
        /// <param name="item">
        ///     The item to locate in the source <see cref="IList{T}" />.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the source <see cref="IList{T}" /> contains <paramref name="item" />; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection{T}.Contains(T)" /> implicitly.</para>
        /// </remarks>
        public bool Contains(T item) => this.source.Contains(item);

        /// <summary>
        /// Copies the items of the source <see cref="IList{T}" /> to an <see cref="Array" />
        /// starting at a particular array index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="Array" /> that is the destination of the items
        ///     copied from the source <see cref="IList{T}" />. It must have zero-based indexing.
        /// </param>
        /// <param name="index">
        ///     The zero-based index at which the copying begins.
        /// </param>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection.CopyTo(Array, int)" /> implicitly.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="array" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if <paramref name="index" /> is less than <c>0</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if:</para>
        ///     <para>
        ///         - the number of elements in the source <see cref="IList{T}" /> is greater than the available space
        ///           from <paramref name="index"/> to the end of <paramref name="array" />.
        ///     </para>
        ///     <para>
        ///         - <paramref name="array" /> is not a one-dimensional array of type <typeparamref name="T" />.
        ///     </para>
        /// </exception>
        public void CopyTo(Array array, int index)
        {
            if (object.ReferenceEquals(array, null))
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (!(array is T[]))
            {
                throw new ArgumentException($"The {nameof(array)} must be of type {nameof(T)}[].", nameof(array));
            }

            this.CopyTo((T[])array, index);
        }

        /// <summary>
        /// Copies the items of the source <see cref="IList{T}" /> to a one-dimensional <typeparamref name="T" /> array
        /// starting at a particular array index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <typeparamref name="T" /> array that is the destination of the items
        ///     copied from the source <see cref="IList{T}" />. It must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        ///     The zero-based index at which the copying begins.
        /// </param>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection{T}.CopyTo(T[], int)" /> implicitly.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="array" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if <paramref name="arrayIndex" /> is less than <c>0</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if:</para>
        ///     <para>
        ///         - the number of elements in the source <see cref="IList{T}" /> is greater than the available space
        ///           from <paramref name="arrayIndex"/> to the end of <paramref name="array" />.
        ///     </para>
        ///     <para>
        ///         - <paramref name="array" /> is not a one-dimensional array of type <typeparamref name="T" />.
        ///     </para>
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (object.ReferenceEquals(array, null))
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (arrayIndex + this.Count > array.Length)
            {
                throw new ArgumentException();
            }

            this.source.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets an enumerator that iterates through the <see cref="ObservableListWrapper{T}" />.
        /// </summary>
        /// <returns>
        ///     Returns an enumerator that iterates through the <see cref="ObservableListWrapper{T}" />.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="IEnumerable{T}.GetEnumerator()" /> implicitly.</para>
        /// </remarks>
        public IEnumerator<T> GetEnumerator() => this.source.GetEnumerator();
        
        /// <summary>
        /// Determines the index of a specified item in the source <see cref="IList{T}" />.
        /// </summary>
        /// <param name="value">
        ///     The item to locate in the source <see cref="IList{T}" />.
        /// </param>
        /// <returns>
        ///     Returns the index of <paramref name="value" /> if found
        ///     in the source <see cref="IList{T}" />; otherwise <c>-1</c>.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="IList.IndexOf(object)" /> implicitly.</para>
        /// </remarks>
        public int IndexOf(object value) => value is T ? this.IndexOf((T)value) : -1;

        /// <summary>
        /// Determines the index of a specified item in the source <see cref="IList{T}" />.
        /// </summary>
        /// <param name="item">
        ///     The item to locate in the source <see cref="IList{T}" />.
        /// </param>
        /// <returns>
        ///     Returns the index of <paramref name="item" /> if found
        ///     in the source <see cref="IList{T}" />; otherwise <c>-1</c>.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="IList{T}.IndexOf(T)" /> implicitly.</para>
        /// </remarks>
        public int IndexOf(T item) => this.source.IndexOf(item);

        /// <summary>
        /// Inserts an item to the source <see cref="IList{T}" /> at the specified index.
        /// </summary>
        /// <param name="index">
        ///     The zero-based index where <paramref name="value" /> should be inserted.
        /// </param>
        /// <param name="value">
        ///     The value to insert into the source list.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if <paramref name="value" /> is not of type <typeparamref name="T" />.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if <paramref name="index" /> is less than <c>0</c> or greater than <see cref="Count" />.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     <para>Thrown if the source list has a fixed size and/or is read-only.</para>
        /// </exception>
        public void Insert(int index, object value)
        {
            if (!(value is T))
            {
                throw new ArgumentException($"{nameof(value)} must be of the generic type parameter {nameof(T)}.", nameof(value));
            }

            var item = (T)value;
            this.Insert(index, item);
            this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        /// <summary>
        /// Inserts an item to the source <see cref="IList{T}" /> at the specified index.
        /// </summary>
        /// <param name="index">
        ///     The zero-based index where <paramref name="item" /> should be inserted.
        /// </param>
        /// <param name="item">
        ///     The item to insert into the source list.
        /// </param>
        /// <exception cref="NotSupportedException">
        ///     <para>Thrown if the source list has a fixed size and/or is read-only.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if <paramref name="index" /> is less than <c>0</c> or greater than <see cref="Count" />.</para>
        /// </exception>
        public void Insert(int index, T item)
        {
            if (this.IsFixedSize || this.IsReadOnly)
            {
                throw new NotSupportedException("The source list has a fixed size and/or is read-only.");
            }

            if (index < 0 || index > this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.source.Insert(index, item);
            this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        /// <summary>
        /// Removes the first occurrence of a specific item from the source <see cref="IList{T}" />.
        /// </summary>
        /// <param name="value">
        ///     The item to remove from the source <see cref="IList{T}" />.
        /// </param>
        /// <remarks>
        ///     <para>Implements <see cref="IList.Remove(object)" /> implicitly.</para>
        /// </remarks>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if <paramref name="value" /> is not of the generic type parameter <typeparamref name="T" />.</para>
        /// </exception>
        public void Remove(object value)
        {
            if (!(value is T))
            {
                throw new ArgumentException($"{nameof(value)} must be of the generic type parameter {nameof(T)}.", nameof(value));
            }

            this.Remove((T)value);
        }

        /// <summary>
        /// Removes the first occurrence of a specific item from the source <see cref="IList{T}" />.
        /// </summary>
        /// <param name="item">
        ///     The item to remove from the source <see cref="IList{T}" />.
        /// </param>
        /// <returns>
        ///     Returns <c>true</c> if <paramref name="item" /> was successfully removed
        ///     from the source <see cref="IList{T}" />; otherwise <c>false</c>.
        ///     This method also returns <c>false</c> if <paramref name="item" />
        ///     was not found in the original source <see cref="IList{T}" />.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="ICollection{T}.Remove(T)" /> implicitly.</para>
        /// </remarks>
        public bool Remove(T item)
        {
            var index = this.IndexOf(item);
            if (index >= 0)
            {
                if (this.source.Remove(item))
                { 
                    this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the item at the specified position in the source <see cref="IList{T}" />.
        /// </summary>
        /// <param name="index">
        ///     The zero-based index of the item to remove.
        /// </param>
        /// <remarks>
        ///     <para>Implements <see cref="IList{T}.RemoveAt(int)" /> implicitly.</para>
        /// </remarks>
        /// <exception cref="NotSupportedException">
        ///     <para>Thrown if the source <see cref="IList{T}" /> has a fixed size and/or is read-only.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if <paramref name="index" /> is less than <c>0</c> or greater or equals <see cref="Count" />.</para>
        /// </exception>
        public void RemoveAt(int index)
        {
            if (this.IsFixedSize || this.IsReadOnly)
            {
                throw new NotSupportedException();
            }

            if (index < 0 || index >= this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var item = this[index];
            this.source.RemoveAt(index);
            this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="IEnumerable.GetEnumerator()" /> explicitly.</para>
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Raises the <see cref="CollectionChanged" /> event.
        /// </summary>
        /// <param name="args">
        ///     Contains additional information for the event handler.
        /// </param>
        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
            => this.CollectionChanged?.Invoke(this, args);
        
        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <param name="property">
        ///     The name of the changed property.
        /// </param>
        private void RaisePropertyChanged([CallerMemberName]string property = null)
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}
