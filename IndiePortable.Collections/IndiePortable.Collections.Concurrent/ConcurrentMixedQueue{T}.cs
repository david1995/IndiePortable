// <copyright file="ConcurrentMixedQueue{T}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Collections.Concurrent
{
    using System.Collections.Generic;

    public class ConcurrentMixedQueue<T>
        : MixedQueue<T>
    {
        private readonly object concurrencyLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentMixedQueue{T}"/> class.
        /// </summary>
        public ConcurrentMixedQueue()
        {
        }

        public ConcurrentMixedQueue(int unitCapacity)
            : base(unitCapacity)
        {
        }

        public ConcurrentMixedQueue(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public ConcurrentMixedQueue(int unitCapacity, IEnumerable<T> collection)
            : base(unitCapacity, collection)
        {
        }

        protected override void EnqueueOverride(T item)
        {
            lock (this.concurrencyLock)
            {
                base.EnqueueOverride(item);
            }
        }

        protected override T DequeueOverride()
        {
            lock (this.concurrencyLock)
            {
                return base.DequeueOverride();
            }
        }

        protected override T PeekOverride()
        {
            lock (this.concurrencyLock)
            {
                return base.PeekOverride();
            }
        }
    }
}
