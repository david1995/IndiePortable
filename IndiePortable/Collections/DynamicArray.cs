// ------------------------------------------------------------
// <copyright file="DynamicArray.cs" company="David Eiwen">
// © 2015 David Eiwen
// </copyright>
// <summary>
// This file contains the <see cref="DynamicArray" /> class.
// </summary>
// ------------------------------------------------------------

namespace IndiePortable.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Formatter;

    /// <summary>
    /// Provides a dynamically sized, thread safe array.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the items.
    /// </typeparam>
    /// <remarks>
    ///     Implements <see cref="IEnumerable{T}" />, <see cref="IList{T}" />, <see cref="ISerializable" />, <see cref="IDisposable" /> explicitly.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "Array suffix is describing the functionaliry better than the Collection suffix.")]
    [Serializable]
    [ComVisible(true)]
    public class DynamicArray<T>
        : IEnumerable<T>, IList<T>, ISerializable, IDisposable
    {
        /// <summary>
        /// The <see cref="SemaphoreSlim" /> that handles the thread synchronization for the <see cref="DynamicArray{T}" />.
        /// </summary>
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// The backing array for the <see cref="DynamicArray{T}" />.
        /// </summary>
        private T[] backing;

        /// <summary>
        /// The backing field for the <see cref="GrowthRate" /> property.
        /// </summary>
        private double growthRateBacking;

        /// <summary>
        /// The backing field for the <see cref="Count" /> property.
        /// </summary>
        private int countBacking;

        /// <summary>
        /// The backing field for the <see cref="Capacity" /> property.
        /// </summary>
        private int capacityBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicArray{T}" /> class
        /// with the default capacity of 8 items and the default growth rate of 2.
        /// </summary>
        public DynamicArray()
            : this(8, 2d)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicArray{T}" /> class
        /// with a specified capacity of items and the default growth rate of 2.
        /// </summary>
        /// <param name="capacity">
        ///     The initial capacity of the <see cref="DynamicArray{T}" />.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="capacity" /> is smaller than 0.
        /// </exception>
        public DynamicArray(int capacity)
            : this(capacity, 2d)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicArray{T}" /> class
        /// with the default capacity of 8 items and a specified growth rate.
        /// </summary>
        /// <param name="growthRate">
        ///     The growth rate of the <see cref="DynamicArray{T}" />.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="growthRate" /> is smaller or equals 1.
        /// </exception>
        public DynamicArray(double growthRate)
            : this(8, growthRate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicArray{T}" /> class
        /// with a specified capacity of items and a specified growth rate.
        /// </summary>
        /// <param name="capacity">
        ///     The initial capacity of the <see cref="DynamicArray{T}" />.
        /// </param>
        /// <param name="growthRate">
        ///     The growth rate of the <see cref="DynamicArray{T}" />.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when<para />
        ///         - <paramref name="growthRate" /> is smaller or equals 1.<para />
        ///         - <paramref name="capacity" /> is smaller than 0.
        /// </exception>
        public DynamicArray(int capacity, double growthRate)
        {
            // throw exception if growth rate is smaller or equals 1
            if (growthRate <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(growthRate));
            }

            // throw exception if capacity is smaller than 0
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }
            
            this.Capacity = capacity;
            this.GrowthRate = growthRate;
            this.countBacking = 0;
            this.backing = new T[capacity];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicArray{T}" /> class
        /// with an <see cref="IEnumerable{T}" /> and a default growth rate of 2.
        /// </summary>
        /// <param name="items">
        ///     The items that shall be copied to the <see cref="DynamicArray{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="items" /> is null.
        /// </exception>
        public DynamicArray(IEnumerable<T> items)
            : this(2d, items)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicArray{T}" /> class
        /// with an <see cref="IEnumerable{T}" /> and a specified growth rate.
        /// </summary>
        /// <param name="growthRate">
        ///     The growth rate of the <see cref="DynamicArray{T}" />.
        /// </param>
        /// <param name="items">
        ///     The items that shall be copied to the <see cref="DynamicArray{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="items" /> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="growthRate" /> is smaller or equals 1.
        /// </exception>
        public DynamicArray(double growthRate, IEnumerable<T> items)
        {
            // throw exception if items is null
            if (object.ReferenceEquals(items, null))
            {
                throw new ArgumentNullException(nameof(items));
            }
            
            var count = items.Count();
            var array = count > 0 ? items.ToArray() : new T[8];
            this.countBacking = array.Length;
            this.GrowthRate = growthRate;
            this.Capacity = count;
            this.backing = array;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicArray{T}" /> class.
        /// </summary>
        /// <param name="data">
        ///     The serialized data of the <see cref="DynamicArray{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        protected DynamicArray(ObjectDataCollection data)
        {
            // throw exception if data is null
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            
            this.Capacity = data.GetValue<int>(nameof(this.Capacity));
            this.GrowthRate = data.GetValue<double>(nameof(this.GrowthRate));
            this.Count = data.GetValue<int>(nameof(this.Count));

            var load = new T[this.Capacity];

            for (int n = 0; n < this.Count; n++)
            {
                load[n] = data.GetValue<T>(n.ToString());
            }

            this.backing = load;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DynamicArray{T}" /> class.
        /// </summary>
        ~DynamicArray()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Raised when the <see cref="Capacity" /> property has been changed.
        /// </summary>
        protected event EventHandler CapacityChanged;

        /// <summary>
        /// Raised when the <see cref="Count" /> property has been changed.
        /// </summary>
        protected event EventHandler CountChanged;

        /// <summary>
        /// Raised when the <see cref="GrowthRate" /> property has been changed.
        /// </summary>
        protected event EventHandler GrowthRateChanged;

        /// <summary>
        /// Gets the capacity of the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <value>
        ///     Contains the capacity of the <see cref="DynamicArray{T}" />.
        /// </value>
        public int Capacity
        {
            get
            {
                return this.capacityBacking;
            }

            private set
            {
                this.capacityBacking = value;
                this.RaiseCapacityChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the growth rate of the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <value>
        ///     Contains the growth rate of the <see cref="DynamicArray{T}" />.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="value" /> smaller or equals 1.
        /// </exception>
        public double GrowthRate
        {
            get
            {
                return this.growthRateBacking;
            }

            private set
            {
                // throw exception if value is smaller or equals 1
                if (value <= 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                
                this.growthRateBacking = value;
                this.RaiseGrowthRateChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the amount of items in the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <value>
        ///     Contains the amount of items in the <see cref="DynamicArray{T}" />.
        /// </value>
        /// <remarks>
        ///     Implements <see cref="ICollection{T}.Count" /> implicitly.
        /// </remarks>
        public int Count
        {
            get
            {
                return this.countBacking;
            }

            private set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                
                this.countBacking = value;
                this.RaiseCountChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="DynamicArray{T}" /> is read-only.
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether the <see cref="DynamicArray{T}" /> is read-only.
        /// </value>
        /// <remarks>
        ///     Implements <see cref="ICollection{T}.IsReadOnly" /> implicitly.
        /// </remarks>
        public bool IsReadOnly { get; } = false;

        /// <summary>
        /// Gets the usage percent of the <see cref="DynamicArray{T}" />.
        /// The range is between 0.0 and 1.0.
        /// </summary>
        /// <value>
        ///     Contains the usage percent of the <see cref="DynamicArray{T}" />.
        /// </value>
        public double UsagePercent
        {
            get { return (double)this.Count / this.Capacity; }
        }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <param name="index">
        ///     The index of the item that shall be returned or set.
        /// </param>
        /// <returns>
        ///     Returns the item at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="index" /> is smaller than 0 or greater or equals <see cref="Count" />.
        /// </exception>
        /// <remarks>
        ///     Implements <see cref="IList{T}.this[int]" /> implicitly.
        /// </remarks>
        public T this[int index]
        {
            get
            {
                try
                {
                    return this.GetItem(index);
                }
                catch (ArgumentOutOfRangeException exc)
                {
                    // throw exception if index is out of range
                    throw new ArgumentOutOfRangeException($"The value of the {nameof(index)} paramter has been out of range.", exc);
                }
            }

            set
            {
                try
                {
                    this.ReplaceItem(index, value);
                }
                catch (ArgumentOutOfRangeException exc)
                {
                    // throw exception if index is out of range.
                    throw new ArgumentOutOfRangeException($"The value of the {nameof(index)} paramter has been out of range.", exc);
                }
            }
        }

        /// <summary>
        /// Adds an item to the end of the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <param name="item">
        ///     The item that shall be added.
        /// </param>
        /// <remarks>
        ///     Implements <see cref="ICollection{T}.Add(T)" /> implicitly.
        /// </remarks>
        public void Add(T item) => this.AddItem(item);

        /// <summary>
        /// Adds items stored in an <see cref="IEnumerable{T}" /> to the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <param name="items">
        ///     The items that shall be added.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="items" /> is <c>null</c>.</para>
        /// </exception>
        public void AddRange(IEnumerable<T> items)
        {
            // throw exception if items is null
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var item in items)
            {
                this.AddItem(item);
            }
        }
        
        /// <summary>
        /// Adds items stored in an array of <typeparamref name="T" /> to the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <param name="items">
        ///     The items that shall be added.
        /// </param>
        public void AddRange(params T[] items) => this.AddRange((IEnumerable<T>)items);
        
        /// <summary>
        /// Moves an item in the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <param name="sourceIndex">
        ///     The index of the item to move.
        /// </param>
        /// <param name="destinationIndex">
        ///     The destination position where the item should be moved.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if:</para>
        ///     <para>  - <paramref name="sourceIndex" /> is less than <c>0</c> or greater or equals <see cref="Count" />.</para>
        ///     <para>  - <paramref name="destinationIndex" /> is less than <c>0</c> or greater or equals <see cref="Count" />.</para>
        /// </exception>
        public void Move(int sourceIndex, int destinationIndex)
            => this.MoveItem(sourceIndex, destinationIndex);

        /// <summary>
        /// Removes the first occurrence of a specific item in the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <param name="item">
        ///     The item that shall be removed.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the item has been found in the <see cref="DynamicArray{T}" />.
        /// </returns>
        /// <remarks>
        ///     Implements <see cref="ICollection{T}.Remove(T)" /> implicitly.
        /// </remarks>
        public bool Remove(T item)
        {
            int index = this.IndexOf(item);

            // return false if item not found
            if (index < 0)
            {
                return false;
            }

            // remove item and return true
            this.RemoveItem(index);
            return true;
        }

        /// <summary>
        /// Gets the index of the first occurrence of an item in the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <param name="item">
        ///     The item that shall be searched for.
        /// </param>
        /// <returns>
        ///     Returns the index of the first occurrence, or -1 if <paramref name="item" /> could not be found.
        /// </returns>
        /// <remarks>
        ///     Implements <see cref="IList{T}.IndexOf(T)" /> implicitly.
        /// </remarks>
        public int IndexOf(T item)
        {
            this.semaphore.Wait();
            try
            {
                for (int n = 0; n < this.countBacking; n++)
                {
                    if (object.Equals(this.backing[n], item) || object.ReferenceEquals(this.backing[n], item))
                    {
                        // item found
                        return n;
                    }
                }

                // no item found
                return -1;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Inserts an item at a specified index in the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <param name="index">
        ///     The index where <paramref name="item" /> shall be inserted.
        /// </param>
        /// <param name="item">
        ///     The item that shall be inserted.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="index" /> is smaller than 0 or greater than <see cref="Count" />.
        /// </exception>
        /// <remarks>
        ///     Implements <see cref="IList{T}.Insert(int, T)" />.
        /// </remarks>
        public void Insert(int index, T item)
        {
            // throw exception if index is out of range
            if (index < 0 || index > this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.InsertItem(index, item);
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">
        ///     The index of the item that shall be removed.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="index" /> is smaller than 0 or greater or equals <see cref="Count" />.
        /// </exception>
        /// <remarks>
        ///     Implements <see cref="IList{T}.RemoveAt(int)" /> implicitly.
        /// </remarks>
        public void RemoveAt(int index)
        {
            // throw exception if index is out of range
            if (index < 0 || index >= this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.RemoveItem(index);
        }

        /// <summary>
        /// Removes all items from the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <remarks>
        ///     Implements <see cref="ICollection{T}.Clear()" /> implicitly.
        /// </remarks>
        public void Clear() => this.ResetItems();

        /// <summary>
        /// Determines whether the <see cref="DynamicArray{T}" /> contains a specified item.
        /// </summary>
        /// <param name="item">
        ///     The item to locate in the <see cref="DynamicArray{T}" />.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the element could be found.
        /// </returns>
        /// <remarks>
        ///     Implements <see cref="ICollection{T}.Contains(T)" /> implicitly.
        /// </remarks>
        public bool Contains(T item)
        {
            this.semaphore.Wait();
            try
            {
                var ret = this.backing.Contains(item);
                return ret;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Copies the items of the <see cref="DynamicArray{T}" /> to an <see cref="Array" />.
        /// </summary>
        /// <param name="array">
        ///     The <see cref="Array" /> where the items shall be copied.
        /// </param>
        /// <param name="arrayIndex">
        ///     The index inside of <paramref name="array" /> where the copying shall start.
        /// </param>
        /// <remarks>
        ///     Implements <see cref="ICollection{T}.CopyTo(T[], int)" /> implicitly.
        /// </remarks>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.semaphore.Wait();
            try
            {
                this.backing.CopyTo(array, arrayIndex);
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Gets the object data of the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <param name="data">
        ///     The data that shall be serialized.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown when <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        ///     Implements <see cref="ISerializable.GetObjectData(ObjectDataCollection)" /> implicitly.
        /// </remarks>
        public virtual void GetObjectData(ObjectDataCollection data)
        {
            // throw exception if data is null
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.semaphore.Wait();
            try
            {
                data.AddValue(nameof(this.Capacity), this.Capacity);
                data.AddValue(nameof(this.Count), this.Count);
                data.AddValue(nameof(this.GrowthRate), this.GrowthRate);

                for (int n = 0; n < this.Count; n++)
                {
                    data.AddValue(n.ToString(), this.backing[n]);
                }
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Gets a new <see cref="IEnumerator{T}" /> for the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <returns>
        ///     Returns the generated <see cref="IEnumerator{T}" />.
        /// </returns>
        /// <remarks>
        ///     Implements <see cref="IEnumerable{T}.GetEnumerator()" />.
        /// </remarks>
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        /// <summary>
        /// Gets an enumerator that iterates through the <see cref="IEnumerable" />.
        /// </summary>
        /// <returns>
        ///     Returns an enumerator that iterates through the <see cref="IEnumerable" />.
        /// </returns>
        /// <remarks>
        ///     Implements <see cref="IEnumerable.GetEnumerator()" /> explicitly.
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Releases all resources reserved by the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <remarks>
        ///     Implements <see cref="IDisposable.Dispose()" /> implicitly.
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
        protected virtual void Dispose(bool managed)
        {
            this.semaphore.Dispose();
            if (managed)
            {
                this.backing = null;
                this.countBacking = 0;
                this.growthRateBacking = 0d;
            }
        }

        /// <summary>
        /// Adds an element at the end of the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <param name="item">
        ///     The item that shall be added.
        /// </param>
        /// <remarks>
        ///     This method is virtual.
        /// </remarks>
        protected virtual void AddItem(T item) => this.InsertItem(this.Count, item);

        /// <summary>
        /// Moves an item in the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <param name="sourceIndex">
        ///     The index of the item to move.
        /// </param>
        /// <param name="destinationIndex">
        ///     The destination position where the item should be moved.
        /// </param>
        protected virtual void MoveItem(int sourceIndex, int destinationIndex)
        {
            this.semaphore.Wait();

            try
            {
                if (sourceIndex < 0 || sourceIndex >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(sourceIndex));
                }

                if (destinationIndex < 0 || destinationIndex >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(destinationIndex));
                }

                var item = this[sourceIndex];
                var direction = Math.Sign(destinationIndex - sourceIndex);

                for (var n = sourceIndex;
                     direction > 0 ? n < destinationIndex : n > destinationIndex;
                     n += direction)
                {
                    this[sourceIndex] = this[sourceIndex + direction];
                }

                this[destinationIndex] = item;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Gets the item at a specified index.
        /// </summary>
        /// <param name="index">
        ///     The index of the item.
        /// </param>
        /// <returns>
        ///     Returns the item at the specified index.
        /// </returns>
        /// <remarks>
        ///     This method is virtual.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="index" /> is smaller than 0 or greater or equals <see cref="Count" />.
        /// </exception>
        protected virtual T GetItem(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.semaphore.Wait();

            try
            {
                return this.backing[index];
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Inserts an item at a specified position.
        /// </summary>
        /// <param name="index">
        ///     The index where the item shall be inserted.
        /// </param>
        /// <param name="item">
        ///     The item that shall be inserted.
        /// </param>
        /// <remarks>
        ///     This method is virtual.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="index" /> is smaller than 0 or greater than <see cref="Count" />.
        /// </exception>
        protected virtual void InsertItem(int index, T item)
        {
            // throw exception if index is out of range
            if (index < 0 || index > this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.semaphore.Wait();
            
            try
            {
                // extend array size
                if (this.Count == this.Capacity)
                {
                    this.Capacity = this.Capacity > 0 ? (int)Math.Ceiling(this.Capacity * this.GrowthRate) : 1;
                    Array.Resize(ref this.backing, this.Capacity);
                }

                // move items after index one index back
                for (int n = this.Count - 1; n >= index; n--)
                {
                    this.backing[n + 1] = this.backing[n];
                }

                // insert item
                this.backing[index] = item;
            }
            finally
            {
                this.semaphore.Release();
            }

            // increment count
            this.Count++;
        }

        /// <summary>
        /// Replaces an item at a specified index.
        /// </summary>
        /// <param name="index">
        ///     The index of the item that shall be replaced.
        /// </param>
        /// <param name="item">
        ///     The item that shall replace the existing one.
        /// </param>
        /// <remarks>
        ///     This method is virtual.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="index" /> is smaller than 0 or greater or equals <see cref="Count" />.
        /// </exception>
        protected virtual void ReplaceItem(int index, T item)
        {
            // throw exception if index is out of range
            if (index < 0 || index >= this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.semaphore.Wait();

            try
            {
                this.backing[index] = item;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Removes the item at the specified index and moves all items after it forwards.
        /// </summary>
        /// <param name="index">
        ///     The index of the item that shall be removed.
        /// </param>
        /// <remarks>
        ///     This method is virtual.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="index" /> is smaller than 0 or greater or equals <see cref="Count" />.
        /// </exception>
        protected virtual void RemoveItem(int index)
        {
            this.semaphore.Wait();

            try
            {
                // throw exception if index is out of range
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                // set index item to default(T)
                this.backing[index] = default(T);

                // move all items after index to front
                for (int n = index + 1; n < this.Count; n++)
                {
                    this.backing[n - 1] = this.backing[n];
                }
            }
            finally
            {
                this.semaphore.Release();
            }

            this.Count--;
        }

        /// <summary>
        /// Resets the items in the <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <remarks>
        ///     This method is virtual.
        /// </remarks>
        protected virtual void ResetItems()
        {
            this.semaphore.Wait();

            try
            {
                // set items to default value
                for (int n = 0; n < this.countBacking; n++)
                {
                    this.backing[n] = default(T);
                }
            }
            finally
            {
                this.semaphore.Release();
            }

            // reset count
            this.Count = 0;
        }

        /// <summary>
        /// Raises the <see cref="CapacityChanged" /> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for the event handlers.
        /// </param>
        protected void RaiseCapacityChanged(EventArgs e) => this.CapacityChanged?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="CountChanged" /> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for the event handlers.
        /// </param>
        protected void RaiseCountChanged(EventArgs e) => this.CountChanged?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="GrowthRateChanged" /> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for the event handlers.
        /// </param>
        protected void RaiseGrowthRateChanged(EventArgs e) => this.GrowthRateChanged?.Invoke(this, e);

        /// <summary>
        /// Provides an enumerator for a <see cref="DynamicArray{T}" />.
        /// </summary>
        /// <remarks>
        ///     Implements <see cref="IEnumerator{T}" />, <see cref="IEnumerator" />, <see cref="IDisposable" />.
        /// </remarks>
        public struct Enumerator
            : IEnumerator<T>, IEnumerator, IDisposable
        {
            /// <summary>
            /// The <see cref="DynamicArray{T}" /> that shall be enumerated.
            /// </summary>
            private DynamicArray<T> array;

            /// <summary>
            /// The current position in <see cref="array" />.
            /// </summary>
            private int currentPosition;

            /// <summary>
            /// Determines whether the semaphore of <see cref="array" /> is held by the enumerator.
            /// </summary>
            private bool isSemaphoreHeld;

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator" /> struct.
            /// </summary>
            /// <param name="array">
            ///     The <see cref="DynamicArray{T}" /> that shall be enumerated.
            /// </param>
            public Enumerator(DynamicArray<T> array)
            {
                if (object.ReferenceEquals(array, null))
                {
                    throw new ArgumentNullException(nameof(array));
                }

                this.array = array;
                this.currentPosition = -1;
                this.isSemaphoreHeld = false;
            }

            /// <summary>
            /// Gets the current element of the <see cref="DynamicArray{T}" />.
            /// </summary>
            /// <value>
            ///     Contains the current element of the <see cref="DynamicArray{T}" />.
            /// </value>
            /// <exception cref="InvalidOperationException">
            ///     Thrown when the <see cref="Enumerator" /> has not been moved.
            /// </exception>
            /// <remarks>
            ///     Implements <see cref="IEnumerator{T}.Current" /> implicitly.
            /// </remarks>
            public T Current
            {
                get
                {
                    if (this.currentPosition < 0 || this.currentPosition >= this.array.Count)
                    {
                        throw new InvalidOperationException("The IEnumerator has to be moved once to access a value.");
                    }

                    return this.array.backing[this.currentPosition];
                }
            }

            /// <summary>
            /// Gets the current element of the <see cref="IEnumerator" />.
            /// </summary>
            /// <value>
            ///     Contains the current element of the <see cref="IEnumerator" />.
            /// </value>
            /// <remarks>
            ///     Implements <see cref="IEnumerator.Current" /> explicitly.
            /// </remarks>
            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            /// <summary>
            /// Releases all resources allocated by the <see cref="Enumerator" />.
            /// </summary>
            /// <remarks>
            ///     Implements <see cref="IDisposable.Dispose()" /> implicitly.
            /// </remarks>
            [System.Diagnostics.CodeAnalysis.SuppressMessage(
                "Microsoft.Usage",
                "CA1816:CallGCSuppressFinalizeCorrectly",
                Justification = "False Positive; actually calls GC.SuppressFinalize() on itself.")]
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Advances the <see cref="Enumerator" /> to the next element of the <see cref="DynamicArray{T}" />.
            /// </summary>
            /// <returns>
            ///     Returns a value indicating whether the <see cref="Enumerator" /> could successfully be moved.
            /// </returns>
            /// <remarks>
            ///     Implements <see cref="IEnumerator.MoveNext()" /> implicitly.
            /// </remarks>
            public bool MoveNext()
            {
                if (this.currentPosition < 0 && !this.isSemaphoreHeld)
                {
                    this.array.semaphore.Wait();
                    this.isSemaphoreHeld = true;
                }

                var ret = ++this.currentPosition < this.array.Count;
                if (!ret && this.isSemaphoreHeld)
                {
                    this.array.semaphore.Release();
                    this.isSemaphoreHeld = false;
                }

                return ret;
            }

            /// <summary>
            /// Sets the <see cref="Enumerator" /> to its initial position,
            /// which is before the first element in the <see cref="DynamicArray{T}" />.
            /// </summary>
            /// <remarks>
            ///     Implements <see cref="IEnumerator.Reset()" /> implicitly.
            /// </remarks>
            public void Reset()
            {
                if (this.isSemaphoreHeld)
                {
                    this.array.semaphore.Release();
                    this.isSemaphoreHeld = false;
                }

                this.currentPosition = -1;
            }

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources.
            /// </summary>
            /// <param name="managed">
            ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
            /// </param>
            private void Dispose(bool managed)
            {
                if (this.isSemaphoreHeld)
                {
                    this.array.semaphore.Release();
                }
                
                this.array = null;
                this.currentPosition = 0;
                this.isSemaphoreHeld = false;
            }
        }
    }
}
