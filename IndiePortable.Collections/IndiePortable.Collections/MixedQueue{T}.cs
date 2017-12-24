// <copyright file="MixedQueue{T}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class MixedQueue<T>
        : IReadOnlyCollection<T>
    {
        private readonly LinkedList<MixedQueueEntry> entries = new LinkedList<MixedQueueEntry>();

        private readonly int unitCapacity;

        private (bool Updated, int Value) countCache;

        public MixedQueue()
            : this(16, Array.Empty<T>())
        {
        }

        public MixedQueue(int unitCapacity)
            : this(unitCapacity, Array.Empty<T>())
        {
        }

        public MixedQueue(IEnumerable<T> collection)
            : this(16, collection)
        {
        }

        public MixedQueue(int unitCapacity, IEnumerable<T> collection)
        {
            this.unitCapacity = unitCapacity <= 0
                              ? throw new ArgumentOutOfRangeException(nameof(unitCapacity))
                              : unitCapacity;

            foreach (var c in collection ?? throw new ArgumentNullException(nameof(collection)))
            {
                this.Enqueue(c);
            }
        }

        public int Count
        {
            get
            {
                if (this.countCache.Updated)
                {
                    this.countCache = (false, this.entries.Aggregate(0, (acc, e) => acc + e.WritePointer - e.ReadPointer));
                }

                return this.countCache.Value;
            }
        }

        public void Enqueue(T item) => this.EnqueueOverride(item);

        public T Dequeue() => this.DequeueOverride();

        public T Peek() => this.PeekOverride();

        public IEnumerator<T> GetEnumerator() => this.EnumeratorGenerator().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        protected virtual void EnqueueOverride(T item)
        {
            var entry = this.entries.Last.Value;
            if (entry.WritePointer == this.unitCapacity)
            {
                entry = new MixedQueueEntry(this.unitCapacity);
                this.entries.AddLast(entry);
            }

            entry.Enqueue(item);
            this.countCache.Updated = true;
        }

        protected virtual T DequeueOverride()
        {
            var entry = this.entries.First.Value;

            var ret = entry.Dequeue();
            if (entry.ReadPointer == entry.WritePointer && entry.WritePointer == this.unitCapacity)
            {
                this.entries.Remove(entry);
            }

            this.countCache.Updated = true;
            return ret;
        }

        protected virtual T PeekOverride() => this.entries.First.Value.Peek();

        private IEnumerable<T> EnumeratorGenerator()
        {
            foreach (var e in this.entries)
            {
                foreach (var t in e)
                {
                    yield return t;
                }
            }
        }

        public class MixedQueueEntry
            : IEnumerable<T>
        {
            private readonly T[] entries;

            public MixedQueueEntry(int capacity)
            {
                this.entries = capacity <= 0
                             ? throw new ArgumentOutOfRangeException(nameof(capacity))
                             : new T[capacity];
            }

            public int ReadPointer { get; private set; } = 0;

            public int WritePointer { get; private set; } = 0;

            public void Enqueue(T item)
            {
                if (this.WritePointer == this.entries.Length)
                {
                    throw new InvalidOperationException("Entry is full.");
                }

                this.entries[this.WritePointer++] = item;
            }

            public T Dequeue()
            {
                if (this.ReadPointer == this.WritePointer)
                {
                    throw new InvalidOperationException("Entry is empty.");
                }

                return this.entries[this.ReadPointer++];
            }

            public T Peek()
            {
                if (this.ReadPointer == this.WritePointer)
                {
                    throw new InvalidOperationException("Entry is empty.");
                }

                return this.entries[this.ReadPointer];
            }

            public IEnumerator<T> GetEnumerator() => this.EnumeratorGenerator().GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            private IEnumerable<T> EnumeratorGenerator()
            {
                foreach (var t in this.entries.Skip(this.ReadPointer).Take(this.WritePointer - this.ReadPointer))
                {
                    yield return t;
                }
            }
        }
    }
}
