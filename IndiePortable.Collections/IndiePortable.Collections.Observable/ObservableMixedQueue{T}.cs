// <copyright file="ObservableMixedQueue{T}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Collections.Observable
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    public class ObservableMixedQueue<T>
        : Concurrent.ConcurrentMixedQueue<T>, INotifyCollectionChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableMixedQueue{T}"/> class.
        /// </summary>
        public ObservableMixedQueue()
        {
        }

        public ObservableMixedQueue(int unitCapacity)
            : base(unitCapacity)
        {
        }

        public ObservableMixedQueue(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public ObservableMixedQueue(int unitCapacity, IEnumerable<T> collection)
            : base(unitCapacity, collection)
        {
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void EnqueueOverride(T item)
        {
            base.EnqueueOverride(item);
            this.OnAdded(item);
        }

        protected override T DequeueOverride()
        {
            var ret = base.DequeueOverride();
            this.OnRemoved(ret);
            return ret;
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            => this.CollectionChanged?.Invoke(this, e ?? throw new ArgumentNullException(nameof(e)));

        protected virtual void OnAdded(T item) => this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));

        protected virtual void OnRemoved(T item) => this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
    }
}
