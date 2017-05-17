// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyObservableDynamicArray.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ReadOnlyObservableDynamicArray class.
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

    /// <summary>
    /// Provides a read-only wrapper around a <see cref="ObservableDynamicArray{T}" />.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the elements of the <see cref="ReadOnlyObservableDynamicArray{T}" />.
    /// </typeparam>
    /// <seealso cref="IObservableReadOnlyList{T}" />
    /// <seealso cref="INotifyCollectionChanged" />
    /// <seealso cref="INotifyPropertyChanged" />
    /// <seealso cref="IDisposable" />
    public class ReadOnlyObservableDynamicArray<T>
        : IObservableReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// The source <see cref="IObservableList{T}" />.
        /// </summary>
        private IObservableList<T> source;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyObservableDynamicArray{T}" /> class.
        /// </summary>
        /// <param name="source">
        ///     The source <see cref="IObservableList{T}" /> that shall be wrapped.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="source" /> is <c>null</c>.</para>
        /// </exception>
        public ReadOnlyObservableDynamicArray(IObservableList<T> source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.source.CollectionChanged += this.Source_CollectionChanged;
            this.source.PropertyChanged += this.Source_PropertyChanged;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ReadOnlyObservableDynamicArray{T}" /> class.
        /// </summary>
        ~ReadOnlyObservableDynamicArray()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Raised when the collection has been changed.
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
        /// Gets the number of elements in the <see cref="ReadOnlyObservableDynamicArray{T}" />.
        /// </summary>
        /// <value>
        ///     Contains the number of elements in the <see cref="ReadOnlyObservableDynamicArray{T}" />.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="IReadOnlyCollection{T}.Count" /> implicitly.</para>
        /// </remarks>
        public int Count =>  (this.source as IList<T>).Count;


        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => true;


        bool ICollection.IsSynchronized => false;


        object ICollection.SyncRoot => null;


        object IList.this[int index]
        {
            get => this[index];
            set => throw new InvalidOperationException();
        }

        /// <summary>
        /// Gets the <typeparamref name="T"/> at the specified index.
        /// </summary>
        /// <value>
        ///     Contains he <typeparamref name="T" /> at the specified index.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>
        ///     Returns he <typeparamref name="T" /> at the specified index in the <see cref="ReadOnlyObservableDynamicArray{T}" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if <paramref name="index" /> is smaller than <c>0</c> or greater or equals <see cref="Count" />.</para>
        /// </exception>
        /// <remarks>
        ///     <para>Implements <see cref="IReadOnlyList{T}.this[int]" /> implicitly.</para>
        /// </remarks>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return (this.source as IList<T>)[index];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ReadOnlyObservableDynamicArray{T}" />.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerator{T}" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator() => this.source.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="IEnumerable" />.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerator" /> that can be used to iterate through the <see cref="IEnumerable" />.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Releases all resources reserved by the <see cref="ReadOnlyObservableDynamicArray{T}" />.
        /// </summary>
        /// <remarks>
        ///     <para>Implements <see cref="IDisposable.Dispose()" /> implicitly.</para>
        /// </remarks>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="managed">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected void Dispose(bool managed)
        {
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <param name="property">
        ///     The name of the changed property.
        /// </param>
        protected void RaisePropertyChanged([CallerMemberName]string property = null)
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        /// <summary>
        /// Raises the <see cref="CollectionChanged" /> event.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="NotifyCollectionChangedEventArgs" /> for the event handlers.
        /// </param>
        protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
            => this.CollectionChanged?.Invoke(this, args);

        /// <summary>
        /// Handles the <see cref="INotifyCollectionChanged.CollectionChanged" /> event of <see cref="source" />.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the event.
        /// </param>
        /// <param name="e">
        ///     Contains additional information for the event handler.
        /// </param>
        private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaiseCollectionChanged(e);
        }

        /// <summary>
        /// Handles the <see cref="INotifyPropertyChanged.PropertyChanged" /> event of <see cref="source" />.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the event.
        /// </param>
        /// <param name="e">
        ///     The <see cref="PropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void Source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ObservableDynamicArray<T>.Count):
                case nameof(ObservableDynamicArray<T>.Capacity):
                case nameof(ObservableDynamicArray<T>.UsagePercent):
                    this.RaisePropertyChanged(e.PropertyName);
                    break;
            }
        }


        int IList.Add(object value) => throw new InvalidOperationException();

        void IList.Clear() => throw new InvalidOperationException();

        bool IList.Contains(object value)
            => value is T
             ? this.Contains((T)value)
             : false;

        int IList.IndexOf(object value)
            => value is T
             ? this.source.IndexOf((T)value)
             : -1;

        void IList.Insert(int index, object value)
        {
            throw new InvalidOperationException();
        }

        void IList.Remove(object value)
        {
            throw new InvalidOperationException();
        }

        void IList.RemoveAt(int index)
        {
            throw new InvalidOperationException();
        }

        void ICollection.CopyTo(Array array, int index)
            => (this.source as ICollection).CopyTo(array, index);
    }
}
