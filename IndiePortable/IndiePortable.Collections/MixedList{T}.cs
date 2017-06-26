// <copyright file="MixedList{T}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class MixedList<T>
        : IList<T>
    {
        public const int DefaultChunkSize = 16;

        private readonly LinkedList<Chunk> chunks = new LinkedList<Chunk>();

        private bool isUpdated;

        private object updateLock = new object();

        private int countCache;

        public MixedList()
        {
            this.ChunkSize = DefaultChunkSize;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                var chunkNumber = index / this.ChunkSize;
                var chunkIndex = index % this.ChunkSize;

                var chunk = this.chunks.ElementAt(chunkNumber);
                return chunk.Items[chunkIndex];
            }

            set
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                var chunkNumber = index / this.ChunkSize;
                var chunkIndex = index % this.ChunkSize;

                var chunk = this.chunks.ElementAt(chunkNumber);
                chunk.Items[chunkIndex] = value;
            }
        }

        public int Count
        {
            get
            {
                if (this.isUpdated)
                {
                    this.countCache = this.chunks.Sum(n => n.Count);
                    this.isUpdated = false;
                }

                return this.countCache;
            }
        }

        public int ChunkSize { get; }

        public bool IsReadOnly => false;

        public void Add(T item)
            => this.Insert(this.Count, item);

        public void Clear()
        {
            lock (this.updateLock)
            {
                this.chunks.Clear();
                this.isUpdated = true;
            }
        }

        public bool Contains(T item) => throw new NotImplementedException();

        public void CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

        public int IndexOf(T item) => throw new NotImplementedException();

        public void Insert(int index, T item)
        {
            if (index < 0 || index > this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            throw new NotImplementedException();

            lock (this.updateLock)
            {
                var chunkNumber = index / this.ChunkSize;
                var chunkIndex = index % this.ChunkSize;

                var chunk = this.chunks.ElementAt(chunkNumber);

                this.isUpdated = true;
            }
        }

        public bool Remove(T item) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();

        public IEnumerator<T> GetEnumerator() => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        protected void MoveRight(int startIndex, int count, int amount)
        {
            throw new NotImplementedException();
            for (var n = startIndex + count - 1; n >= startIndex; n++)
            {
            }
        }

        [Serializable]
        private struct Chunk
        {
            public Chunk(int capacity)
            {
                this.Capacity = capacity <= 0
                              ? throw new ArgumentOutOfRangeException(nameof(capacity))
                              : capacity;

                this.Count = 0;
                this.Items = new T[capacity];
            }

            public int Capacity { get; }

            public int Count { get; }

            public T[] Items { get; }
        }
    }
}
