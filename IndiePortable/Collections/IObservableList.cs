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
    }
}
