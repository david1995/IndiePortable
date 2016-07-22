// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphIterator.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the GraphIterator class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Threading;

    /// <summary>
    /// Provides methods for reading an object graph.
    /// </summary>
    /// <remarks>
    ///     Implements <see cref="IDisposable" /> explicitly.
    /// </remarks>
    public class GraphIterator
        : IDisposable
    {
        /// <summary>
        /// The <see cref="ReaderWriterLockSlim" /> that handles thread synchronization for the <see cref="GraphIterator" />.
        /// </summary>
        private ReaderWriterLockSlim readWriteLock;

        /// <summary>
        /// The list providing a cache of <see cref="SerializationTypeInfo" /> instances.
        /// </summary>
        private List<SerializationTypeInfo> typeInfosCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphIterator" /> class.
        /// </summary>
        public GraphIterator()
        {
            this.readWriteLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            this.typeInfosCache = new List<SerializationTypeInfo>();
        }

        /// <summary>
        /// Determines whether the specified type implements the <see cref="ISerializable" /> interface.
        /// </summary>
        /// <param name="type">
        ///     The <see cref="TypeInfo" /> that shall be checked.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether <paramref name="type" /> implements the <see cref="ISerializable" /> interface.
        /// </returns>
        public static bool ImplementsISerializable(TypeInfo type)
            => type?.ImplementedInterfaces.Contains(typeof(ISerializable)) == true;

        /// <summary>
        /// Determines whether the specified type is marked with the <see cref="SerializableAttribute" /> attribute.
        /// </summary>
        /// <param name="type">
        ///     The <see cref="TypeInfo" /> that shall be checked.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether <paramref name="type" /> is marked with the <see cref="SerializableAttribute" /> attribute.
        /// </returns>
        public static bool IsMarkedSerializable(MemberInfo type)
            => type?.CustomAttributes.Any(a => a.AttributeType == typeof(SerializableAttribute)) == true;

        /// <summary>
        /// Determines whether the specified type is marked with the <see cref="DataContractAttribute" /> attribute.
        /// </summary>
        /// <param name="type">
        ///     The <see cref="TypeInfo" /> that shall be checked.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether <paramref name="type" /> is marked with the <see cref="DataContractAttribute" /> attribute.
        /// </returns>
        public static bool IsMarkedDataContract(MemberInfo type)
            => type?.CustomAttributes.Any(a => a.AttributeType == typeof(DataContractAttribute)) == true;

        /// <summary>
        /// Determines whether the specified field is marked with the <see cref="DataMemberAttribute" /> attribute.
        /// </summary>
        /// <param name="field">
        ///     The <see cref="FieldInfo" /> that shall be checked.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether <paramref name="field" /> is marked with the <see cref="DataMemberAttribute" /> attribute.
        /// </returns>
        public static bool IsMarkedDataMember(MemberInfo field)
            => field?.CustomAttributes.Any(a => a.AttributeType == typeof(DataMemberAttribute)) == true;

        /// <summary>
        /// Determines whether the specified value can be serialized.
        /// </summary>
        /// <param name="value">
        ///     The value that shall be checked. The whole object graph is not checked recursively.
        /// </param>
        /// <param name="surrogateSelectors">
        ///     The list of <see cref="ISurrogateSelector" /> instances that contain surrogates for certain types that are not serializable by itself.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the root <paramref name="value" /> can be serialized.
        /// </returns>
        public static bool CanBeSerialized(object value, IEnumerable<ISurrogateSelector> surrogateSelectors)
        {
            // return true if value is null
            if (value == null)
            {
                return true;
            }
            
            var type = value.GetType().GetTypeInfo();

            // if the type is marked with SerializableAttribute attribute
            // if the type is marked with DataContractAttribute attribute
            // if type is a primitive
            // if type is enum
            // if type is string
            // if any surrogate is specified for the type
            // if type is an array
            // if type implements ICollection<T>
            // -> true
            return IsMarkedSerializable(type) ||
                IsMarkedDataContract(type) ||
                type.IsPrimitive ||
                type.IsEnum ||
                type.AsType() == typeof(string) ||
                surrogateSelectors.Any(s => s.ContainsSurrogate(type.AsType())) ||
                type.IsArray ||
                type.ImplementedInterfaces.Any(i => i.GetTypeInfo().IsGenericType &&
                                                    !i.GetTypeInfo().IsGenericTypeDefinition &&
                                                    i.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        /// <summary>
        /// Retrieves object serialization information from an object graph.
        /// </summary>
        /// <param name="graph">
        ///     The graph that shall be serialized. Can be <c>null</c>.
        /// </param>
        /// <param name="surrogateSelectors">
        ///     The list of <see cref="ISurrogateSelector" /> instances that retrieve data from types that do not provide a serialization mechanism.
        /// </param>
        /// <returns>
        ///     Returns the <see cref="Dictionary{TKey, TValue}" /> containing all objects.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="surrogateSelectors" /> is <c>null</c>.
        /// </exception>
        public Dictionary<int, ObjectDataCollection> GetGraphData(object graph, IEnumerable<ISurrogateSelector> surrogateSelectors)
        {
            // throw exception if surrogate selectors is null
            if (surrogateSelectors == null)
            {
                throw new ArgumentNullException(nameof(surrogateSelectors));
            }

            var ret = new Dictionary<int, ObjectDataCollection>();
            int currentID = 0;
            this.GetGraphData(graph, surrogateSelectors, ret, ref currentID);
            return ret;
        }

        /// <summary>
        /// Releases any resources reserved by the <see cref="GraphIterator" />.
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
        ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool managed)
        {
            this.readWriteLock.Dispose();

            if (managed)
            {
                this.typeInfosCache = null;
                this.readWriteLock = null;
            }
        }

        /// <summary>
        /// Adds the specified <see cref="Type" /> to the <see cref="SerializationTypeInfo" /> cache.
        /// </summary>
        /// <param name="type">
        ///     The <see cref="Type" /> that shall be added.
        /// </param>
        protected void AddType(Type type)
        {
            this.readWriteLock.EnterWriteLock();
            try
            {
                this.typeInfosCache.Add(new SerializationTypeInfo(type));
            }
            finally
            {
                this.readWriteLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Gets the <see cref="SerializationTypeInfo" /> for the specified cached <see cref="Type" />.
        /// </summary>
        /// <param name="type">
        ///     The type of which the <see cref="SerializationTypeInfo" /> shall be got.
        /// </param>
        /// <returns>
        ///     Returns the <see cref="SerializationTypeInfo" /> for the specified <see cref="Type" />.
        /// </returns>
        protected SerializationTypeInfo GetTypeInfo(Type type)
        {
            this.readWriteLock.EnterReadLock();
            try
            {
                return this.typeInfosCache.First(s => s.ViewedType == type);
            }
            finally
            {
                this.readWriteLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="Type" /> is already cached.
        /// </summary>
        /// <param name="type">
        ///     The <see cref="Type" /> that shall be checked.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the type is already cached.
        /// </returns>
        protected bool IsTypeCached(Type type)
        {
            this.readWriteLock.EnterReadLock();
            try
            {
                return this.typeInfosCache.Any(s => s.ViewedType == type);
            }
            finally
            {
                this.readWriteLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets the <see cref="SerializationTypeInfo" /> for the specified <see cref="Type" />.
        /// </summary>
        /// <param name="type">
        ///     The <see cref="Type" /> for which the <see cref="SerializationTypeInfo" /> shall be got.
        /// </param>
        /// <returns>
        ///     Returns the <see cref="SerializationTypeInfo" /> for the specified <see cref="Type" />.
        /// </returns>
        private SerializationTypeInfo CheckType(Type type)
        {
            this.readWriteLock.EnterUpgradeableReadLock();
            try
            {
                // add type if it does not exist
                if (!this.IsTypeCached(type))
                {
                    this.AddType(type);
                }

                return this.GetTypeInfo(type);
            }
            finally
            {
                this.readWriteLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Retrieves object serialization information from an object graph.
        /// </summary>
        /// <param name="graph">
        ///     The graph that shall be serialized. Can be <c>null</c>.
        /// </param>
        /// <param name="surrogateSelectors">
        ///     The list of <see cref="ISurrogateSelector" /> instances that retrieve data from types that do not provide a serialization mechanism.
        /// </param>
        /// <param name="currentObjects">
        ///     The dictionary containing all serialized objects and their IDs.
        /// </param>
        /// <param name="currentID">
        ///     The current ID for objects in the graph. This parameter must be passed by reference.
        /// </param>
        private void GetGraphData(
            object graph,
            IEnumerable<ISurrogateSelector> surrogateSelectors,
            Dictionary<int, ObjectDataCollection> currentObjects,
            ref int currentID)
        {
            // throw exception if value cannot be serialized
            if (!CanBeSerialized(graph, surrogateSelectors))
            {
                throw new SerializationException($"The type {graph?.GetType()} cannot be serialized.");
            }

            // return if value already exists
            if (currentObjects.Any(kv => kv.Value.Source == graph))
            {
                return;
            }

            var objData = new ObjectDataCollection(graph, currentID);

            // special case -> graph is null
            if (graph == null)
            {
                currentObjects.Add(currentID++, objData);
                return;
            }

            // get fitting serialization method
            var typeInfo = this.CheckType(graph.GetType());
            var type = typeInfo.ViewedType.GetTypeInfo();

            if (typeInfo.IsMarkedSerializable)
            {
                // if the type is marked with SerializableAttribute attribute
                if (typeInfo.ImplementsISerializable)
                {
                    // if the type additionally implements the ISerializable interface
                    (graph as ISerializable).GetObjectData(objData);
                }
                else if (type.IsValueType && !type.IsPrimitive && !type.IsEnum)
                {
                    // if serializable type is struct -> simply read fields
                    foreach (var field in type.DeclaredFields.Where(f => !f.IsInitOnly))
                    {
                        objData.AddValue(field.Name, field.GetValue(graph));
                    }
                }
                else
                {
                    // if type is only marked with SerializableAttribute attribute
                    SerializableSurrogate.Default.GetData(graph, objData);
                }
            }
            else if (typeInfo.IsMarkedDataContract)
            {
                // if the type is marked with DataContractAttribute attribute
                DataContractSurrogate.Default.GetData(graph, objData);
            }
            else if (type.IsPrimitive || type.AsType() == typeof(string) || type.IsEnum)
            {
                // if type is a primitive, enum or string -> set directly
                // -> do not add to list
                return;
            }
            else if (surrogateSelectors.Any(s => s.ContainsSurrogate(type.AsType())))
            {
                // if any surrogate is specified for the type
                var surrogateSelector = surrogateSelectors.First(s => s.ContainsSurrogate(type.AsType()));
                
                var surrogate = surrogateSelector.GetSurrogate(type.AsType());
                surrogate.GetData(graph, objData);
            }
            else if (type.IsArray)
            {
                var array = graph as Array;
                var n = 0;
                objData.AddValue(nameof(Array.Rank), array.Rank);
                objData.AddValue(nameof(Array.Length), array.Length);
                for (var d = 0; d < array.Rank; d++)
                {
                    objData.AddValue($"d{d: x8}", array.GetLength(d));
                }

                foreach (var elem in array)
                {
                    objData.AddValue(n.ToString(), elem);
                    n++;
                }
            }
            else if (typeInfo.IsICollection)
            {
                // if type implements ICollection<T>
                var contentType = type.ImplementedInterfaces.FirstOrDefault(
                    i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>))?.GenericTypeArguments.Single();

                // get generic type definition of CollectionSurrogate<T> and make new generic type with content type
                var genericCollectionSurrogateType = typeof(CollectionSurrogate<>);
                var collectionSurrogateType = genericCollectionSurrogateType.MakeGenericType(contentType);
                dynamic defaultSurrogate = collectionSurrogateType.GetTypeInfo().GetDeclaredField("Default").GetValue(null);
                defaultSurrogate.GetData((dynamic)graph, objData);
            }
            else
            {
                // throw exception because no data can be collected from a type
                throw new SerializationException(
                    $"The {nameof(GraphIterator)} cannot collect data from objects of type {type.AssemblyQualifiedName}.");
            }
            
            currentObjects.Add(currentID++, objData);

            // recursively check all field values
            foreach (var field in objData)
            {
                this.GetGraphData(field.Value, surrogateSelectors, currentObjects, ref currentID);
            }
        }
    }
}
