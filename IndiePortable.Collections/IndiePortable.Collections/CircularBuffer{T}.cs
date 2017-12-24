// <copyright file="CircularBuffer{T}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class CircularBuffer<T>
        : IReadOnlyCollection<T>
    {
        private Queue<T> buffer;

        public CircularBuffer(int length)
        {
            this.Length = length <= 0
                        ? throw new ArgumentOutOfRangeException(nameof(length))
                        : length;

            this.buffer = new Queue<T>(length);
        }

        public int Length { get; }

        public int Count => this.buffer.Count;

        public void Add(T item)
        {
            if (this.buffer.Count == this.Length)
            {
                throw new InvalidOperationException("The buffer is full.");
            }

            this.buffer.Enqueue(item);
        }

        public T Read()
        {
            if (this.buffer.Count == 0)
            {
                throw new InvalidOperationException("The buffer is empty.");
            }

            return this.buffer.Dequeue();
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => this.buffer.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
