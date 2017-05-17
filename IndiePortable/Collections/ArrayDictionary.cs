// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayDictionary.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ArrayDictionary&lt;TKey, TValue&gt; class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Formatter;

    /// <summary>
    /// Provides an array-based dictionary.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the key.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the value.
    /// </typeparam>
    /// <remarks>
    ///     Implements <see cref="IDictionary{TKey, TValue}" />, <see cref="ISerializable" />, <see cref="IDisposable" /> explicitly.
    /// </remarks>
    [Serializable]
    [ComVisible(true)]
    public class ArrayDictionary<TKey, TValue>
        : IDictionary<TKey, TValue>, ISerializable, IDisposable
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// The <see cref="SemaphoreSlim" /> that handles thread synchronization for the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// The entries of the <see cref="ArrayDictionary{TKey, TValue}" /> combined with a hashed key.
        /// </summary>
        private DynamicArray<KeyValuePair<TKey, TValue>>[] hashedEntries;

        /// <summary>
        /// The keys stored in the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        private DynamicArray<TKey> keys;

        /// <summary>
        /// The values stored in the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        private DynamicArray<TValue> values;
        
        /// <summary>
        /// The backing field for the <see cref="Keys" /> property.
        /// </summary>
        private readonly ReadOnlyCollection<TKey> keysBacking;
        
        /// <summary>
        /// The backing field for the <see cref="Values" /> property.
        /// </summary>
        private readonly ReadOnlyCollection<TValue> valuesBacking;

        /// <summary>
        /// The backing field for the <see cref="Count" /> property.
        /// </summary>
        private int countBacking;

        /// <summary>
        /// Determines whether the <see cref="ArrayDictionary{TKey, TValue}" /> is disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayDictionary{TKey, TValue}" /> class.
        /// </summary>
        public ArrayDictionary()
            : this(31)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayDictionary{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="capacity">
        ///     The capacity of the <see cref="ArrayDictionary{TKey, TValue}" />. Must be greater than 0.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="capacity" /> is not greater than 0.
        /// </exception>
        public ArrayDictionary(int capacity)
        {
            // throw exception if capacity is smaller or equals 0
            if (capacity <= 0)
            {
                throw new ArgumentException($"The {nameof(capacity)} parameter must be greater than 0.", nameof(capacity));
            }

            this.Capacity = capacity;
            this.countBacking = 0;
            this.hashedEntries = new DynamicArray<KeyValuePair<TKey, TValue>>[capacity];
            this.keys = new DynamicArray<TKey>();
            this.keysBacking = new ReadOnlyCollection<TKey>(this.keys);
            this.values = new DynamicArray<TValue>();
            this.valuesBacking = new ReadOnlyCollection<TValue>(this.values);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        /// <value>
        ///     Contains the number of elements contained in the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </value>
        /// <remarks>
        ///     Implements <see cref="ICollection{T}.Count" /> implicitly.
        /// </remarks>
        public int Count
        {
            get { return this.countBacking; }
        }

        /// <summary>
        /// Gets the capacity of the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// This does not affect the amount of items that can be stored in the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        /// <value>
        ///     Contains the capacity of the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </value>
        public int Capacity { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ArrayDictionary{TKey, TValue}" /> is read-only.
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether the <see cref="ArrayDictionary{TKey, TValue}" /> is read-only.
        /// </value>
        /// <remarks>
        ///     Implements <see cref="ICollection{T}.IsReadOnly" /> implicitly.
        /// </remarks>
        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the <see cref="ICollection{T}" /> containing the keys of the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="ICollection{T}" /> containing the keys of the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </value>
        /// <remarks>
        ///     Implements <see cref="IDictionary{TKey, TValue}.Keys" /> implicitly.
        /// </remarks>
        public ICollection<TKey> Keys
        {
            get { return this.keysBacking; }
        }

        /// <summary>
        /// Gets the <see cref="ICollection{T}" /> containing the values in the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="ICollection{T}" /> containing the values in the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </value>
        /// <remarks>
        ///     Implements <see cref="IDictionary{TKey, TValue}.Values" /> implicitly.
        /// </remarks>
        public ICollection<TValue> Values
        {
            get { return this.valuesBacking; }
        }

        /// <summary>
        /// Gets or sets the <typeparamref name="TValue"/> with the specified key.
        /// </summary>
        /// <value>
        ///     Contains the <typeparamref name="TValue" /> with the specified key.
        /// </value>
        /// <param name="key">
        ///     The key for the desired <typeparamref name="TValue" />.
        /// </param>
        /// <returns>
        ///     Returns the <typeparamref name="TValue" /> associated with <paramref name="key" />.
        /// </returns>
        public TValue this[TKey key]
        {
            get
            {
                var success = this.TryGetValue(key, out var ret);

                if (!success)
                {
                    throw new ArgumentException("The specified key could not be found.", nameof(key));
                }

                return ret;
            }

            set
            {
                var success = this.TrySetValue(key, value);
                if (!success)
                {
                    throw new InvalidOperationException("The value could not be set.");
                }
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        /// <param name="item">
        ///     The object to add to the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     Thrown if the key already exists in the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </exception>
        /// <remarks>
        ///     Implements <see cref="ICollection{T}.Add(T)" /> implicitly.
        /// </remarks>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            // throw exception if key is already in the dictionary
            if (this.ContainsKey(item.Key))
            {
                throw new ArgumentException($"The key already exists in the {nameof(ArrayDictionary<TKey, TValue>)}.");
            }

            this.AddItem(item);
        }

        /// <summary>
        /// Adds an item to the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        /// <param name="key">
        ///     The key of the item.
        /// </param>
        /// <param name="value">
        ///     The value of the item.
        /// </param>
        /// <remarks>
        ///     Implements <see cref="IDictionary{TKey, TValue}.Add(TKey, TValue)" /> implicitly.
        /// </remarks>
        public void Add(TKey key, TValue value) => this.Add(new KeyValuePair<TKey, TValue>(key, value));

        /// <summary>
        /// Removes all items from the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        /// <remarks>
        ///     Implements <see cref="ICollection{T}.Clear()" /> implicitly.
        /// </remarks>
        public void Clear() => this.Clear();

        /// <summary>
        /// Determines whether the <see cref="ArrayDictionary{TKey, TValue}" /> contains the specified item.
        /// </summary>
        /// <param name="item">
        ///     The item that shall be searched for.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the specified <paramref name="item" />
        ///     could be found in the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </returns>
        /// <remarks>
        ///     Implements <see cref="ICollection{T}.Contains(T)" /> implicitly.
        /// </remarks>
        public bool Contains(KeyValuePair<TKey, TValue> item) => this.ContainsItem(item);

        /// <summary>
        /// Determines whether the <see cref="ArrayDictionary{TKey, TValue}" /> contains the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key that shall be searched for.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the specified <paramref name="key" />
        ///     could be found in the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </returns>
        /// <remarks>
        ///     Implements <see cref="IDictionary{TKey, TValue}.ContainsKey(TKey)" /> implicitly.
        /// </remarks>
        public bool ContainsKey(TKey key) => this.ContainsKeyItem(key);

        /// <summary>
        /// Copies the elements of the <see cref="ArrayDictionary{TKey, TValue}" /> to an array starting at a specified array index.
        /// </summary>
        /// <param name="array">
        ///     The target array that shall be populated.
        /// </param>
        /// <param name="arrayIndex">
        ///     The start index in the array.
        /// </param>
        /// <remarks>
        ///     Implements <see cref="ICollection{T}.CopyTo(T[], int)" /> implicitly.
        /// </remarks>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => this.CopyToArray(array, arrayIndex);

        /// <summary>
        /// Populates a specified <see cref="ObjectDataCollection" /> instance with data from the <see cref="ArrayDictionary{TKey, TValue}" /> instance.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> that shall be populated.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        ///     Implements <see cref="ISerializable.GetObjectData(ObjectDataCollection)" />.
        /// </remarks>
        public void GetObjectData(ObjectDataCollection data)
        {
            // throw exception if data is null
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.semaphore.Wait();
            try
            {
                data.AddValue(nameof(this.Count), this.Count);

                var n = 0;
                foreach (var kv in this)
                {
                    data.AddValue(n.ToString(), kv);
                    n++;
                }
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">
        ///     The item that shall be removed.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the item could be found.
        /// </returns>
        /// <remarks>
        ///     Implements <see cref="ICollection{T}.Remove(T)" /> implicitly.
        /// </remarks>
        public bool Remove(KeyValuePair<TKey, TValue> item) => this.Remove(item.Key);

        /// <summary>
        /// Removes the item with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key specifying the item to remove.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the item could be found.
        /// </returns>
        /// <remarks>
        ///     Implements <see cref="IDictionary{TKey, TValue}.Remove(TKey)" /> implicitly.
        /// </remarks>
        public bool Remove(TKey key) => this.RemoveItem(key);

        /// <summary>
        /// Tries to get the value associated with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key that is associated with the desired value.
        /// </param>
        /// <param name="value">
        ///     The value associated with <paramref name="key" />.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether <paramref name="key" /> could be found.
        /// </returns>
        /// <remarks>
        ///     Implements <see cref="IDictionary{TKey, TValue}.TryGetValue(TKey, out TValue)" /> implicitly.
        /// </remarks>
        public bool TryGetValue(TKey key, out TValue value)
        {
            try
            {
                value = this.GetItem(key);
                return true;
            }
            catch (Exception)
            {
                value = default(TValue);
                return false;
            }
        }
        
        /// <summary>
        /// Tries to set the value associated with a specified key.
        /// </summary>
        /// <param name="key">
        ///     The key whose value shall be replaced.
        /// </param>
        /// <param name="newValue">
        ///     The new value associated with <paramref name="key" />.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the operation has been successful.
        /// </returns>
        public bool TrySetValue(TKey key, TValue newValue)
        {
            this.semaphore.Wait();
            try
            {
                this.ReplaceItem(key, newValue);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Gets an enumerator for the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        /// <returns>
        ///     Returns an enumerator for the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </returns>
        /// <remarks>
        ///     Implements <see cref="IEnumerable{T}.GetEnumerator()" /> implicitly.
        /// </remarks>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new Enumerator(this);

        /// <summary>
        /// Gets an enumerator for the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        /// <returns>
        ///     Returns an enumerator for the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </returns>
        /// <remarks>
        ///     Implements <see cref="IEnumerable.GetEnumerator()" /> explicitly.
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Releases all resources reserved by the <see cref="ArrayDictionary{TKey, TValue}" />.
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
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                }

                this.semaphore.Dispose();

                this.countBacking = 0;
                this.hashedEntries = null;
                this.keys = null;
                this.values = null;

                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        /// <param name="item">
        ///     The item that shall be added.
        /// </param>
        /// <remarks>
        ///     <para>This method is supposed to be overwritten by derived types if they implement their own item adding logic.</para>
        /// </remarks>
        protected virtual void AddItem(KeyValuePair<TKey, TValue> item)
        {
            this.semaphore.Wait();
            try
            {
                // check whether key already exists
                if (this.ContainsKeyItem(item.Key))
                {
                    throw new InvalidOperationException($"The specified key already exists in the {nameof(ArrayDictionary<TKey, TValue>)}.");
                }

                // generate key hash
                var hash = HashGenerator.GetHash(item.Key?.GetHashCode() ?? 0, this.Capacity);
                if (this.hashedEntries[hash] == null)
                {
                    this.hashedEntries[hash] = new DynamicArray<KeyValuePair<TKey, TValue>>();
                }

                // add to hashed entries
                this.hashedEntries[hash].Add(item);

                // add to keys and values
                this.keys.Add(item.Key);
                this.values.Add(item.Value);

                this.countBacking++;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        protected virtual void ClearItems()
        {
            this.semaphore.Wait();
            try
            {
                for (var n = 0; n < this.countBacking; n++)
                {
                    this.hashedEntries[n] = default(DynamicArray<KeyValuePair<TKey, TValue>>);
                }
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Determines whether the <see cref="ArrayDictionary{TKey, TValue}" /> contains a specified key.
        /// </summary>
        /// <param name="key">
        ///     The key that shall be searched for.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the specified <paramref name="key" /> could be found in the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </returns>
        protected virtual bool ContainsKeyItem(TKey key)
        {
            this.semaphore.Wait();
            try
            {
                return this.Keys.Contains(key);
            }
            finally
            {
                this.semaphore.Release();
            }
        }
        
        /// <summary>
        /// Copies the elements of the <see cref="ArrayDictionary{TKey, TValue}" /> to an array starting at a specified array index.
        /// </summary>
        /// <param name="array">
        ///     The target array that shall be populated.
        /// </param>
        /// <param name="arrayIndex">
        ///     The start index in the array.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="array" /> is <c>null</c>.</para>
        /// </exception>
        protected virtual void CopyToArray(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            // throw exception if array is null
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            this.semaphore.Wait();

            try
            {
                var currentRepresentation = this.ToArray();
                
                for (var current = 0; current < currentRepresentation.Length; current++)
                {
                    array[arrayIndex + current] = currentRepresentation[current];
                }
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Determines whether the <see cref="ArrayDictionary{TKey, TValue}" /> contains a specified item.
        /// </summary>
        /// <param name="item">
        ///     The item that shall be searched for.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the specified <paramref name="item" /> could be found in the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </returns>
        protected virtual bool ContainsItem(KeyValuePair<TKey, TValue> item)
            => this.Any(i => i.Key?.Equals(item.Key) ?? item.Key?.Equals(i.Key) ?? true);

        /// <summary>
        /// Gets the value associated with a specified key.
        /// </summary>
        /// <param name="key">
        ///     The key that is associated with the desired value.
        /// </param>
        /// <returns>
        ///     Returns the value associated with <paramref name="key" />.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Thrown if the specified <paramref name="key" /> could not be found in the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </exception>
        protected virtual TValue GetItem(TKey key)
            => this.First(i => i.Key?.Equals(key) ?? key?.Equals(i.Key) ?? true).Value;

        /// <summary>
        /// Replaces the value associated with the specified key by a new value.
        /// </summary>
        /// <param name="key">
        ///     The key that is associated with the old value.
        /// </param>
        /// <param name="newValue">
        ///     The new value for <paramref name="key" />.
        /// </param>
        protected virtual void ReplaceItem(TKey key, TValue newValue)
        {
            this.semaphore.Wait();
            try
            {
                if (!this.ContainsKeyItem(key))
                {
                    throw new ArgumentException($@"The specified key could not be found in the {nameof(ArrayDictionary<TKey, TValue>)}.", nameof(key));
                }

                // generate hash
                var hash = HashGenerator.GetHash(key?.GetHashCode() ?? 0, this.Capacity);
                
                var index = this.hashedEntries[hash].IndexOf(
                    this.hashedEntries[hash].First(kv => kv.Key?.Equals(key) ?? key?.Equals(kv.Key) ?? true));

                this.hashedEntries[hash][index] = new KeyValuePair<TKey, TValue>(key, newValue);

                // set values
                var keyIndex = this.keys.IndexOf(key);
                this.values[keyIndex] = newValue;
            }
            catch
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Removes the specified item from the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        /// <param name="item">
        ///     The item that shall be removed.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the specified item could be found.
        /// </returns>
        protected virtual bool RemoveItem(KeyValuePair<TKey, TValue> item)
        {
            this.semaphore.Wait();
            try
            {
                if (!this.ContainsItem(item))
                {
                    return false;
                }

                // get hash
                var hash = HashGenerator.GetHash(item.Key?.GetHashCode() ?? 0, this.Capacity);
                var entryList = this.hashedEntries[hash];
                var entry = entryList.First(e => e.Key?.Equals(item.Key) ?? item.Key?.Equals(e.Key) ?? true);
                entryList.Remove(entry);

                // remove key and value
                var index = this.keysBacking.IndexOf(item.Key);
                this.keys.RemoveAt(index);
                this.values.RemoveAt(index);
                this.countBacking--;
                return true;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Removes the item with the specified key from the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        /// <param name="key">
        ///     The key of the item that shall be removed.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether an item with the specified <paramref name="key" /> could be found.
        /// </returns>
        protected virtual bool RemoveItem(TKey key)
        {
            this.semaphore.Wait();
            try
            {
                if (!this.ContainsKeyItem(key))
                {
                    return false;
                }

                // get hash
                var hash = HashGenerator.GetHash(key?.GetHashCode() ?? 0, this.Capacity);
                var entryList = this.hashedEntries[hash];
                var entry = entryList.First(e => e.Key?.Equals(key) ?? key?.Equals(e.Key) ?? true);
                entryList.Remove(entry);

                // remove key and value
                var index = this.keysBacking.IndexOf(key);
                this.keys.RemoveAt(index);
                this.values.RemoveAt(index);
                this.countBacking--;
                return true;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Provides an enumerator for an <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        /// <remarks>
        ///     Implements <see cref="IEnumerator{T}" />, <see cref="IEnumerator" />, <see cref="IDisposable" /> explicitly.
        /// </remarks>
        public struct Enumerator
            : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable
        {
            /// <summary>
            /// The <see cref="ArrayDictionary{TKey, TValue}" /> that shall be enumerated.
            /// </summary>
            private ArrayDictionary<TKey, TValue> enumerable;

            /// <summary>
            /// The current position of the <see cref="Enumerator" />.
            /// </summary>
            private int currentIndex;

            /// <summary>
            /// Initializes a new instance of the <see cref="ArrayDictionary{TKey, TValue}.Enumerator" /> struct.
            /// </summary>
            /// <param name="enumerable">
            ///     The <see cref="ArrayDictionary{TKey, TValue}" /> that shall be enumerated.
            /// </param>
            /// <exception cref="ArgumentNullException">
            ///     <para>Thrown if <paramref name="enumerable" /> is <c>null</c>.</para>
            /// </exception>
            public Enumerator(ArrayDictionary<TKey, TValue> enumerable)
            {
                this.enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
                this.currentIndex = -1;
            }

            /// <summary>
            /// Gets the current element of the <see cref="Enumerator" />.
            /// </summary>
            /// <value>
            ///     Contains the current element of the <see cref="Enumerator" />.
            /// </value>
            /// <remarks>
            ///     Implements <see cref="IEnumerator{T}.Current" /> implicitly.
            /// </remarks>
            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    return new KeyValuePair<TKey, TValue>(
                        this.enumerable.keys[this.currentIndex],
                        this.enumerable.values[this.currentIndex]);
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
            /// Releases any resources reserved by the <see cref="Enumerator" />.
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
                if (this.currentIndex >= 0)
                {
                    this.enumerable.semaphore.Wait();
                }

                this.currentIndex = 0;
                this.enumerable = null;
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Advances the position of the <see cref="Enumerator" /> to the next element.
            /// </summary>
            /// <returns>
            ///     Returns a value indicating whether the <see cref="Enumerator" /> is in the list.
            /// </returns>
            /// <remarks>
            ///     Implements <see cref="IEnumerator.MoveNext()" /> implicitly.
            /// </remarks>
            public bool MoveNext()
            {
                if (this.currentIndex < 0)
                {
                    this.enumerable.semaphore.Wait();
                }

                return ++this.currentIndex < this.enumerable.Count;
            }

            /// <summary>
            /// Resets the <see cref="Enumerator" /> before the first element.
            /// </summary>
            /// <remarks>
            ///     Implements <see cref="IEnumerator.Reset()" /> implicitly.
            /// </remarks>
            public void Reset()
            {
                if (this.currentIndex >= 0)
                {
                    this.enumerable.semaphore.Release();
                }

                this.currentIndex = -1;
            }
        }
    }
}
