// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryFormatter.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the BinaryFormatter class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides methods for serializing and de-serializing object graphs into a binary format.
    /// </summary>
    public class BinaryFormatter
    {
        /// <summary>
        /// The maximum protocol <see cref="Version" /> supported by the <see cref="BinaryFormatter" />.
        /// </summary>
        public static readonly Version ProtocolVersion = new Version(1, 0, 0, 0);

        /// <summary>
        /// The list of supported formatters.
        /// </summary>
        public static readonly IReadOnlyCollection<IProtocolFormatter> SupportedFormatters = new ReadOnlyCollection<IProtocolFormatter>(
            new[]
            {
                new Protocol1_0_0_0.ProtocolFormatter()
            });

        /// <summary>
        /// The backing field for the <see cref="SurrogateSelectors" /> property.
        /// </summary>
        private List<ISurrogateSelector> surrogateSelectorsBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFormatter" /> class.
        /// </summary>
        public BinaryFormatter()
        {
            this.surrogateSelectorsBacking = new List<ISurrogateSelector>();
            this.UsedProtocolVersion = SupportedFormatters.Max(p => p.ProtocolVersion);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFormatter" /> class.
        /// </summary>
        /// <param name="surrogateSelectors">
        ///     The surrogate selectors that are initially added to the <see cref="BinaryFormatter" />.
        /// </param>
        public BinaryFormatter(IEnumerable<ISurrogateSelector> surrogateSelectors)
        {
            this.surrogateSelectorsBacking = new List<ISurrogateSelector>(surrogateSelectors);
            this.UsedProtocolVersion = SupportedFormatters.Max(p => p.ProtocolVersion);
        }

        /// <summary>
        /// Gets the list containing the surrogate selectors of the <see cref="BinaryFormatter" />.
        /// </summary>
        /// <value>
        ///     Contains the list containing the surrogate selectors of the <see cref="BinaryFormatter" />.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         At (de-)serialization time, the list is enumerated and searched for the first
        ///         <see cref="ISurrogate" /> that fits the requested type.
        ///     </para>
        /// </remarks>
        public IList<ISurrogateSelector> SurrogateSelectors => this.surrogateSelectorsBacking;

        /// <summary>
        /// Gets or sets the protocol version used by the <see cref="BinaryFormatter" />.
        /// </summary>
        /// <value>
        ///     Contains the protocol version used by the <see cref="BinaryFormatter" />.
        /// </value>
        public Version UsedProtocolVersion { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFormatter" /> class, including surrogates for the core library.
        /// </summary>
        /// <returns>
        ///     The initialized <see cref="BinaryFormatter" />.
        /// </returns>
        public static BinaryFormatter CreateWithCoreSurrogates()
            => new BinaryFormatter(new[] { new MscorlibSurrogates.MscorlibSurrogateSelector() });

        /// <summary>
        /// If a binary value is in big-endian encoding, converts it to little-endian format.
        /// </summary>
        /// <param name="value">
        ///     The byte representation that shall be converted.
        /// </param>
        public static void MakeLittleEndian(ref byte[] value) => value = BitConverter.IsLittleEndian ? value : value.Reverse().ToArray();

        /// <summary>
        /// Gets the length-prefixed UTF-8 string byte representation of a string.
        /// </summary>
        /// <param name="value">
        ///     The string that shall be converted.
        /// </param>
        /// <returns>
        ///     Returns the byte representation of <paramref name="value" /> as a length-prefixed string.
        /// </returns>
        public static byte[] GetLengthPrefixedString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new byte[4];
            }

            using (var memstr = new MemoryStream())
            {
                var valueBytes = Encoding.UTF8.GetBytes(value);
                var lengthBytes = BitConverter.GetBytes(valueBytes.Length);
                MakeLittleEndian(ref lengthBytes);
                memstr.Write(lengthBytes, 0, lengthBytes.Length);
                memstr.Write(valueBytes, 0, valueBytes.Length);

                return memstr.ToArray();
            }
        }

        /// <summary>
        /// Gets the <see cref="string" /> represented in a <see cref="Stream" />.
        /// </summary>
        /// <param name="source">
        ///     The <see cref="Stream" /> containing the length-prefixed UTF-8 string.
        /// </param>
        /// <returns>
        ///     Returns the represented <see cref="string" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="source" /> is <c>null</c>.
        /// </exception>
        public static string GetString(Stream source)
        {
            // throw exception if source is null
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var dummy = 0;
            return GetString(source, ref dummy);
        }

        /// <summary>
        /// Gets the <see cref="string" /> represented in a <see cref="Stream" />.
        /// </summary>
        /// <param name="source">
        ///     The <see cref="Stream" /> containing the length-prefixed UTF-8 string.
        /// </param>
        /// <param name="currentPosition">
        ///     The current position incremented by the method. Can be set to a dummy variable.
        /// </param>
        /// <returns>
        ///     Returns the represented <see cref="string" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="source" /> is <c>null</c>.
        /// </exception>
        public static string GetString(Stream source, ref int currentPosition)
        {
            // throw exception if source is null
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var lengthBytes = new byte[sizeof(int)];
            ReadStream(source, lengthBytes, sizeof(int));
            MakeLittleEndian(ref lengthBytes);
            var length = BitConverter.ToInt32(lengthBytes, 0);
            currentPosition += sizeof(int);

            var contentBytes = new byte[length];
            ReadStream(source, contentBytes, length);
            currentPosition += length;
            return Encoding.UTF8.GetString(contentBytes, 0, length);
        }

        /// <summary>
        /// Reads the specified amount of bytes from a <see cref="Stream" />.
        /// </summary>
        /// <param name="source">
        ///     The source <see cref="Stream" />. Must support reading.
        /// </param>
        /// <param name="target">
        ///     The target byte array. Must have <paramref name="length" /> size.
        /// </param>
        /// <param name="length">
        ///     The amount of bytes that shall be read.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if:</para>
        ///     <para>  - <paramref name="source" /> is <c>null</c>.</para>
        ///     <para>  - <paramref name="target" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if the length of <paramref name="target" /> is not <paramref name="length" />.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if <paramref name="source" /> is not readable.</para>
        /// </exception>
        public static void ReadStream(Stream source, byte[] target, int length)
        {
            // throw exception if source is null
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            // throw exception if target is null
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            // throw exception if source cannot be read
            if (!source.CanRead)
            {
                throw new InvalidOperationException("The specified stream cannot be read.");
            }

            // throw exception if target size is not length
            if (target.Length != length)
            {
                throw new ArgumentException($"The length of {nameof(target)} must be {length} ({nameof(length)}).", nameof(target));
            }

            var currentLength = 0;
            while (currentLength < length)
            {
                currentLength += source.Read(target, currentLength, length - currentLength);
            }
        }

        /// <summary>
        /// Serializes an <see cref="object" /> graph into a <see cref="Stream" />.
        /// </summary>
        /// <param name="target">
        ///     The target <see cref="Stream" /> of the serialization.
        /// </param>
        /// <param name="graph">
        ///     The <see cref="object" /> graph that shall be serialized.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="target" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        ///     Thrown if an error occured during serialization.
        /// </exception>
        public void Serialize(Stream target, object graph)
        {
            // throw exception if target is null
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var exc = this.SafeSerialize(target, graph);

            // throw exception if exception occured.
            if (exc != null)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Deserializes an <see cref="object" /> graph from a <see cref="Stream" />.
        /// </summary>
        /// <param name="source">
        ///     The source <see cref="Stream" /> for the deserialization.
        /// </param>
        /// <returns>
        ///     Returns the deserialized <see cref="object" /> graph.
        /// </returns>
        public object Deserialize(Stream source)
        {
            object ret;
            var exc = this.SafeDeserialize(source, out ret);

            // throw exception if necessary
            if (exc != null)
            {
                throw exc;
            }

            return ret;
        }

        /// <summary>
        /// Deserializes an <see cref="object" /> graph from a <see cref="Stream" /> and casts the root to <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">
        ///     The <see cref="Type" /> to which the root shall be casted.
        /// </typeparam>
        /// <param name="source">
        ///     The source <see cref="Stream" /> for the deserialization.
        /// </param>
        /// <returns>
        ///     Returns the deserialized <see cref="object" /> graph.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="source" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        ///     Thrown if an error occurred during deserialization.
        /// </exception>
        public T Deserialize<T>(Stream source)
        {
            // throw exception if source is null
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            T ret;
            var exc = this.SafeDeserialize<T>(source, out ret);

            // throw exception if necessary
            if (exc != null)
            {
                throw exc;
            }

            return ret;
        }

        /// <summary>
        /// Tries to deserialize an <see cref="object" /> graph from a <see cref="Stream" />.
        /// </summary>
        /// <param name="source">
        ///     The source <see cref="Stream" /> for the deserialization.
        /// </param>
        /// <param name="result">
        ///     The resulting <see cref="object" /> graph.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the deserialization has been successful.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1007:UseGenericsWhereAppropriate",
            Justification = "A generic variant of the method is provided, but for deserialization it is often better to return an object.")]
        public bool TryDeserialize(Stream source, out object result) => this.SafeDeserialize(source, out result) == null;

        /// <summary>
        /// Tries to deserialize an <see cref="object" /> graph from a <see cref="Stream" /> and cast it to <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">
        ///     The target type of the deserialization.
        /// </typeparam>
        /// <param name="source">
        ///     The source <see cref="Stream" /> for the deserialization.
        /// </param>
        /// <param name="result">
        ///     The resulting <see cref="object" /> graph with a <typeparamref name="T" /> root object.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the deserialization has been successful.
        /// </returns>
        public bool TryDeserialize<T>(Stream source, out T result) => this.SafeDeserialize(source, out result) == null;

        /// <summary>
        /// Tries to serialize an <see cref="object" /> graph to a <see cref="Stream" />.
        /// </summary>
        /// <param name="target">
        ///     The target <see cref="Stream" /> for the serialization.
        /// </param>
        /// <param name="graph">
        ///     The <see cref="object" /> graph that shall be serialized.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the serialization has been successful.
        /// </returns>
        public bool TrySerialize(Stream target, object graph) => this.SafeSerialize(target, graph) == null;

        /// <summary>
        /// Safely deserializes an <see cref="object" /> graph from a <see cref="Stream" />.
        /// </summary>
        /// <param name="source">
        ///     The source <see cref="Stream" /> for the deserialization.
        /// </param>
        /// <param name="result">
        ///     The resulting <see cref="object" /> graph.
        /// </param>
        /// <returns>
        ///     If an error occurs during deserialization, returns the thrown <see cref="Exception" />; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="source" /> is null.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1007:UseGenericsWhereAppropriate",
            Justification = "A generic variant of the method is provided, but for deserialization it is often better to return an object.")]
        protected Exception SafeDeserialize(Stream source, out object result)
        {
            // throw exception if source is null
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            try
            {
                var protFormatter = SupportedFormatters.FirstOrDefault(f => f.ProtocolVersion == this.UsedProtocolVersion);
                if (protFormatter == null)
                {
                    throw new InvalidOperationException($"The specified protocol version {this.UsedProtocolVersion} is not supported.");
                }

                result = protFormatter.Deserialize(source, this.SurrogateSelectors);
                return null;
            }
            catch (Exception exc)
            {
                // if an unhandled exception occurs
                result = null;
                return exc;
            }
        }

        /// <summary>
        /// Safely deserializes an <see cref="object" /> and casts it to <typeparamref name="T" /> from a <see cref="Stream" />.
        /// </summary>
        /// <typeparam name="T">
        ///     The type to which the deserialized <see cref="object" /> shall be casted.
        /// </typeparam>
        /// <param name="source">
        ///     The source <see cref="Stream" /> for the deserialization.
        /// </param>
        /// <param name="result">
        ///     The read <typeparamref name="T" /> instance.
        /// </param>
        /// <returns>
        ///     If an error occurs during deserialization, returns the thrown <see cref="Exception" />; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="source" /> is <c>null</c>.
        /// </exception>
        protected Exception SafeDeserialize<T>(Stream source, out T result)
        {
            // throw exception if source is null
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            try
            {
                var protFormatter = SupportedFormatters.FirstOrDefault(f => f.ProtocolVersion == this.UsedProtocolVersion);
                if (protFormatter == null)
                {
                    throw new InvalidOperationException($"The specified protocol version {this.UsedProtocolVersion} is not supported.");
                }

                result = (T)protFormatter.Deserialize(source, this.SurrogateSelectors);
                return null;
            }
            catch (Exception exc)
            {
                // if an unhandled exception occurs
                result = default(T);
                return exc;
            }
        }

        /// <summary>
        /// Safely serializes an <see cref="object" /> into a <see cref="Stream" />.
        /// </summary>
        /// <param name="target">
        ///     The target <see cref="Stream" /> of the serialization.
        /// </param>
        /// <param name="graph">
        ///     The <see cref="object" /> graph that shall be serialized. Can be <c>null</c>.
        /// </param>
        /// <returns>
        ///     If an error occurs during serialization, returns the thrown <see cref="Exception" />; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="target" /> is <c>null</c>.
        /// </exception>
        protected Exception SafeSerialize(Stream target, object graph)
        {
            // throw exception if target is null
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            try
            {
                var protFormatter = SupportedFormatters.FirstOrDefault(f => f.ProtocolVersion == this.UsedProtocolVersion);
                if (protFormatter == null)
                {
                    throw new InvalidOperationException($"The specified protocol version {this.UsedProtocolVersion} is not supported.");
                }

                protFormatter.Serialize(target, graph, this.SurrogateSelectors);
                return null;
            }
            catch (Exception exc)
            {
                // if an unhandled exception occurs
                return exc;
            }
        }
    }
}
