// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="HashGenerator.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the HashGenerator class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Collections
{
    using System;

    /// <summary>
    /// Provides a multiplication hash function for integer numbers.
    /// </summary>
    internal static class HashGenerator
    {
        /// <summary>
        /// The multiplied value used for spreading the values.
        /// </summary>
        internal static readonly double A = (Math.Sqrt(5) - 1) / 2;

        /// <summary>
        /// Generates the hash code for a specified value.
        /// </summary>
        /// <param name="value">
        ///     The value that shall be hashed.
        /// </param>
        /// <param name="max">
        ///     The exclusive upper limit of the hash code.
        /// </param>
        /// <returns>
        ///     Returns the generated hash code.
        /// </returns>
        internal static int GetHash(int value, int max) => (int)Math.Floor(max * ((value * A) % 1));
    }
}
