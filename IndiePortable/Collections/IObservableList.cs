// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IObservableList.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IObservableList&lt;T&gt; class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Collections
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    
    /// <summary>
    /// Provides an interface for generic typed lists that are observable.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the contained items.
    /// </typeparam>
    /// <seealso cref="INotifyCollectionChanged" />
    /// <seealso cref="IList{T}" />
    /// <seealso cref="IList" />
    public interface IObservableList<T>
        : INotifyCollectionChanged, INotifyPropertyChanged, IList<T>, IList
    {
        /// <summary>
        /// Gets the usage percent of the <see cref="IObservableList{T}" />.
        /// The range is between 0.0 and 1.0.
        /// </summary>
        /// <value>
        ///     Contains the usage percent of the <see cref="IObservableList{T}" />.
        /// </value>
        double UsagePercent { get; }
        
        /// <summary>
        /// Gets the capacity of the <see cref="IObservableList{T}" />.
        /// </summary>
        /// <value>
        ///     Contains the capacity of the <see cref="IObservableList{T}" />.
        /// </value>
        int Capacity { get; }
    }
}
