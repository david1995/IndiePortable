// --------------------------------------------------------------
// <copyright file="ObservableDynamicArray.cs" company="David Eiwen">
// (c) 2015 by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="ObservableDynamicArray{T}" /> class.
// </summary>
// <remarks>
// We love Your name, Jesus!
// </remarks>
// --------------------------------------------------------------

namespace IndiePortable.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Formatter;

    /// <summary>
    /// Provides a dynamic array that raises an event when the array has been changed.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the elements.
    /// </typeparam>
    /// <remarks>
    ///     Derives from <see cref="DynamicArray{T}" />.<para />
    ///     Implements <see cref="INotifyPropertyChanged" />, <see cref="INotifyCollectionChanged" />, <see cref="ISerializable" />.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "Array suffix describes the functinality better than Collection suffix.")]
    [Serializable]
    public class ObservableDynamicArray<T>
        : DynamicArray<T>, IObservableList<T>, INotifyPropertyChanged, ISerializable
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDynamicArray{T}" /> class.
        /// </summary>
        public ObservableDynamicArray()
            : this(8, 2d)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDynamicArray{T}" /> class.
        /// </summary>
        /// <param name="growthRate">
        ///     The growth rate of the <see cref="ObservableDynamicArray{T}" />.
        /// </param>
        public ObservableDynamicArray(double growthRate)
            : this(8, growthRate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDynamicArray{T}" /> class.
        /// </summary>
        /// <param name="capacity">
        ///     The initial capacity of the <see cref="ObservableDynamicArray{T}" />.
        /// </param>
        public ObservableDynamicArray(int capacity)
            : this(capacity, 2d)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDynamicArray{T}" /> class.
        /// </summary>
        /// <param name="capacity">
        ///     The initial capacity of the <see cref="ObservableDynamicArray{T}" />.
        /// </param>
        /// <param name="growthRate">
        ///     The growth rate of the <see cref="ObservableDynamicArray{T}" />.
        /// </param>
        public ObservableDynamicArray(int capacity, double growthRate)
            : base(capacity, growthRate)
        {
            this.CapacityChanged += (s, e) => this.RaisePropertyChanged(nameof(this.Capacity));
            this.CountChanged += (s, e) => this.RaisePropertyChanged(nameof(this.Count));
            this.GrowthRateChanged += (s, e) => this.RaisePropertyChanged(nameof(this.GrowthRate));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDynamicArray{T}" /> class.
        /// </summary>
        /// <param name="items">
        ///     The initial items of the <see cref="ObservableDynamicArray{T}" />.
        /// </param>
        public ObservableDynamicArray(IEnumerable<T> items)
            : this(2d, items)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDynamicArray{T}" /> class.
        /// </summary>
        /// <param name="growthRate">
        ///     The growth rate of the <see cref="ObservableDynamicArray{T}" />.
        /// </param>
        /// <param name="items">
        ///     The initial items of the <see cref="ObservableDynamicArray{T}" />.
        /// </param>
        public ObservableDynamicArray(double growthRate, IEnumerable<T> items)
            : base(growthRate, items)
        {
            this.CapacityChanged += (s, e) => this.RaisePropertyChanged(nameof(this.Capacity));
            this.CountChanged += (s, e) => this.RaisePropertyChanged(nameof(this.Count));
            this.GrowthRateChanged += (s, e) => this.RaisePropertyChanged(nameof(this.GrowthRate));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDynamicArray{T}" /> class.
        /// </summary>
        /// <param name="data">
        ///     The serialized data for the <see cref="ObservableDynamicArray{T}" />.
        /// </param>
        public ObservableDynamicArray(ObjectDataCollection data)
            : base(data)
        {
            this.CapacityChanged += (s, e) => this.RaisePropertyChanged(nameof(this.Capacity));
            this.CountChanged += (s, e) => this.RaisePropertyChanged(nameof(this.Count));
            this.GrowthRateChanged += (s, e) => this.RaisePropertyChanged(nameof(this.GrowthRate));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ObservableDynamicArray{T}" /> class.
        /// </summary>
        ~ObservableDynamicArray()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Raised when a property of the <see cref="ObservableDynamicArray{T}" /> has been changed.
        /// </summary>
        /// <remarks>
        ///     Implements <see cref="INotifyPropertyChanged.PropertyChanged" /> implicitly.
        /// </remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raised when the <see cref="ObservableDynamicArray{T}" /> has been changed.
        /// </summary>
        /// <remarks>
        ///     Implements <see cref="INotifyCollectionChanged.CollectionChanged" /> implicitly.
        /// </remarks>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #region explicit IList & ICollection property & indexer implementation


        bool IList.IsFixedSize { get; } = false;


        bool ICollection.IsSynchronized { get; } = false;


        object ICollection.SyncRoot { get; } = null;


        object IList.this[int index]
        {
            get { return this[index]; }

            set
            {
                if (value is T)
                {
                    this[index] = (T)value;
                    return;
                }

                throw new ArgumentException($"The value must be of type {nameof(T)}.", nameof(value));
            }
        }

        #endregion

        /// <summary>
        /// Gets the object data of the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <param name="data">The data that shall be serialized.</param>
        /// <remarks>
        ///     <para>Implements <see cref="ISerializable.GetObjectData(ObjectDataCollection)" /> implicitly.</para>
        ///     <para>Overrides <see cref="DynamicArray{T}.GetObjectData(ObjectDataCollection)" />.</para>
        /// </remarks>
        public override void GetObjectData(ObjectDataCollection data)
        {
            base.GetObjectData(data);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <param name="property">
        ///     The name of the changed property.
        /// </param>
        protected void RaisePropertyChanged([CallerMemberName]string property = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        /// <summary>
        /// Raises the <see cref="CollectionChanged" /> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for the event handlers.
        /// </param>
        protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e) => this.CollectionChanged?.Invoke(this, e);

        /// <summary>
        /// Adds an item at the end of the <see cref="ObservableDynamicArray{T}" />.
        /// </summary>
        /// <param name="item">
        ///     The item that shall be added.
        /// </param>
        /// <remarks>
        ///     Overrides <see cref="DynamicArray{T}.AddItem(T)" />.
        /// </remarks>
        protected override void AddItem(T item)
        {
            var count = this.Count;
            base.InsertItem(count, item);
            this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, count));
        }

        /// <summary>
        /// Inserts an item at a specified position in the <see cref="ObservableDynamicArray{T}" />.
        /// </summary>
        /// <param name="index">
        ///     The index where the item shall be placed.
        /// </param>
        /// <param name="item">
        ///     The item that shall be inserted.
        /// </param>
        /// <remarks>
        ///     Overrides <see cref="DynamicArray{T}.InsertItem(int, T)" />.
        /// </remarks>
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">
        ///     The index of the item that shall be removed.
        /// </param>
        /// <remarks>
        ///     Overrides <see cref="DynamicArray{T}.RemoveItem(int)" />.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="index" /> is smaller that 0 or greater or equals <see cref="DynamicArray{TKey}.Count" />.
        /// </exception>
        protected override void RemoveItem(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var item = this[index];
            base.RemoveItem(index);
            this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        /// <summary>
        /// Replaces the item at the specified index.
        /// </summary>
        /// <param name="index">
        ///     The index of the item that shall be replaced.
        /// </param>
        /// <param name="item">
        ///     The item that shall replace the current one.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="index" /> is smaller than 0 or greater or equals <see cref="DynamicArray{T}.Count" />.
        /// </exception>
        /// <remarks>
        ///     Overrides <see cref="DynamicArray{T}.ReplaceItem(int, T)" />.
        /// </remarks>
        protected override void ReplaceItem(int index, T item)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var oldItem = this[index];

            base.ReplaceItem(index, item);
            this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem));
        }

        /// <summary>
        /// Resets the <see cref="ObservableDynamicArray{T}" /> to 0 items.
        /// </summary>
        /// <remarks>
        ///     Overrides <see cref="DynamicArray{T}.ResetItems()" />.
        /// </remarks>
        protected override void ResetItems()
        {
            base.ResetItems();
            this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #region ICollection & IList method implementations


        int IList.Add(object value)
        {
            if (value is T)
            {
                this.Add((T)value);
                return (this as IList).IndexOf(value);
            }
            
            throw new ArgumentException($"{nameof(value)} must be of type {nameof(T)}.", nameof(value));
        }


        bool IList.Contains(object value)
            => value is T
             ? this.Contains((T)value)
             : false;


        int IList.IndexOf(object value)
            => value is T
             ? this.IndexOf((T)value)
             : -1;


        void IList.Insert(int index, object value)
        {
            if (value is T)
            {
                this.Insert(index, (T)value);
                return;
            }

            throw new ArgumentException($"{nameof(value)} must be of type {nameof(T)}.", nameof(value));
        }


        void IList.Remove(object value)
        {
            if (value is T)
            {
                this.Remove((T)value);
                return;
            }

            throw new ArgumentException($"{nameof(value)} must be of type {nameof(T)}.", nameof(value));
        }


        void ICollection.CopyTo(Array array, int index)
        {
            if (array is T[])
            {
                this.CopyTo(array as T[], index);
                return;
            }

            throw new ArgumentException($"{nameof(array)} must be of type {nameof(T)}[].", nameof(array));
        }

        #endregion
    }
}
