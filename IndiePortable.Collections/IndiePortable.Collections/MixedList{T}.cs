// <copyright file="MixedList{T}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class MixedList<T>
        : IList<T>, IReadOnlyList<T>
    {
        private readonly int unitCapacity;

        private readonly LinkedList<MixedListEntry> entries = new LinkedList<MixedListEntry>();

        private (bool Updated, int Value) countCache;

        public MixedList()
            : this(16, Array.Empty<T>())
        {
        }

        public MixedList(int unitCapacity)
            : this(unitCapacity, Array.Empty<T>())
        {
        }

        public MixedList(IEnumerable<T> collection)
            : this(16, collection)
        {
        }

        public MixedList(int unitCapacity, IEnumerable<T> collection)
        {
            this.unitCapacity = unitCapacity <= 0
                              ? throw new ArgumentOutOfRangeException(nameof(unitCapacity))
                              : unitCapacity;

            foreach (var c in collection)
            {
                this.Add(c);
            }
        }

        public T this[int index]
        {
            get => this.GetOverride(index);
            set => this.ReplaceOverride(index, value);
        }

        public int Count
            => (this.countCache = this.countCache.Updated
                                ? (false, this.entries.Aggregate(0, (acc, e) => acc + e.Count()))
                                : (false, this.countCache.Value)).Value;

        public bool IsReadOnly => false;

        public void Add(T item) => this.AddOverride(item);

        public void Clear() => this.entries.Clear();

        public bool Contains(T item) => this.entries.Any(e => e.Contains(item));

        public void CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

        public int IndexOf(T item) => throw new NotImplementedException();

        public void Insert(int index, T item) => this.InsertOverride(index, item);

        public bool Remove(T item)
        {
            var i = this.IndexOf(item);
            if (i >= 0)
            {
                this.RemoveAtOverride(this.IndexOf(item));
            }

            return i >= 0;
        }

        public void RemoveAt(int index) => this.RemoveAtOverride(index);

        public IEnumerator<T> GetEnumerator() => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        protected virtual void AddOverride(T item)
        {
            var entry = this.entries.Last.Value;
            if (entry.IsFull)
            {
                entry = new MixedListEntry(this.unitCapacity);
                this.entries.AddLast(entry);
            }

            entry.Add(item);
        }

        protected virtual void InsertOverride(int index, T item)
        {

        }

        protected virtual T GetOverride(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            throw new NotImplementedException();
        }

        protected virtual void ReplaceOverride(int index, T item)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            // get entry with index

            // replace item in entry
        }

        protected virtual void RemoveAtOverride(int index)
        {

        }

        public class MixedListEntry
            : IEnumerable<T>
        {
            private readonly int capacity;

            private readonly List<T> source;

            public MixedListEntry(int capacity)
            {
                this.capacity = capacity <= 0 ? throw new ArgumentOutOfRangeException(nameof(capacity)) : capacity;
                this.source = new List<T>(capacity);
            }

            public bool IsFull => this.source.Count == this.capacity;

            public void Add(T item)
            {

            }

            public void Insert(int index, T item)
            {

            }

            public void Remove(int index)
            {

            }

            public IEnumerator<T> GetEnumerator() => this.source.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }
    }
}
