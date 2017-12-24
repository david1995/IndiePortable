// <copyright file="GraphDataCollector.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Formatter.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using IndiePortable.Formatter.Core.Graph;
    using IndiePortable.Formatter.Extensions;

    public class GraphDataCollector
    {
        public INodeData GetGraphData(object root)
        {
            var collectedObjects = new HashSet<object>();
            var collectedNodes = new Dictionary<object, INodeData>();
            INodeData GetObjectData(object source)
            {
                switch (source)
                {
                    // enum type
                    case Enum e: return new EnumData(e);

                    // primitive type:
                    case ValueType p when p.GetType().GetTypeInfo().IsPrimitive: return new ValueData(p);

                    // string type
                    case string str: return new StringData(str);

                    // array type with rank 1
                    // TODO: ability to gather data about arrays with rank > 1
                    case Array a when a.Rank == 1 && collectedObjects.Add(source):
                        var adata = new ArrayData(a.Length, a.Cast<object>().Select(o => GetObjectData(o)));
                        collectedNodes.Add(source, adata);
                        return adata;

                    // complex type marked with the SerializableAttribute and implementing ISerializable
                    case ISerializable i when i.GetType().GetTypeInfo().GetCustomAttributes<SerializableAttribute>().Any() && collectedObjects.Add(source):
                        var serInfo = new SerializationInfo(i.GetType(), DefaultFormatterConverter.Instance);
                        i.GetObjectData(serInfo, default(StreamingContext));

                        var idata = new ObjectData(from f in serInfo.AsEnumerable()
                                                   select new FieldData(f.Name, GetObjectData(f.Value)));
                        collectedNodes.Add(i, idata);
                        return idata;

                    // ICollection<T> implementations
                    case IEnumerable c when c.GetType().GetInterfaces().Where(i => i.IsConstructedGenericType).Select(i => i.GetGenericTypeDefinition()).Contains(typeof(ICollection<>))
                                         && collectedObjects.Add(source):

                        var cdata = new ArrayData(c.Cast<object>().Count(), c.Cast<object>().Select(i => GetObjectData(i)));
                        collectedNodes.Add(source, cdata);
                        return cdata;

                    // complex type only marked with the SerializableAttribute
                    case object s when s.GetType().GetTypeInfo().GetCustomAttributes<SerializableAttribute>().Any() && collectedObjects.Add(source):
                        var sdata = new ObjectData(from f in s.GetType().GetTypeInfo().DeclaredFields
                                                   where !f.GetCustomAttributes<NonSerializedAttribute>(true).Any()
                                                   select new FieldData(f.Name, GetObjectData(f.GetValue(source))));
                        collectedNodes.Add(s, sdata);
                        return sdata;

                    // complex type marked with the DataContractAttribute
                    case object d when d.GetType().GetTypeInfo().GetCustomAttributes<SerializableAttribute>().Any() && collectedObjects.Add(source):
                        var props = from p in d.GetType().GetTypeInfo().DeclaredProperties
                                    where p.GetCustomAttributes<DataMemberAttribute>().Any() && p.CanRead && p.CanWrite
                                    select new FieldData(p.Name, GetObjectData(p.GetValue(d)));

                        var fields = from f in d.GetType().GetTypeInfo().DeclaredFields
                                     where f.GetCustomAttributes<DataMemberAttribute>().Any() && !f.IsInitOnly
                                     select new FieldData(f.Name, GetObjectData(f.GetValue(d)));

                        var ddata = new ObjectData(props.Concat(fields));
                        collectedNodes.Add(d, ddata);
                        return ddata;

                    // already collected object
                    case object o when collectedObjects.Contains(o): return collectedNodes[o];

                    // null reference
                    case null: return NullData.Instance;

                    // neither of the known types
                    default: throw new SerializationException("The specified object is not serializable.");
                }
            }

            return GetObjectData(root);
        }
    }
}
