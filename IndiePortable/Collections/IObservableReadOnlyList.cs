// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IObservableReadOnlyList.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IObservableReadOnlyList&lt;T&gt; class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Collections
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// Provides an interface for generic typed read-only lists that are observable.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the items stored in the <see cref="IObservableReadOnlyList{T}" />.
    /// </typeparam>
    /// <seealso cref="INotifyCollectionChanged" />
    /// <seealso cref="IReadOnlyList{T}" />
    /// <seealso cref="IList"/>
    public interface IObservableReadOnlyList<T>
        : INotifyCollectionChanged, IReadOnlyList<T>, IList
    {
        /// <summary>
        /// Gets the usage percent of the <see cref="IObservableReadOnlyList{T}" />.
        /// The range is between 0.0 and 1.0.
        /// </summary>
        /// <value>
        ///     Contains the usage percent of the <see cref="IObservableReadOnlyList{T}" />.
        /// </value>
        double UsagePercent { get; }

        /// <summary>
        /// Gets the capacity of the <see cref="IObservableReadOnlyList{T}" />.
        /// </summary>
        /// <value>
        ///     Contains the capacity of the <see cref="IObservableReadOnlyList{T}" />.
        /// </value>
        int Capacity { get; }
    }
}
