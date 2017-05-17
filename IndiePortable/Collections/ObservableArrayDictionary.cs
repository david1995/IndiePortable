// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableArrayDictionary.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ObservableArrayDictionary&lt;TKey, TValue&gt; class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Runtime.InteropServices;
    using Formatter;

    /// <summary>
    /// Represents an array-based dictionary that notifies about changes.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the key.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the value.
    /// </typeparam>
    /// <remarks>
    ///     Derives from <see cref="ArrayDictionary{TKey, TValue}" />.
    /// </remarks>
    [Serializable]
    [ComVisible(true)]
    public class ObservableArrayDictionary<TKey, TValue>
        : ArrayDictionary<TKey, TValue>, INotifyCollectionChanged
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Determines whether the <see cref="ObservableArrayDictionary{TKey, TValue}" /> is disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableArrayDictionary{TKey, TValue}" /> class.
        /// </summary>
        public ObservableArrayDictionary()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableArrayDictionary{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="capacity">
        ///     The capacity of the <see cref="ObservableArrayDictionary{TKey, TValue}" />. Must be greater than 0.
        /// </param>
        public ObservableArrayDictionary(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Raised when the collection has been modified.
        /// </summary>
        /// <remarks>
        ///     Implements <see cref="INotifyCollectionChanged.CollectionChanged" /> implicitly.
        /// </remarks>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Raises the <see cref="CollectionChanged" /> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for the event handlers.
        /// </param>
        protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
                    => this.CollectionChanged?.Invoke(this, e);

        /// <summary>
        /// Adds an item to the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        /// <param name="item">
        ///     The item that shall be added.
        /// </param>
        /// <remarks>
        ///     Overrides <see cref="ArrayDictionary{TKey, TValue}.AddItem(KeyValuePair{TKey, TValue})" />.
        /// </remarks>
        protected override void AddItem(KeyValuePair<TKey, TValue> item)
        {
            base.AddItem(item);
            this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        /// <summary>
        /// Removes all items from the <see cref="ArrayDictionary{TKey, TValue}" />.
        /// </summary>
        /// <remarks>
        ///     Overrides <see cref="ArrayDictionary{TKey, TValue}.ClearItems()" />.
        /// </remarks>
        protected override void ClearItems()
        {
            base.ClearItems();
            this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        
        /// <summary>
        /// Replaces the value associated with the specified key by a new value.
        /// </summary>
        /// <param name="key">
        ///     The key that is associated with the old value.
        /// </param>
        /// <param name="newValue">
        ///     The new value for <paramref name="key" />.
        /// </param>
        /// <remarks>
        ///     Overrides <see cref="ArrayDictionary{TKey, TValue}.ReplaceItem(TKey, TValue)" />.
        /// </remarks>
        protected override void ReplaceItem(TKey key, TValue newValue)
        {

            // throw exception if key could not be found.
            if (!this.TryGetValue(key, out var oldValue))
            {
                throw new ArgumentException("The specified key could not be found.", nameof(key));
            }

            base.ReplaceItem(key, newValue);
            this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Replace,
                new KeyValuePair<TKey, TValue>(key, newValue),
                new KeyValuePair<TKey, TValue>(key, oldValue)));
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
        /// <remarks>
        ///     Overrides <see cref="ArrayDictionary{TKey, TValue}.RemoveItem(KeyValuePair{TKey, TValue})" />.
        /// </remarks>
        protected override bool RemoveItem(KeyValuePair<TKey, TValue> item)
        {
            var success = base.RemoveItem(item);
            if (success)
            {
                this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    item));
            }

            return success;
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
        /// <remarks>
        ///     Overrides <see cref="ArrayDictionary{TKey, TValue}.RemoveItem(TKey)" />.
        /// </remarks>
        protected override bool RemoveItem(TKey key)
        {
            if (!this.TryGetValue(key, out var oldItem))
            {
                return false;
            }

            var success = base.RemoveItem(key);
            if (success)
            {
                this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    new KeyValuePair<TKey, TValue>(key, oldItem)));
            }

            return success;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        /// <remarks>
        ///     <para>Overrides <see cref="ArrayDictionary{TKey, TValue}.Dispose(bool)" />.</para>
        /// </remarks>
        protected override void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                }

                base.Dispose(disposing);
                this.CollectionChanged = null;
                this.isDisposed = true;
            }
        }
    }
}
