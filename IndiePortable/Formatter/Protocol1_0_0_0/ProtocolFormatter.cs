// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtocolFormatter.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ProtocolFormatter class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Protocol1_0_0_0
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides a binary <see cref="IProtocolFormatter" /> for the protocol version 1.0.0.0.
    /// </summary>
    /// <remarks>
    ///     Implements <see cref="IProtocolFormatter" /> explicitly.
    /// </remarks>
    internal class ProtocolFormatter
        : IProtocolFormatter, IDisposable
    {
        /// <summary>
        /// The protocol version used by the <see cref="ProtocolFormatter" />.
        /// </summary>
        internal static readonly Version UsedProtocolVersion = new Version(1, 0, 0, 0);

        /// <summary>
        /// Provides the introduction bytes for protocol data.
        /// </summary>
        internal static readonly byte[] IntroductionBytes = { 0x00, 0x00, 0xff, 0xff };

        /// <summary>
        /// Contains the local <see cref="GraphIterator" /> instance.
        /// </summary>
        private GraphIterator localIterator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolFormatter" /> class.
        /// </summary>
        public ProtocolFormatter()
        {
            this.localIterator = new GraphIterator();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ProtocolFormatter" /> class.
        /// </summary>
        ~ProtocolFormatter()
        {
            this.Dispose();
        }

        /// <summary>
        /// Gets the protocol version.
        /// </summary>
        /// <value>
        /// The protocol version.
        /// </value>
        /// <remarks>
        ///     Implements <see cref="IProtocolFormatter.ProtocolVersion" /> implicitly.
        /// </remarks>
        public Version ProtocolVersion
        {
            get { return UsedProtocolVersion; }
        }

        /// <summary>
        /// Deserializes an <see cref="object" /> graph from a <see cref="Stream" />.
        /// </summary>
        /// <param name="source">
        ///     The source <see cref="Stream" /> for the deserialization.
        /// </param>
        /// <param name="surrogateSelectors">
        ///     The list of surrogate selectors providing <see cref="ISurrogate" />
        ///     instances for types that do not provide serialization mechanisms.
        /// </param>
        /// <returns>
        ///     Returns the deserialized <see cref="object" /> graph.
        /// </returns>
        /// <exception cref="SerializationException">
        ///     Thrown when the message format is invalid.
        /// </exception>
        /// <remarks>
        ///     Implements <see cref="IProtocolFormatter.Deserialize(Stream, IEnumerable{ISurrogateSelector})" /> implicitly.
        /// </remarks>
        public object Deserialize(Stream source, IEnumerable<ISurrogateSelector> surrogateSelectors)
        {
            uint intro, packageId, partId, partCount;
            int length;
            int major, minor, build, revision;
            Version protocolVersion;

            var introBytes = new byte[sizeof(uint)];
            var lengthBytes = new byte[sizeof(int)];
            var packageIdBytes = new byte[sizeof(uint)];
            var partIdBytes = new byte[sizeof(uint)];
            var partCountBytes = new byte[sizeof(uint)];

            var majorBytes = new byte[sizeof(int)];
            var minorBytes = new byte[sizeof(int)];
            var buildBytes = new byte[sizeof(int)];
            var revisionBytes = new byte[sizeof(int)];

            // read intro
            BinaryFormatter.ReadStream(source, introBytes, sizeof(uint));
            BinaryFormatter.MakeLittleEndian(ref introBytes);
            intro = BitConverter.ToUInt32(introBytes, 0);
            if (intro != 0xffff0000)
            {
                throw new SerializationException("Invalid package intro.");
            }

            // read length
            BinaryFormatter.ReadStream(source, lengthBytes, sizeof(int));
            BinaryFormatter.MakeLittleEndian(ref lengthBytes);
            length = BitConverter.ToInt32(lengthBytes, 0);
            if (length < 0)
            {
                throw new SerializationException($"Invalid package length: {length}.");
            }

            // read package id
            BinaryFormatter.ReadStream(source, packageIdBytes, sizeof(uint));
            BinaryFormatter.MakeLittleEndian(ref packageIdBytes);
            packageId = BitConverter.ToUInt32(packageIdBytes, 0);

            // read part Id
            BinaryFormatter.ReadStream(source, partIdBytes, sizeof(uint));
            BinaryFormatter.MakeLittleEndian(ref partIdBytes);
            partId = BitConverter.ToUInt32(partIdBytes, 0);

            // read part count
            BinaryFormatter.ReadStream(source, partCountBytes, sizeof(uint));
            BinaryFormatter.MakeLittleEndian(ref partCountBytes);
            partCount = BitConverter.ToUInt32(partCountBytes, 0);
            if (partId >= partCount)
            {
                throw new SerializationException("Invalid part ID: part ID must be smaller than total amount of parts.");
            }

            // read major version
            BinaryFormatter.ReadStream(source, majorBytes, sizeof(int));
            BinaryFormatter.MakeLittleEndian(ref majorBytes);
            major = BitConverter.ToInt32(majorBytes, 0);

            // read minor version
            BinaryFormatter.ReadStream(source, minorBytes, sizeof(int));
            BinaryFormatter.MakeLittleEndian(ref minorBytes);
            minor = BitConverter.ToInt32(minorBytes, 0);

            // read build version
            BinaryFormatter.ReadStream(source, buildBytes, sizeof(int));
            BinaryFormatter.MakeLittleEndian(ref buildBytes);
            build = BitConverter.ToInt32(buildBytes, 0);

            // read revision version
            BinaryFormatter.ReadStream(source, revisionBytes, sizeof(int));
            BinaryFormatter.MakeLittleEndian(ref revisionBytes);
            revision = BitConverter.ToInt32(revisionBytes, 0);

            // get version
            protocolVersion = new Version(major, minor, build, revision);
            if (protocolVersion != this.ProtocolVersion)
            {
                throw new ProtocolVersionNotSupportedException(
                    $"The received protocol version is not supported by the current {nameof(IProtocolFormatter)}.",
                    protocolVersion);
            }

            var binaryObjects = new List<BinaryObjectRepresentation>();

            // read objects
            var currentLength = 0;
            while (currentLength < length)
            {
                int objectId;
                string clrType;
                int objectLength;

                var objectIdBytes = new byte[sizeof(uint)];
                var objectTypeBytes = new byte[sizeof(uint)];
                var objectLengthBytes = new byte[sizeof(int)];

                // read object Id
                BinaryFormatter.ReadStream(source, objectIdBytes, sizeof(uint));
                BinaryFormatter.MakeLittleEndian(ref objectIdBytes);
                objectId = BitConverter.ToInt32(objectIdBytes, 0);
                currentLength += sizeof(uint);

                // read object type
                BinaryFormatter.ReadStream(source, objectTypeBytes, sizeof(uint));
                currentLength += sizeof(uint);

                // read clr type
                clrType = BinaryFormatter.GetString(source, ref currentLength);

                // read object length
                BinaryFormatter.ReadStream(source, objectLengthBytes, sizeof(int));
                BinaryFormatter.MakeLittleEndian(ref objectLengthBytes);
                objectLength = BitConverter.ToInt32(objectLengthBytes, 0);
                currentLength += sizeof(int);

                // read object body
                var objectBody = new byte[objectLength];
                BinaryFormatter.ReadStream(source, objectBody, objectLength);
                currentLength += objectLength;

                BinaryObjectRepresentation binaryObject = (ObjectType)objectTypeBytes[0] == ObjectType.Array
                                                        ? new BinaryArrayRepresentation(objectId, objectTypeBytes, clrType, objectBody)
                                                        : new BinaryObjectRepresentation(objectId, objectTypeBytes, clrType, objectBody);
                binaryObjects.Add(binaryObject);
            }

            // create objects (without constructing them)
            foreach (var binaryObject in binaryObjects)
            {
                switch (binaryObject.Type)
                {
                    case ObjectType.Null: break;
                    case ObjectType.Object:
                        ReadObjectFields(binaryObject);
                        binaryObject.Result = GenerateObject(binaryObject, binaryObjects, surrogateSelectors);
                        break;

                    case ObjectType.Primitive:
                        return GetPrimitive(binaryObject.ObjectBody, (PrimitiveType)binaryObject.FullType[1]);

                    case ObjectType.String:
                        using (var memstr = new MemoryStream(binaryObject.ObjectBody))
                        {
                            var dummylen = 0;
                            return BinaryFormatter.GetString(memstr, ref dummylen);
                        }

                    case ObjectType.Array:
                        ReadArray(binaryObject as BinaryArrayRepresentation);
                        break;

                    case ObjectType.Enum:
                        

                    default: throw new SerializationException(
                        $"The current {nameof(IProtocolFormatter)} cannot deserialize an object with the specified object type.");
                }
            }

            var structs = binaryObjects.Where(o =>
                o.Type == ObjectType.Object && (Type.GetType(o.ClrType)?.GetTypeInfo().IsValueType ?? false));
            var notstructs = binaryObjects.Where(n => !structs.Contains(n));

            // first initialize structs
            foreach (var obj in structs)
            {
                // then generate object data
                var retData = new SerializationInfo(Type.GetType(obj.ClrType), new GraphIterator.FormatterConverter());

                // populate object data
                foreach (var property in obj.Properties)
                {
                    switch ((ObjectType)property.Key.Item2[0])
                    {
                        case ObjectType.Null:
                            retData.AddValue(property.Key.Item1, null);
                            break;

                        case ObjectType.Array:
                        case ObjectType.Object:
                            var objId = BitConverter.ToUInt32(property.Value, 0);
                            var newRoot = binaryObjects.First(o => o.ObjectID == objId);
                            retData.AddValue(property.Key.Item1, GenerateObject(newRoot, binaryObjects, surrogateSelectors));
                            break;

                        case ObjectType.Primitive:
                            retData.AddValue(property.Key.Item1, GetPrimitive(property.Value, (PrimitiveType)property.Key.Item2[1]));
                            break;

                        case ObjectType.Enum:
                            retData.AddValue(property.Key.Item1, GetPrimitive(property.Value, (PrimitiveType)property.Key.Item2[1]));
                            break;

                        case ObjectType.String:
                            using (var memstr = new MemoryStream(property.Value))
                            {
                                var dummy = 0;
                                retData.AddValue(property.Key.Item1, BinaryFormatter.GetString(memstr, ref dummy));
                                break;
                            }

                        default:
                            throw new SerializationException(
                                $@"The deserialized property ""{property.Key.Item1}"" is of an object type that is not supported by the current {nameof(IProtocolFormatter)}.");
                    }
                }

                var type = Type.GetType(obj.ClrType);

                if (type != null)
                {
                    var typeInfo = type?.GetTypeInfo();
                    var serType = new SerializationTypeInfo(type);
                    var res = obj.Result;
                    obj.Result = ChooseDeserializationMethod(ref res, obj, type, typeInfo, serType, retData, surrogateSelectors);
                }
            }

            // after creating instances -> calling a constructor
            foreach (var obj in notstructs)
            {
                // then generate object data
                var retData = new SerializationInfo(Type.GetType(obj.ClrType), new GraphIterator.FormatterConverter());

                if ((ObjectType)obj.FullType[0] == ObjectType.Array)
                {
                    // populate object data
                    var binObject = obj as BinaryArrayRepresentation;
                    foreach (var elem in binObject.Elements)
                    {
                        object elemValue;
                        switch ((ObjectType)elem.ElementType[0])
                        {
                            case ObjectType.Null:
                                elemValue = null;
                                break;

                            case ObjectType.Array:
                            case ObjectType.Object:
                                var objId = BitConverter.ToUInt32(elem.BodyBytes, 0);
                                var newRoot = binaryObjects.First(o => o.ObjectID == objId);
                                elemValue = GenerateObject(newRoot, binaryObjects, surrogateSelectors);
                                break;

                            case ObjectType.Primitive:
                            case ObjectType.Enum:
                                elemValue = GetPrimitive(elem.BodyBytes, (PrimitiveType)elem.ElementType[1]);
                                break;

                            case ObjectType.String:
                                using (var memstr = new MemoryStream(elem.BodyBytes, false))
                                {
                                    var dummy = 0;
                                    elemValue = BinaryFormatter.GetString(memstr, ref dummy);
                                    break;
                                }

                            default:
                                throw new SerializationException(
                                    $@"The deserialized array element ""{elem.SerialIndex}"" is of an object type that is not supported by the current {nameof(IProtocolFormatter)}.");
                        }

                        var arrayElem = new ArrayElement(elem.Indices, elemValue);
                        retData.AddValue(elem.SerialIndex.ToString(), arrayElem);
                    }
                }
                else
                {
                    // populate object data
                    foreach (var property in obj.Properties)
                    {
                        switch ((ObjectType)property.Key.Item2[0])
                        {
                            case ObjectType.Null:
                                retData.AddValue(property.Key.Item1, null);
                                break;

                            case ObjectType.Array:
                            case ObjectType.Object:
                                var objId = BitConverter.ToUInt32(property.Value, 0);
                                var newRoot = binaryObjects.First(o => o.ObjectID == objId);
                                retData.AddValue(property.Key.Item1, GenerateObject(newRoot, binaryObjects, surrogateSelectors));
                                break;

                            case ObjectType.Primitive:
                                retData.AddValue(property.Key.Item1, GetPrimitive(property.Value, (PrimitiveType)property.Key.Item2[1]));
                                break;

                            case ObjectType.Enum:
                                retData.AddValue(property.Key.Item1, GetPrimitive(property.Value, (PrimitiveType)property.Key.Item2[1]));
                                break;

                            case ObjectType.String:
                                using (var memstr = new MemoryStream(property.Value))
                                {
                                    var dummy = 0;
                                    retData.AddValue(property.Key.Item1, BinaryFormatter.GetString(memstr, ref dummy));
                                    break;
                                }

                            default:
                                throw new SerializationException(
                                    $@"The deserialized property ""{property.Key.Item1}"" is of an object type that is not supported by the current {nameof(IProtocolFormatter)}.");
                        }
                    }
                }

                var type = Type.GetType(obj.ClrType);

                if (type != null)
                {
                    var typeInfo = type?.GetTypeInfo();
                    var serType = new SerializationTypeInfo(type);
                    var res = obj.Result;
                    obj.Result = ChooseDeserializationMethod(ref res, obj, type, typeInfo, serType, retData, surrogateSelectors);
                }
            }
            
            return binaryObjects.First(b => b.ObjectID == 0).Result;
        }

        /// <summary>
        /// Serializes an <see cref="object" /> graph to a specified <see cref="Stream" />.
        /// </summary>
        /// <param name="target">
        ///     The target <see cref="Stream" /> for the serialization.
        /// </param>
        /// <param name="graph">
        ///     The <see cref="object" /> that shall be serialized.
        /// </param>
        /// <param name="surrogateSelectors">
        ///     The list of <see cref="ISurrogateSelector" /> providing serialization surrogates.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when <paramref name="graph" /> represents an object type that cannot be serialized by the <see cref="ProtocolFormatter" />.
        /// </exception>
        /// <remarks>
        ///     Implements <see cref="IProtocolFormatter.Serialize(Stream, object, IEnumerable{ISurrogateSelector})" /> implicitly.
        /// </remarks>
        public void Serialize(Stream target, object graph, IEnumerable<ISurrogateSelector> surrogateSelectors)
        {
            // throw exception if target is null
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            // get graph data
            var data = this.localIterator.GetGraphData(graph, surrogateSelectors);

            byte[] body;
            using (var memstr = new MemoryStream())
            {
                // get all objects
                foreach (var kv in data)
                {
                    var objType = ObjectAnalyzer.GetPrimaryObjectType(kv.Value.Source);

                    // get content
                    byte[] content;
                    using (var objstr = new MemoryStream())
                    {
                        switch (objType)
                        {
                            case ObjectType.Object: // object -> write properties
                                SerializeObjectFields(
                                    objstr,
                                    kv.Value.Info,
                                    kv.Value.Source,
                                    data);
                                break;

                            case ObjectType.Primitive: // primitive value
                                dynamic primValue = kv.Value.Source;
                                byte[] primValueBytes = BitConverter.GetBytes(primValue);
                                objstr.Write(primValueBytes, 0, primValueBytes.Length);
                                break;

                            case ObjectType.String: // string value
                                var stringBytes = BinaryFormatter.GetLengthPrefixedString(kv.Value.Source as string);
                                objstr.Write(stringBytes, 0, stringBytes.Length);
                                break;

                            case ObjectType.Enum: // enum value
                                var primitiveType = (PrimitiveType)ObjectAnalyzer.GetObjectType(kv.Value.Source)[1];
                                byte[] enumBytes = GetEnumBytes(primitiveType, kv.Value.Source);
                                objstr.Write(enumBytes, 0, enumBytes.Length);
                                break;

                            case ObjectType.Array:
                                SerializeArray(
                                    objstr,
                                    kv.Value.Info,
                                    kv.Value.Source,
                                    data);
                                break;

                            case ObjectType.Null: break; // break when null
                            default: throw new InvalidOperationException($"The specified object type is not usable by the {nameof(ProtocolFormatter)}.");
                        }

                        content = objstr.ToArray();
                    }

                    // buffer object id
                    var objIdBytes = BitConverter.GetBytes(kv.Key);
                    BinaryFormatter.MakeLittleEndian(ref objIdBytes);
                    memstr.Write(objIdBytes, 0, objIdBytes.Length);

                    // buffer object type
                    var objTypeBytes = ObjectAnalyzer.GetObjectType(kv.Value.Source);
                    memstr.Write(objTypeBytes, 0, objTypeBytes.Length);

                    // buffer clr type name
                    var typeName = kv.Value.Info?.ObjectType?.AssemblyQualifiedName;
                    var typeNameBytes = BinaryFormatter.GetLengthPrefixedString(typeName);
                    memstr.Write(typeNameBytes, 0, typeNameBytes.Length);

                    // buffer content length
                    var contentLengthBytes = BitConverter.GetBytes(content.Length);
                    BinaryFormatter.MakeLittleEndian(ref contentLengthBytes);
                    memstr.Write(contentLengthBytes, 0, contentLengthBytes.Length);

                    // buffer content
                    memstr.Write(content, 0, content.Length);
                }

                body = memstr.ToArray();
            }

            // write intro
            target.Write(IntroductionBytes, 0, IntroductionBytes.Length);

            // write body length
            var bodyLengthBytes = BitConverter.GetBytes(body.Length);
            BinaryFormatter.MakeLittleEndian(ref bodyLengthBytes);
            target.Write(bodyLengthBytes, 0, bodyLengthBytes.Length);

            // write package ID
            uint packageId = 0;
            var packageIdBytes = BitConverter.GetBytes(packageId);
            BinaryFormatter.MakeLittleEndian(ref packageIdBytes);
            target.Write(packageIdBytes, 0, packageIdBytes.Length);

            // write part ID
            uint partId = 0;
            var partIdBytes = BitConverter.GetBytes(partId);
            BinaryFormatter.MakeLittleEndian(ref partIdBytes);
            target.Write(partIdBytes, 0, partIdBytes.Length);

            // write part count
            uint partCount = 1;
            var partCountBytes = BitConverter.GetBytes(partCount);
            BinaryFormatter.MakeLittleEndian(ref partCountBytes);
            target.Write(partCountBytes, 0, partCountBytes.Length);

            // write protocol version
            var majorBytes = BitConverter.GetBytes(this.ProtocolVersion.Major);
            BinaryFormatter.MakeLittleEndian(ref majorBytes);
            var minorBytes = BitConverter.GetBytes(this.ProtocolVersion.Minor);
            BinaryFormatter.MakeLittleEndian(ref minorBytes);
            var buildBytes = BitConverter.GetBytes(this.ProtocolVersion.Build);
            BinaryFormatter.MakeLittleEndian(ref buildBytes);
            var revisionBytes = BitConverter.GetBytes(this.ProtocolVersion.Revision);
            BinaryFormatter.MakeLittleEndian(ref revisionBytes);

            target.Write(majorBytes, 0, majorBytes.Length);
            target.Write(minorBytes, 0, minorBytes.Length);
            target.Write(buildBytes, 0, buildBytes.Length);
            target.Write(revisionBytes, 0, revisionBytes.Length);

            // write body
            target.Write(body, 0, body.Length);
        }

        /// <summary>
        /// Releases all resources reserved by the <see cref="ProtocolFormatter" />.
        /// </summary>
        /// <remarks>
        ///     Implements <see cref="IDisposable.Dispose()" /> implicitly.
        /// </remarks>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="managed">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected void Dispose(bool managed)
        {
            this.localIterator.Dispose();
            if (managed)
            {
                this.localIterator = null;
            }
        }

        /// <summary>
        /// Gets the primitive value from a byte array.
        /// </summary>
        /// <param name="source">
        ///     The byte array serving as data source.
        /// </param>
        /// <param name="type">
        ///     The primitive type that shall be extracted.
        /// </param>
        /// <returns>
        ///     Returns the specified primitive value.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="type" /> does not represent a primitive type supported by the <see cref="ProtocolFormatter" />.
        /// </exception>
        private static ValueType GetPrimitive(byte[] source, PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.Bool: return BitConverter.ToBoolean(source, 0);
                case PrimitiveType.SByte: return (sbyte)source[0];
                case PrimitiveType.Short: return BitConverter.ToInt16(source, 0);
                case PrimitiveType.Int: return BitConverter.ToInt32(source, 0);
                case PrimitiveType.Long: return BitConverter.ToInt64(source, 0);
                case PrimitiveType.Byte: return source[0];
                case PrimitiveType.UShort: return BitConverter.ToUInt16(source, 0);
                case PrimitiveType.UInt: return BitConverter.ToUInt32(source, 0);
                case PrimitiveType.ULong: return BitConverter.ToUInt64(source, 0);
                case PrimitiveType.Char: return BitConverter.ToChar(source, 0);
                case PrimitiveType.Float: return BitConverter.ToSingle(source, 0);
                case PrimitiveType.Double: return BitConverter.ToDouble(source, 0);
                default: throw new ArgumentException(
                    $"The specified {nameof(PrimitiveType)} is not supported by the current {nameof(IProtocolFormatter)}.",
                    nameof(type));
            }
        }


        private static byte[] GetPrimitiveBytes(PrimitiveType type, object value)
        {
            byte[] primitiveBytes;
            
            switch (type)
            {
                case PrimitiveType.Bool:
                    primitiveBytes = BitConverter.GetBytes((bool)value);
                    break;

                case PrimitiveType.Char:
                    primitiveBytes = BitConverter.GetBytes((char)value);
                    break;

                case PrimitiveType.Float:
                    primitiveBytes = BitConverter.GetBytes((float)value);
                    break;

                case PrimitiveType.Double:
                    primitiveBytes = BitConverter.GetBytes((double)value);
                    break;

                case PrimitiveType.Byte:
                    primitiveBytes = BitConverter.GetBytes((byte)value);
                    break;

                case PrimitiveType.Short:
                    primitiveBytes = BitConverter.GetBytes((short)value);
                    break;

                case PrimitiveType.Int:
                    primitiveBytes = BitConverter.GetBytes((int)value);
                    break;

                case PrimitiveType.Long:
                    primitiveBytes = BitConverter.GetBytes((long)value);
                    break;

                case PrimitiveType.SByte:
                    primitiveBytes = BitConverter.GetBytes((sbyte)value);
                    break;

                case PrimitiveType.UShort:
                    primitiveBytes = BitConverter.GetBytes((ushort)value);
                    break;

                case PrimitiveType.UInt:
                    primitiveBytes = BitConverter.GetBytes((uint)value);
                    break;

                case PrimitiveType.ULong:
                    primitiveBytes = BitConverter.GetBytes((ulong)value);
                    break;

                default: throw new InvalidOperationException();
            }

            return primitiveBytes;
        }


        private static byte[] GetEnumBytes(PrimitiveType type, object value)
        {
            byte[] enumBytes;

            switch (type)
            {
                case PrimitiveType.SByte:
                    enumBytes = BitConverter.GetBytes((sbyte)value);
                    break;

                case PrimitiveType.Short:
                    enumBytes = BitConverter.GetBytes((short)value);
                    break;

                case PrimitiveType.Int:
                    enumBytes = BitConverter.GetBytes((int)value);
                    break;

                case PrimitiveType.Long:
                    enumBytes = BitConverter.GetBytes((long)value);
                    break;

                case PrimitiveType.Byte:
                    enumBytes = BitConverter.GetBytes((byte)value);
                    break;

                case PrimitiveType.UShort:
                    enumBytes = BitConverter.GetBytes((ushort)value);
                    break;

                case PrimitiveType.UInt:
                    enumBytes = BitConverter.GetBytes((uint)value);
                    break;

                case PrimitiveType.ULong:
                    enumBytes = BitConverter.GetBytes((ulong)value);
                    break;

                default: throw new SerializationException($"The type {value?.GetType()} has no valid enum base type.");
            }

            BinaryFormatter.MakeLittleEndian(ref enumBytes);
            return enumBytes;
        }


        private static void SerializeArray(
            MemoryStream target,
            SerializationInfo info,
            object source,
            IReadOnlyDictionary<int, (SerializationInfo Info, int ObjectId, object Source)> objects)
        {
            // | ElementClrType : LPStr | rank : 32 | dimensionLength : 32 ... || index : 32 ... | element || ...
            var array = source is Array a ? a : throw new ArgumentException();

            // Element Clr Type
            var clrType = BinaryFormatter.GetLengthPrefixedString(info.ObjectType.GetElementType().AssemblyQualifiedName);
            target.Write(clrType, 0, clrType.Length);

            // rank
            var rankBytes = BitConverter.GetBytes(array.Rank);
            target.Write(rankBytes, 0, sizeof(int));

            var dimensionSizes = new int[array.Rank];

            // dimensions
            for (var d = 0; d < array.Rank; d++)
            {
                // dimension length
                var dimensionLength = array.GetLength(d);
                var dimensionLengthBytes = BitConverter.GetBytes(dimensionLength);
                target.Write(dimensionLengthBytes, 0, sizeof(int));
                dimensionSizes[d] = dimensionLength;
            }

            // -----------------------------------------------
            // elements: | index : 32 ... | element | ...
            // -----------------------------------------------

            var currentIndices = new int[array.Rank];

            for (var currentAbs = 0; currentAbs < array.Length; currentAbs++)
            {
                // Array[i % a, (i/a) % b, (i/(a*b) % c, i / (a*b*c)] = v
                var temp = currentAbs;
                for (var ci = 0; ci < array.Rank; ci++)
                {
                    var divisor = (dimensionSizes.Take(ci).Aggregate(1, (acc, i) => acc * i));
                    var mod = (dimensionSizes[ci]);

                    currentIndices[ci] = (temp / divisor) % mod;
                }

                foreach (var index in currentIndices)
                {
                    var indexBytes = BitConverter.GetBytes(index);
                    target.Write(indexBytes, 0, sizeof(int));
                }

                var elem = array.GetValue(currentIndices);

                byte[] content;

                // write field type
                var fieldTypeBytes = ObjectAnalyzer.GetObjectType(elem);
                target.Write(fieldTypeBytes, 0, sizeof(int));

                // get field major type
                var fieldMajorType = (ObjectType)fieldTypeBytes[0];

                // get field minor type
                var fieldMinorType = (PrimitiveType)fieldTypeBytes[1];

                using (var fieldstr = new MemoryStream())
                {
                    switch (fieldMajorType)
                    {
                        case ObjectType.Null: break; // break if null
                        case ObjectType.Enum: // write enum data
                            byte[] enumBytes = GetEnumBytes(fieldMinorType, elem);
                            fieldstr.Write(enumBytes, 0, enumBytes.Length);
                            break;

                        case ObjectType.Array: // write array obj ref
                        case ObjectType.Object: // write obj ref
                            var objRef = objects.First(o => o.Value.Source == elem).Key;
                            var objRefBytes = BitConverter.GetBytes(objRef);
                            BinaryFormatter.MakeLittleEndian(ref objRefBytes);
                            fieldstr.Write(objRefBytes, 0, objRefBytes.Length);
                            break;

                        case ObjectType.Primitive:
                            byte[] valueBytes = GetPrimitiveBytes(fieldMinorType, elem);
                            BinaryFormatter.MakeLittleEndian(ref valueBytes);
                            fieldstr.Write(valueBytes, 0, valueBytes.Length);
                            break;

                        case ObjectType.String:
                            var stringBytes = BinaryFormatter.GetLengthPrefixedString(elem as string);
                            fieldstr.Write(stringBytes, 0, stringBytes.Length);
                            break;

                        default: throw new InvalidOperationException();
                    }

                    content = fieldstr.ToArray();
                }

                // write element length
                var lengthBytes = BitConverter.GetBytes(content.Length);
                target.Write(lengthBytes, 0, lengthBytes.Length);

                // write content
                target.Write(content, 0, content.Length);
            }
        }

        /// <summary>
        /// Serializes the fields of an <see cref="object" />.
        /// </summary>
        /// <param name="target">
        ///     The target <see cref="MemoryStream" />.
        /// </param>
        /// <param name="info">
        ///     The <see cref="ObjectDataCollection" /> providing data for the serialization.
        /// </param>
        /// <param name="objects">
        ///     The list of all objects in the graph.
        /// </param>
        private static void SerializeObjectFields(
            MemoryStream target,
            SerializationInfo info,
            object source,
            IReadOnlyDictionary<int, (SerializationInfo Info, int ObjectId, object Source)> objects)
        {
            foreach (var field in info)
            {
                // write field type
                var fieldTypeBytes = ObjectAnalyzer.GetObjectType(field.Value);
                target.Write(fieldTypeBytes, 0, fieldTypeBytes.Length);

                // get field major type
                var fieldMajorType = (ObjectType)fieldTypeBytes[0];

                // get field minor type
                var fieldMinorType = (PrimitiveType)fieldTypeBytes[1];
                
                // write field name
                var fieldNameBytes = BinaryFormatter.GetLengthPrefixedString(field.Name);
                target.Write(fieldNameBytes, 0, fieldNameBytes.Length);

                // get content
                byte[] content;

                using (var fieldstr = new MemoryStream())
                {
                    switch (fieldMajorType)
                    {
                        case ObjectType.Null: break; // break if null
                        case ObjectType.Enum: // write enum data
                            byte[] enumBytes = GetEnumBytes(fieldMinorType, field.Value);
                            fieldstr.Write(enumBytes, 0, enumBytes.Length);
                            break;
                            
                        case ObjectType.Array: // write array obj ref
                        case ObjectType.Object: // write obj ref
                            var objRef = objects.First(o => o.Value.Source == field.Value).Key;
                            var objRefBytes = BitConverter.GetBytes(objRef);
                            BinaryFormatter.MakeLittleEndian(ref objRefBytes);
                            fieldstr.Write(objRefBytes, 0, objRefBytes.Length);
                            break;

                        case ObjectType.Primitive:
                            byte[] valueBytes = GetPrimitiveBytes(fieldMinorType, field.Value);
                            BinaryFormatter.MakeLittleEndian(ref valueBytes);
                            fieldstr.Write(valueBytes, 0, valueBytes.Length);
                            break;

                        case ObjectType.String:
                            var stringBytes = BinaryFormatter.GetLengthPrefixedString(field.Value as string);
                            fieldstr.Write(stringBytes, 0, stringBytes.Length);
                            break;

                        default: throw new InvalidOperationException();
                    }

                    content = fieldstr.ToArray();
                }

                // write field value length
                var contentLengthBytes = BitConverter.GetBytes(content.Length);
                BinaryFormatter.MakeLittleEndian(ref contentLengthBytes);
                target.Write(contentLengthBytes, 0, contentLengthBytes.Length);

                // write field value
                target.Write(content, 0, content.Length);
            }
        }

        /// <summary>
        /// Generates the represented object.
        /// </summary>
        /// <param name="root">
        ///     The <see cref="BinaryObjectRepresentation" /> of the object.
        /// </param>
        /// <param name="allObjects">
        ///     A list of all reference objects in the graph.
        /// </param>
        /// <param name="surrogateSelectors">
        ///     A list of surrogate selectors providing <see cref="ISurrogate" /> instances for non-serializable types.
        /// </param>
        /// <returns>
        ///     Returns the generated <see cref="object" />. If the object has already been generated, returns this instance.
        /// </returns>
        private static object GenerateObject(
            BinaryObjectRepresentation root,
            IEnumerable<BinaryObjectRepresentation> allObjects,
            IEnumerable<ISurrogateSelector> surrogateSelectors)
        {
            // if root is already deserialized
            if (root.Result != null)
            {
                return root.Result;
            }

            if (string.IsNullOrEmpty(root.ClrType))
            {
                return root.Result = null;
            }

            // get type and constructor information
            var type = Type.GetType(root.ClrType);

            if (typeof(Array).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                var constructors = type.GetTypeInfo().DeclaredConstructors;
                return Activator.CreateInstance(type, 0);
            }

            // search first for an empty constructor
            var constructor = type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(c => c.GetParameters().Length == 0 && c.IsConstructor);
            if (constructor != null)
            {
                root.Result = constructor.Invoke(new object[0]);
            }
            else
            {
                root.Result = Activator.CreateInstance(type);
            }

            return root.Result;
        }

        /// <summary>
        /// Chooses the deserialization method for the type.
        /// </summary>
        /// <param name="value">
        ///     The value that shall be deserialized.
        /// </param>
        /// <param name="root">
        ///     The <see cref="BinaryObjectRepresentation" /> instance representing <paramref name="value" />.
        /// </param>
        /// <param name="type">
        ///     The type that shall be used.
        /// </param>
        /// <param name="typeInfo">
        ///     The type info that shall be used.
        /// </param>
        /// <param name="serType">
        ///     The <see cref="SerializationTypeInfo" /> representing <paramref name="type" />.
        /// </param>
        /// <param name="valueData">
        ///     The data with which <paramref name="value" /> shall be populated.
        /// </param>
        /// <param name="surrogateSelectors">
        ///     The surrogate selectors for types that do not provide a supported serialization mechanism.
        /// </param>
        /// <returns>
        ///     Returns the deserialized value.
        /// </returns>
        private static object ChooseDeserializationMethod(
            ref object value,
            BinaryObjectRepresentation root,
            Type type,
            TypeInfo typeInfo,
            SerializationTypeInfo serType,
            SerializationInfo valueData,
            IEnumerable<ISurrogateSelector> surrogateSelectors)
        {
            if (value == null)
            {
                return null;
            }

            // check what constructing method shall be chosen
            if (serType.IsMarkedSerializable)
            {
                // if the type is marked with SerializableAttribute attribute
                if (serType.ImplementsISerializable)
                {
                    // if the type additionally implements the ISerializable interface
                    var constructor = typeInfo.DeclaredConstructors.Where(c => c.GetParameters().Length == 2)
                                                                   .Select(c => (Constructor: c, Params: c.GetParameters()))
                                                                   .Where(c => c.Params[0].ParameterType == typeof(SerializationInfo))
                                                                   .Where(c => c.Params[1].ParameterType == typeof(StreamingContext))
                                                                   .Select(c => c.Constructor)
                                                                   .SingleOrDefault();

                    if (constructor == null)
                    {
                        throw new SerializationException(
                            $"The type '{typeInfo}' does not contain the deserialization constructor.");
                    }

                    constructor.Invoke(value, new object[] { valueData });
                }
                else if (typeInfo.IsValueType && !typeInfo.IsPrimitive && !typeInfo.IsEnum)
                {
                    // else if type is struct -> simply set values
                    foreach (var field in typeInfo.DeclaredFields.Where(f => !f.IsInitOnly))
                    {
                        field.SetValue(value, valueData.GetValue(field.Name, typeof(object)));
                    }
                }
                else
                {
                    // if type is only marked with SerializableAttribute attribute
                    // -> search for DeserializationConstructorAttribute or parameterless constructor
                    var constructor = serType.DeserializationConstructor;
                    ////typeInfo.DeclaredConstructors.FirstOrDefault(c => c.CustomAttributes.Any(a => a.AttributeType == typeof(DeserializationConstructorAttribute)));
                    
                    if (constructor != null)
                    {
                        constructor.Invoke(value, new object[0]);
                    }

                    foreach (var field in valueData)
                    {
                        var fieldInfo = typeInfo.GetDeclaredField(field.Name);
                        fieldInfo.SetValue(value, field.Value);
                    }
                }
            }
            else if (serType.IsMarkedDataContract)
            {
                // if the type is marked with DataContractAttribute attribute
                DataContractSurrogate.Default.GetData(value, valueData);
            }
            else if (typeInfo.IsPrimitive)
            {
                // if type is a primitive -> get primitive from bytes
                return root.Result = GetPrimitive(root.ObjectBody, (PrimitiveType)root.FullType[1]);
            }
            else if (typeInfo.IsEnum)
            {
                // if type is an enum
                return root.Result = Enum.ToObject(type, GetPrimitive(root.ObjectBody, (PrimitiveType)root.FullType[1]));
            }
            else if (type == typeof(string))
            {
                // if type is string -> get string from bytes
                using (var memstr = new MemoryStream(root.ObjectBody))
                {
                    var dummy = 0;
                    return root.Result = BinaryFormatter.GetString(memstr, ref dummy);
                }
            }
            else if (typeInfo.IsArray)
            {
                // if type is array
                var binArray = root as BinaryArrayRepresentation;
                var array = value as Array;

                foreach (var elem in valueData)
                {
                    if (elem.Value is ArrayElement arrayElem)
                    {
                        array.SetValue(arrayElem.Value, arrayElem.Indices);
                    }
                }
            }
            else if (surrogateSelectors.Any(s => s.ContainsSurrogate(type)))
            {
                // if any surrogate is specified for the type
                var surrogateSelector = surrogateSelectors.First(s => s.ContainsSurrogate(type));
                var surrogateSelectorType = surrogateSelector.GetType();

                // retrieve surrogate
                var surrogate = surrogateSelector.GetSurrogate(type);
                ////var method = surrogateSelectorType.GetRuntimeMethod("GetSurrogate", new [] { typeof(Type) });
                ////dynamic surrogate = method.MakeGenericMethod(type).Invoke(surrogateSelector, new object[0]);
                surrogate.SetData(ref value, valueData);
            }
            else if (typeInfo.IsArray && value is Array array)
            {
                var length = valueData.GetInt32(nameof(Array.Length));

                for (int n = 0; n < length; n++)
                {
                    var indexVal = valueData.GetValue(n.ToString(), typeof(object));
                    array.SetValue(indexVal, n);
                }
            }
            else if (serType.IsICollection)
            {
                // if type implements ICollection<T>
                var contentType = typeInfo.ImplementedInterfaces.FirstOrDefault(
                    i => i.GetGenericTypeDefinition() == typeof(ICollection<>))?.GenericTypeArguments.First();

                // get generic type definition of CollectionSurrogate<T> and make new generic type with content type
                var genericCollectionSurrogateType = typeof(CollectionSurrogate<>);
                var collectionSurrogateType = genericCollectionSurrogateType.MakeGenericType(contentType);
                ISurrogate defaultSurrogate = (ISurrogate)collectionSurrogateType.GetTypeInfo().GetDeclaredField("Default").GetValue(null);
                defaultSurrogate.SetData((dynamic)value, valueData);
            }
            else
            {
                // throw exception because no data can be collected from a type
                throw new SerializationException(
                    $"The {nameof(GraphIterator)} cannot collect data from objects of type {type.AssemblyQualifiedName}.");
            }

            return value;
        }


        private static void ReadArray(BinaryArrayRepresentation source)
        {
            using (var memstr = new MemoryStream(source.ObjectBody, false))
            {
                var currentLength = 0;

                // read element clr type
                var clrType = BinaryFormatter.GetString(memstr, ref currentLength);

                // read array rank
                var rankBytes = new byte[sizeof(int)];
                BinaryFormatter.ReadStream(memstr, rankBytes, sizeof(int));
                currentLength += sizeof(int);
                var rank = BitConverter.ToInt32(rankBytes, 0);

                var dimensionSizes = new int[rank];

                // read dimension sizes
                for (var currentDimension = 0; currentDimension < rank; currentDimension++)
                {
                    var dimensionLengthBytes = new byte[sizeof(int)];
                    BinaryFormatter.ReadStream(memstr, dimensionLengthBytes, sizeof(int));
                    currentLength += sizeof(int);
                    var dimensionLength = BitConverter.ToInt32(dimensionLengthBytes, 0);
                    dimensionSizes[currentDimension] = dimensionLength;
                }

                var elemCount = 0;

                // read elements
                while (currentLength < source.ObjectBody.Length)
                {
                    // read indices
                    var indices = new int[rank];
                    for (var currentIndex = 0; currentIndex < rank; currentIndex++)
                    {
                        var indexBytes = new byte[sizeof(int)];
                        BinaryFormatter.ReadStream(memstr, indexBytes, sizeof(int));
                        currentLength += sizeof(int);
                        var index = BitConverter.ToInt32(indexBytes, 0);
                        indices[currentIndex] = index;
                    }

                    // read field type
                    var fieldTypeBytes = new byte[sizeof(int)];
                    BinaryFormatter.ReadStream(memstr, fieldTypeBytes, sizeof(int));
                    currentLength += sizeof(int);

                    // read field length
                    var fieldLengthBytes = new byte[sizeof(int)];
                    BinaryFormatter.ReadStream(memstr, fieldLengthBytes, sizeof(int));
                    currentLength += sizeof(int);
                    var fieldLength = BitConverter.ToInt32(fieldLengthBytes, 0);

                    // read field body
                    var bodyBytes = new byte[fieldLength];
                    BinaryFormatter.ReadStream(memstr, bodyBytes, fieldLength);
                    currentLength += fieldLength;
                    
                    var arrayElem = new BinaryArrayElement(indices, elemCount, fieldTypeBytes, bodyBytes);
                    source.Elements.Add(arrayElem);
                    elemCount++;
                }

                source.DimensionSizes = dimensionSizes;
                source.Rank = rank;
                source.Result = Array.CreateInstance(Type.GetType(source.ClrType).GetElementType(), dimensionSizes);
            }
        }

        /// <summary>
        /// Parses the fields of an object out of the content.
        /// </summary>
        /// <param name="source">
        ///     The <see cref="BinaryObjectRepresentation" /> serving as source.
        /// </param>
        private static void ReadObjectFields(BinaryObjectRepresentation source)
        {
            using (var memstr = new MemoryStream(source.ObjectBody, false))
            {
                var currentLength = 0;
                while (currentLength < source.ObjectBody.Length)
                {
                    ObjectType fieldType;
                    string fieldName;
                    int fieldLength;

                    var fieldTypeBytes = new byte[sizeof(uint)];
                    var fieldLengthBytes = new byte[sizeof(uint)];

                    // read field type
                    BinaryFormatter.ReadStream(memstr, fieldTypeBytes, sizeof(uint));
                    currentLength += sizeof(uint);
                    fieldType = (ObjectType)fieldTypeBytes[0];

                    // read field name
                    fieldName = BinaryFormatter.GetString(memstr, ref currentLength);

                    // read field length
                    BinaryFormatter.ReadStream(memstr, fieldLengthBytes, sizeof(int));
                    BinaryFormatter.MakeLittleEndian(ref fieldLengthBytes);
                    currentLength += sizeof(int);
                    fieldLength = BitConverter.ToInt32(fieldLengthBytes, 0);

                    // read field value
                    var fieldValueBytes = new byte[fieldLength];
                    BinaryFormatter.ReadStream(memstr, fieldValueBytes, fieldLength);
                    currentLength += fieldLength;

                    source.Properties.Add(new Tuple<string, byte[]>(fieldName, fieldTypeBytes), fieldValueBytes);
                }
            }
        }
    }
}
