// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IProtocolFormatter.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IProtocolFormatter class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Provides an interface for serializing and de-serializing object graphs in a format.
    /// </summary>
    public interface IProtocolFormatter
    {
        /// <summary>
        /// Gets the supported protocol version.
        /// </summary>
        /// <value>
        ///     Contains the supported protocol version.
        /// </value>
        Version ProtocolVersion { get; }

        /// <summary>
        /// Serializes an <see cref="object" /> graph into a <see cref="Stream" />.
        /// </summary>
        /// <param name="target">
        ///     The target <see cref="Stream" /> for the serialization.
        /// </param>
        /// <param name="graph">
        ///     The <see cref="object" /> graph that shall be serialized.
        /// </param>
        /// <param name="surrogateSelectors">
        ///     The list of <see cref="ISurrogateSelector" /> instances for types that do not provide a serialization mechanism.
        /// </param>
        void Serialize(Stream target, object graph, IEnumerable<ISurrogateSelector> surrogateSelectors);

        /// <summary>
        /// Deserializes an <see cref="object" /> graph from a <see cref="Stream" />.
        /// </summary>
        /// <param name="source">
        ///     The source <see cref="Stream" /> for the deserialization.
        /// </param>
        /// <param name="surrogateSelectors">
        ///     The list of <see cref="ISurrogateSelector" /> instances for types that do not provide a serialization mechanism.
        /// </param>
        /// <returns>
        ///     Returns the deserialized <see cref="object" /> graph.
        /// </returns>
        object Deserialize(Stream source, IEnumerable<ISurrogateSelector> surrogateSelectors);
    }
}
