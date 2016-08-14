// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewObservableDynamicArray.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ViewObservableDynamicArray{TIn, TOut} class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Formatter;

    /// <summary>
    /// Provides a Model-View-View Model (MVVM) compatible observable dynamic array with an input type and an output type.
    /// </summary>
    /// <typeparam name="TIn">
    ///     The type of the model items that shall be viewed.
    /// </typeparam>
    /// <typeparam name="TOut">
    ///     The view type of the items.
    /// </typeparam>
    /// <remarks>
    ///     <para>Implements <see cref="IObservableReadOnlyList{T}" />, <see cref="IDisposable" /> explicitly.</para>
    ///     <para>
    ///         Implements <see cref="IReadOnlyList{T}" />, <see cref="INotifyCollectionChanged" />
    ///         implicitly through <see cref="IObservableReadOnlyList{T}"/>
    ///     </para>
    ///     <para>
    ///         Implements <see cref="IReadOnlyCollection{T}" />, <see cref="IEnumerable{T}" />, <see cref="IEnumerable" />
    ///         implicitly through <see cref="IReadOnlyList{TOut}" />.
    ///     </para>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "Array suffix describes the functinality better than Collection suffix.")]
    [ComVisible(true)]
    public class ViewObservableDynamicArray<TIn, TOut>
        : IObservableReadOnlyList<TOut>, INotifyCollectionChanged, IList, IDisposable
        where TIn : class, IEquatable<TIn>
        where TOut : IViewType<TIn>
    {
        /// <summary>
        /// The <see cref="SemaphoreSlim" /> that synchronizes the thread access on the <see cref="ViewObservableDynamicArray{TIn, TOut}" />.
        /// </summary>
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// The backing field for the <see cref="Source" /> property.
        /// </summary>
        private ObservableDynamicArray<TIn> sourceBacking;

        /// <summary>
        /// The <see cref="ArrayDictionary{TIn, TOut}" /> that maps between the source and the destination object.
        /// </summary>
        private ArrayDictionary<TIn, TOut> mapping;

        /// <summary>
        /// The <see cref="DynamicArray{T}" /> containing the view models.
        /// </summary>
        private DynamicArray<TOut> viewModels;

        /// <summary>
        /// The backing field for the <see cref="IsDisposed" /> property.
        /// </summary>
        private bool isDisposedBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewObservableDynamicArray{TIn, TOut}" /> class.
        /// </summary>
        /// <param name="source">
        ///     The source <see cref="ObservableDynamicArray{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if:</para>
        ///     <para>  - <paramref name="source" /> is <c>null</c>.</para>
        /// </exception>
        public ViewObservableDynamicArray(ObservableDynamicArray<TIn> source)
            : this(source, vm => (TOut)Activator.CreateInstance(typeof(TOut), vm))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewObservableDynamicArray{TIn, TOut}" /> class.
        /// </summary>
        /// <param name="source">
        ///     The source <see cref="ObservableDynamicArray{T}" />.
        /// </param>
        /// <param name="viewModelGenerator">
        ///     The method callback used for generating the view models.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if:</para>
        ///     <para>  - <paramref name="source" /> is <c>null</c>.</para>
        ///     <para>  - <paramref name="viewModelGenerator" /> is <c>null</c>.</para>
        /// </exception>
        public ViewObservableDynamicArray(ObservableDynamicArray<TIn> source, Func<TIn, TOut> viewModelGenerator)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (viewModelGenerator == null)
            {
                throw new ArgumentNullException(nameof(viewModelGenerator));
            }
            
            this.sourceBacking = source;
            this.ViewModelGeneratorCallback = viewModelGenerator;
            this.mapping = new ArrayDictionary<TIn, TOut>();
            this.viewModels = new DynamicArray<TOut>();
            this.Source.CollectionChanged += this.Source_CollectionChanged;
            if (source.Count > 0)
            {
                this.AddItems(source);
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ViewObservableDynamicArray{TIn, TOut}" /> class.
        /// </summary>
        ~ViewObservableDynamicArray()
        {
            this.Dispose(false);
        }
        
        /// <summary>
        /// Raised when the <see cref="ViewObservableDynamicArray{TIn, TOut}" /> has been changed.
        /// </summary>
        /// <remarks>
        ///     Implements <see cref="INotifyCollectionChanged.CollectionChanged" /> implicitly.
        /// </remarks>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Gets a value indicating whether the <see cref="ViewObservableDynamicArray{TIn, TOut}" /> has been disposed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="ViewObservableDynamicArray{TIn, TOut}" /> has been disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get { return this.isDisposedBacking; }
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <value>
        ///     Contains the number of elements in the collection.
        /// </value>
        /// <remarks>
        ///     Implements <see cref="IReadOnlyCollection{TOut}.Count" /> implicitly.
        /// </remarks>
        public int Count
        {
            get { return this.Source.Count; }
        }

        /// <summary>
        /// Gets the source <see cref="ObservableDynamicArray{T}" />.
        /// </summary>
        /// <value>
        ///     Contains the source <see cref="ObservableDynamicArray{T}" />.
        /// </value>
        public ObservableDynamicArray<TIn> Source
        {
            get { return this.sourceBacking; }
        }

        /// <summary>
        /// Gets the method callback used for generating the view models.
        /// </summary>
        /// <value>
        ///     Contains the method callback used for generating the view models.
        /// </value>
        public Func<TIn, TOut> ViewModelGeneratorCallback { get; private set; }


        bool IList.IsFixedSize { get; } = false;


        bool IList.IsReadOnly { get; } = true;


        int ICollection.Count
        {
            get { return this.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ICollection" /> is synchronized (thread safe).
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether access to the <see cref="ICollection" /> is synchronized (thread safe).
        /// </value>
        bool ICollection.IsSynchronized { get; } = false;


        object ICollection.SyncRoot { get; } = null;


        object IList.this[int index]
        {
            get { return this[index]; }

            set { throw new InvalidOperationException(); }
        }

        /// <summary>
        /// Gets the <typeparamref name="TOut" /> at the specified index.
        /// </summary>
        /// <value>
        ///     Contains the <typeparamref name="TOut" /> at the specified index.
        /// </value>
        /// <param name="index">
        ///     The index at which the item shall be returned.
        /// </param>
        /// <returns>
        ///     Returns the <typeparamref name="TOut" /> at the specified index.
        /// </returns>
        /// <remarks>
        ///     Implements <see cref="IReadOnlyList{TOut}.this[int]" /> implicitly.
        /// </remarks>
        public TOut this[int index]
        {
            get { return this.viewModels[index]; }
        }

        /// <summary>
        /// Gets the <typeparamref name="TOut"/> specified by a <typeparamref name="TIn" /> value.
        /// </summary>
        /// <value>
        ///     Contains the <typeparamref name="TOut"/> specified by a <typeparamref name="TIn" /> value.
        /// </value>
        /// <param name="input">
        ///     The <typeparamref name="TIn" /> value associated with the desired <typeparamref name="TOut" /> value.
        /// </param>
        /// <returns>
        ///     Returns the <typeparamref name="TOut" /> specified by <paramref name="input" />.
        /// </returns>
        public TOut this[TIn input]
        {
            get { return this.mapping[input]; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="IEnumerator{TOut}" /> that can be used to iterate through the collection.
        /// </returns>
        /// <remarks>
        ///     Implements <see cref="IEnumerable{TOut}.GetEnumerator()" /> implicitly.
        /// </remarks>
        public IEnumerator<TOut> GetEnumerator() => new Enumerator(this);

        /// <summary>
        /// Gets an <see cref="Enumerator" /> for the <see cref="IEnumerable" />.
        /// </summary>
        /// <returns>
        ///     Returns an <see cref="IEnumerator" /> for the <see cref="IEnumerable" />.
        /// </returns>
        /// <remarks>
        ///     Implements <see cref="IEnumerable.GetEnumerator()" /> explicitly.
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        #region Explicit IList & ICollection Method implementations

        
        int IList.Add(object value)
        {
            throw new InvalidOperationException();
        }

        void IList.Clear()
        {
            throw new InvalidOperationException();
        }


        bool IList.Contains(object value)
            => value is TOut
             ? this.Contains((TOut)value)
             : false;


        void ICollection.CopyTo(Array array, int index)
            => this.mapping.Select(kv => kv.Value).ToArray().CopyTo(array, index);


        int IList.IndexOf(object value)
            => value is TOut
             ? this.viewModels.IndexOf((TOut)value)
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

        #endregion

        /// <summary>
        /// Releases all resources reserved by the <see cref="ViewObservableDynamicArray{TIn, TOut}" />.
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
            if (!this.IsDisposed)
            {
                if (disposing)
                {
                }

                this.mapping.Dispose();
                this.viewModels.Dispose();
                this.semaphore.Dispose();

                this.CollectionChanged = null;
                this.sourceBacking = null;
                this.mapping = null;
                this.viewModels = null;

                this.isDisposedBacking = true;
            }
        }

        /// <summary>
        /// Raises the <see cref="CollectionChanged" /> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for the event handlers.
        /// </param>
        protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e) => this.CollectionChanged?.Invoke(this, e);

        /// <summary>
        /// Handles the <see cref="INotifyCollectionChanged.CollectionChanged" /> event of <see cref="Source" />.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the event.
        /// </param>
        /// <param name="e">
        ///     Contains additional information for the event handler.
        /// </param>
        private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.semaphore.Wait();

            NotifyCollectionChangedEventArgs args;

            try
            {
                IList<TOut> newItems = null;
                IList<TOut> oldItems = null;

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        newItems = this.AddItems(e.NewItems.Cast<TIn>());
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, e.NewStartingIndex);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        oldItems = this.RemoveItems(e.OldItems.Cast<TIn>());
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems, e.OldStartingIndex);
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        var itemReplacements = from o in e.OldItems.Cast<TIn>()
                                               from n in e.NewItems.Cast<TIn>()
                                               select new Tuple<TIn, TIn>(o, n);
                        this.ReplaceItems(itemReplacements, out oldItems, out newItems);
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItems, newItems, e.OldStartingIndex);
                        break;
                        
                    case NotifyCollectionChangedAction.Move:
                        var movedItems = e.OldItems.Cast<TIn>();

                        this.MoveItems(movedItems, out newItems);
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, oldItems, e.OldStartingIndex, e.NewStartingIndex);
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        oldItems = this.RemoveItems(from i in this select i.Model);
                        args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                        break;

                    default: throw new InvalidOperationException();
                }
                
            }
            finally
            {
                // leave lock in each case
                this.semaphore.Release();
            }

            this.RaiseCollectionChanged(args);
        }

        /// <summary>
        /// Adds the items in the specified <see cref="IEnumerable{T}" />.
        /// </summary>
        /// <param name="items">
        ///     The items that shall be added.
        /// </param>
        /// <returns>
        ///     Returns the <typeparamref name="TOut" /> representations of the items in <paramref name="items" />.
        /// </returns>
        private IList<TOut> AddItems(IEnumerable<TIn> items)
        {
            var ret = new List<TOut>();
            foreach (var item in items)
            {
                TOut newItem = this.ViewModelGeneratorCallback(item);
                this.mapping.Add(item, newItem);
                this.viewModels.Insert(this.sourceBacking.IndexOf(item), newItem);
                ret.Add(newItem);
            }

            return ret;
        }

        /// <summary>
        /// Removes the items in the specified <see cref="IEnumerable{T}" /> from the <see cref="ViewObservableDynamicArray{TIn, TOut}" />.
        /// </summary>
        /// <param name="items">
        ///     The items that shall be removed.
        /// </param>
        /// <returns>
        ///     Returns the <typeparamref name="TOut" /> representations of the removed items.
        /// </returns>
        private IList<TOut> RemoveItems(IEnumerable<TIn> items)
        {
            var ret = new List<TOut>();
            foreach (var item in items)
            {
                TOut oldItem = this.mapping[item];
                this.mapping.Remove(item);
                this.viewModels.Remove(oldItem);
                ret.Add(oldItem);
            }

            return ret;
        }

        /// <summary>
        /// Replaces the specified items.
        /// </summary>
        /// <param name="itemReplacements">
        ///     The <typeparamref name="TIn" /> replacement tuples.
        /// </param>
        /// <param name="removedItems">
        ///     The list containing the removed <typeparamref name="TOut" /> values.
        /// </param>
        /// <param name="addedItems">
        ///     The list containing the added <typeparamref name="TOut" /> values.
        /// </param>
        private void ReplaceItems(IEnumerable<Tuple<TIn, TIn>> itemReplacements, out IList<TOut> removedItems, out IList<TOut> addedItems)
        {
            removedItems = new DynamicArray<TOut>();
            addedItems = new DynamicArray<TOut>();

            foreach (var tuple in itemReplacements)
            {
                var newItem = this.ViewModelGeneratorCallback(tuple.Item1);

                var sourceIndex = this.sourceBacking.IndexOf(tuple.Item1);
                this.viewModels[sourceIndex] = newItem;

                removedItems.Add(this.mapping[tuple.Item1]);
                addedItems.Add(newItem);

                this.mapping[tuple.Item1] = newItem;
            }
        }


        private void MoveItems(IEnumerable<TIn> items, out IList<TOut> movedItems)
        {
            movedItems = new DynamicArray<TOut>();
            
            foreach (var item in items.ToArray())
            {
                var sourceIndex = this.Source.IndexOf(item);
                var viewmodel = this.mapping[item];
                this.viewModels.Remove(viewmodel);
                this.viewModels.Insert(sourceIndex, viewmodel);
                movedItems.Add(viewmodel);
            }
        }

        /// <summary>
        /// Provides an enumerator for the <see cref="ViewObservableDynamicArray{TIn, TOut}" />.
        /// </summary>
        /// <remarks>
        ///     Implements <see cref="IEnumerable{TOut}" />, <see cref="IEnumerable" />, <see cref="IDisposable" /> explicitly.
        /// </remarks>
        public struct Enumerator
            : IEnumerator<TOut>, IEnumerator, IDisposable
        {
            /// <summary>
            /// The <see cref="ViewObservableDynamicArray{TIn, TOut}" /> that shall be enumerated.
            /// </summary>
            private ViewObservableDynamicArray<TIn, TOut> enumerable;

            /// <summary>
            /// The current index of the <see cref="Enumerator" />.
            /// </summary>
            private int currentIndex;

            /// <summary>
            /// Initializes a new instance of the <see cref="ViewObservableDynamicArray{TIn, TOut}.Enumerator" /> struct.
            /// </summary>
            /// <param name="enumerable">
            ///     The <see cref="ViewObservableDynamicArray{TIn, TOut}" /> that shall be enumerated.
            /// </param>
            /// <exception cref="ArgumentNullException">
            ///     Thrown when <paramref name="enumerable" /> is <c>null</c>.
            /// </exception>
            public Enumerator(ViewObservableDynamicArray<TIn, TOut> enumerable)
            {
                // throw exception if enumerable is null
                if (enumerable == null)
                {
                    throw new ArgumentNullException(nameof(enumerable));
                }

                this.enumerable = enumerable;
                this.currentIndex = -1;
            }

            /// <summary>
            /// Gets the current item of the <see cref="Enumerator" />.
            /// </summary>
            /// <value>
            ///     Contains the current item of the <see cref="Enumerator" />.
            /// </value>
            /// <remarks>
            ///     Implements <see cref="IEnumerator{TOut}.Current" /> implicitly.
            /// </remarks>
            public TOut Current
            {
                get { return this.enumerable[this.currentIndex]; }
            }

            /// <summary>
            /// Gets the current item of the <see cref="IEnumerator" />.
            /// </summary>
            /// <value>
            ///     Contains the current item of the <see cref="IEnumerator" />.
            /// </value>
            /// <remarks>
            ///     Implements <see cref="IEnumerator.Current" /> explicitly.
            /// </remarks>
            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            /// <summary>
            /// Releases all resources reserved by the <see cref="Enumerator" />.
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
                // exit read lock if entered
                if (this.currentIndex >= 0 && this.currentIndex < this.enumerable.Count)
                {
                    this.enumerable.semaphore.Release();
                }

                this.currentIndex = 0;
                this.enumerable = null;
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Advances the <see cref="Enumerator" /> to the next item.
            /// </summary>
            /// <returns>
            ///     Returns a value indicating whether the <see cref="Enumerator" /> is inside of the <see cref="ViewObservableDynamicArray{TIn, TOut}" />.
            /// </returns>
            /// <remarks>
            ///     Implements <see cref="IEnumerator.MoveNext()" /> implicitly.
            /// </remarks>
            public bool MoveNext()
            {
                // enter read lock if not entered
                if (this.currentIndex < 0)
                {
                    this.enumerable.semaphore.Wait();
                }
                
                var ret = ++this.currentIndex < this.enumerable.Count;
                if (!ret)
                {
                    this.enumerable.semaphore.Release();
                }

                return ret;
            }

            /// <summary>
            /// Resets the <see cref="IEnumerator" />.
            /// </summary>
            /// <remarks>
            ///     Implements <see cref="IEnumerator.Reset()" /> implicitly.
            /// </remarks>
            public void Reset()
            {
                // exit read lock if entered
                if (this.currentIndex >= 0 && this.currentIndex < this.enumerable.Count)
                {
                    this.enumerable.semaphore.Release();
                }

                this.currentIndex = -1;
            }
        }
    }
}
