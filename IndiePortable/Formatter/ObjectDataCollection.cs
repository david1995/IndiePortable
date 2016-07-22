// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectDataCollection.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ObjectDataCollection class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a data container for object data.
    /// </summary>
    /// <remarks>
    ///     <para>Implements <see cref="IEnumerable{FieldData}" />, <see cref="IEquatable{ObjectData}" /> explicitly.</para>
    /// </remarks>
    public class ObjectDataCollection
        : IEnumerable<FieldData>, IEquatable<ObjectDataCollection>
    {
        /// <summary>
        /// The backing storage for the <see cref="FieldData" /> values.
        /// </summary>
        private List<FieldData> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDataCollection" /> class.
        /// </summary>
        /// <param name="source">
        ///     The data source for the <see cref="ObjectDataCollection" />. Can be <c>null</c>.
        /// </param>
        /// <param name="id">
        ///     The identified of the <see cref="ObjectDataCollection" /> in the current context.
        /// </param>
        public ObjectDataCollection(object source, int id)
        {
            this.ClrType = source?.GetType();
            this.values = new List<FieldData>();
            this.Source = source;
            this.Id = id;
        }

        /// <summary>
        /// Gets the <see cref="object" /> that serves as the data source for the <see cref="ObjectDataCollection" />.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="object" /> that serves as the data source for the <see cref="ObjectDataCollection" />.
        /// </value>
        public object Source { get; private set; }

        /// <summary>
        /// Gets the identifier of the <see cref="ObjectDataCollection" /> in the current context.
        /// </summary>
        /// <value>
        ///     Contains the identified of the <see cref="ObjectDataCollection" /> in the current context.
        /// </value>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the CLR type of the represented object.
        /// </summary>
        /// <value>
        ///     Contains the CLR type of the represented object.
        /// </value>
        public Type ClrType { get; private set; }

        /// <summary>
        /// Determines whether two <see cref="ObjectDataCollection" /> instances are considered equal.
        /// </summary>
        /// <param name="value1">
        ///     The first <see cref="ObjectDataCollection" /> to compare.
        /// </param>
        /// <param name="value2">
        ///     The second <see cref="ObjectDataCollection" /> to compare.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the two <see cref="ObjectDataCollection" /> instances are considered equal.
        /// </returns>
        public static bool operator ==(ObjectDataCollection value1, ObjectDataCollection value2) => Equals(value1, value2);

        /// <summary>
        /// Determines whether two <see cref="ObjectDataCollection" /> instances are not considered equal.
        /// </summary>
        /// <param name="value1">
        ///     The first <see cref="ObjectDataCollection" /> to compare.
        /// </param>
        /// <param name="value2">
        ///     The second <see cref="ObjectDataCollection" /> to compare.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the two <see cref="ObjectDataCollection" /> instances are not considered equal.
        /// </returns>
        public static bool operator !=(ObjectDataCollection value1, ObjectDataCollection value2) => !Equals(value1, value2);
        
        /// <summary>
        /// Determines whether two <see cref="ObjectDataCollection" /> instances are considered equal.
        /// </summary>
        /// <param name="value1">
        ///     The first <see cref="ObjectDataCollection" /> to compare.
        /// </param>
        /// <param name="value2">
        ///     The second <see cref="ObjectDataCollection" /> to compare.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the two <see cref="ObjectDataCollection" /> instances are considered equal.
        /// </returns>
        public static bool Equals(ObjectDataCollection value1, ObjectDataCollection value2) => value1?.Source == value2?.Source;

        /// <summary>
        /// Appends a value associated with a key and version information to the <see cref="ObjectDataCollection" />.
        /// </summary>
        /// <param name="key">
        ///     The key <paramref name="value" /> is associated with.
        /// </param>
        /// <param name="value">
        ///     The value that is associated with <paramref name="key" />.
        /// </param>
        /// <param name="minVersion">
        ///     The minimum version of the value. This parameter is optional.
        /// </param>
        /// <param name="maxVersion">
        ///     The maximum version of the value. This parameter is optional.
        /// </param>
        public void AddValue(string key, object value, Version minVersion = null, Version maxVersion = null)
                    => this.values.Add(new FieldData(key, value, minVersion, maxVersion));

        /// <summary>
        /// Determines whether the <see cref="ObjectDataCollection" /> contains the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key that shall be searched for.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the <see cref="ObjectDataCollection" /> contains <paramref name="key" />.
        /// </returns>
        public bool ContainsKey(string key) => this.values.Any(v => v.Key == key);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key to which the value shall be returned.
        /// </param>
        /// <returns>
        ///     Returns the value associated with the specified key.
        /// </returns>
        public object GetValue(string key) => this.values.First(v => v.Key == key)?.Value;

        /// <summary>
        /// Retrieves the value associated with a key.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value that shall be retrieved.
        /// </typeparam>
        /// <param name="key">
        ///     The key of the value that shall be searched for.
        /// </param>
        /// <returns>
        ///     Returns the value associated with <paramref name="key" /> casted to <typeparamref name="T" />.
        /// </returns>
        /// <exception cref="InvalidCastException">
        ///     Thrown when <paramref name="key" /> could be found, but the value is not of type <typeparamref name="T" />.
        /// </exception>
        public T GetValue<T>(string key) => (T)this.values.First(v => v.Key == key)?.Value;

        /// <summary>
        /// Retrieves the value associated with a key, if the specified key exists.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value that shall be retrieved.
        /// </typeparam>
        /// <param name="key">
        ///     The key of the value that shall be retrieved.
        /// </param>
        /// <param name="value">
        ///     The resulting value associated with <paramref name="key" />.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether <paramref name="key" /> could be found.
        /// </returns>
        public bool TryGetValue<T>(string key, out T value)
        {
            var contains = this.ContainsKey(key);
            value = contains ? this.GetValue<T>(key) : default(T);
            return contains;
        }

        /// <summary>
        /// Determines whether the current <see cref="ObjectDataCollection" /> instance and another one are considered equal.
        /// </summary>
        /// <param name="other">
        ///     The <see cref="ObjectDataCollection" /> instance that shall be compared to the current instance.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the two <see cref="ObjectDataCollection" /> instances are considered equal.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="IEquatable{ObjectData}.Equals(ObjectData)" /> implicitly.</para>
        /// </remarks>
        public bool Equals(ObjectDataCollection other) => Equals(this, other);

        /// <summary>
        /// Determines whether the current <see cref="ObjectDataCollection" /> instance and a specified <see cref="object" /> are considered equal.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="object" /> that shall be compared to the current instance.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the current <see cref="ObjectDataCollection" /> and the specified <see cref="object" /> are considered equal.
        /// </returns>
        /// <remarks>
        ///     <para>Overrides <see cref="object.Equals(object)" />.</para>
        /// </remarks>
        public override bool Equals(object obj) => Equals(this, obj as ObjectDataCollection);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// <remarks>
        ///     <para>Overrides <see cref="object.GetHashCode()" />.</para>
        /// </remarks>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Gets an enumerator for the <see cref="ObjectDataCollection" />.
        /// </summary>
        /// <returns>
        ///     Returns an enumerator for the <see cref="ObjectDataCollection" />.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="IEnumerable{ObjectData}.GetEnumerator()" /> implicitly.</para>
        /// </remarks>
        public IEnumerator<FieldData> GetEnumerator() => new Enumerator(this);

        /// <summary>
        /// Gets an <see cref="IEnumerator" /> for the <see cref="IEnumerable" />.
        /// </summary>
        /// <returns>
        ///     Returns an <see cref="IEnumerator" /> for the <see cref="IEnumerable" />.
        /// </returns>
        /// <remarks>
        ///     <para>Implements <see cref="IEnumerable.GetEnumerator()" /> explicitly.</para>
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Provides an enumerator for an <see cref="ObjectDataCollection" /> instance.
        /// </summary>
        /// <remarks>
        ///     <para>Implements <see cref="IEnumerator{FieldData}" />, <see cref="IEnumerator" />, <see cref="IDisposable" /> explicitly.</para>
        /// </remarks>
        private struct Enumerator
            : IEnumerator<FieldData>, IEnumerator, IDisposable
        {
            /// <summary>
            /// The <see cref="ObjectDataCollection" /> that shall be enumerated.
            /// </summary>
            private ObjectDataCollection enumerable;

            /// <summary>
            /// The current position of the <see cref="Enumerator" />.
            /// </summary>
            private int currentIndex;

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator" /> struct.
            /// </summary>
            /// <param name="source">
            ///     The source <see cref="ObjectDataCollection" /> that shall be enumerated.
            /// </param>
            internal Enumerator(ObjectDataCollection source)
            {
                this.enumerable = source;
                this.currentIndex = -1;
            }

            /// <summary>
            /// Gets the current element of the <see cref="Enumerator" />.
            /// </summary>
            /// <value>
            ///     Contains the current element of the <see cref="Enumerator" />.
            /// </value>
            /// <remarks>
            ///     <para>Implements <see cref="IEnumerator{FieldData}" /> implicitly.</para>
            /// </remarks>
            public FieldData Current
            {
                get { return this.enumerable.values[this.currentIndex]; }
            }

            /// <summary>
            /// Gets the current element of the <see cref="IEnumerator" />.
            /// </summary>
            /// <value>
            ///     Contains the current element of the <see cref="IEnumerator" />.
            /// </value>
            /// <remarks>
            ///     <para>Implements <see cref="IEnumerator.Current" /> explicitly.</para>
            /// </remarks>
            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            /// <summary>
            /// Releases any resources reserved by the <see cref="Enumerator" />.
            /// </summary>
            /// <remarks>
            ///     <para>Implements <see cref="IDisposable.Dispose()" /> implicitly.</para>
            /// </remarks>
            [System.Diagnostics.CodeAnalysis.SuppressMessage(
                "Microsoft.Usage",
                "CA1816:CallGCSuppressFinalizeCorrectly",
                Justification = "False positive; it actually calls GC.SuppressFinalize on itself.")]
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Advances the <see cref="Enumerator" /> to the next element.
            /// </summary>
            /// <returns>
            ///     Returns a value indicating whether the <see cref="Enumerator" /> has enumerated all elements of the <see cref="ObjectDataCollection" />.
            /// </returns>
            /// <remarks>
            ///     <para>Implements <see cref="IEnumerator.MoveNext()" /> implicitly.</para>
            /// </remarks>
            public bool MoveNext() => ++this.currentIndex < this.enumerable.values.Count;

            /// <summary>
            /// Resets the <see cref="Enumerator" /> before the first element.
            /// </summary>
            /// <remarks>
            ///     <para>Implements <see cref="IEnumerator.Reset()" /> implicitly.</para>
            /// </remarks>
            public void Reset() => this.currentIndex = -1;

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources.
            /// </summary>
            /// <param name="managed">
            ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
            /// </param>
            private void Dispose(bool managed)
            {
                if (managed)
                {
                    this.enumerable = null;
                    this.currentIndex = 0;
                }
            }
        }
    }
}
