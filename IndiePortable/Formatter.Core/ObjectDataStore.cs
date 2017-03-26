// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectDataStore.cs" company="David Eiwen">
// Copyright © 2017 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ObjectDataStore class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    public class ObjectDataStore
        : IGetPropertyData, ISetPropertyData
    {

        private readonly IDictionary<FieldName, object> fieldData;


        public ObjectDataStore(object source)
        {
            this.Source = source;
            this.FieldData = new Dictionary<FieldName, object>();
            this.FieldData = new ReadOnlyDictionary<FieldName, object>(this.fieldData);
        }


        public ObjectDataStore(IDictionary<FieldName, object> data)
        {
            this.IsDeserialized = true;
            this.fieldData = data;
            this.FieldData = new ReadOnlyDictionary<FieldName, object>(this.fieldData);
        }


        public bool IsDeserialized { get; }


        public object Source { get; }


        public IReadOnlyDictionary<FieldName, object> FieldData { get; }


        public void AddField(FieldName name, object value)
        {
            if (this.IsDeserialized)
            {
                throw new InvalidOperationException(
                    "The object data store cannot be changed because the object is not in serialization mode.");
            }

            this.fieldData.Add(name, value);
        }


        public object GetField(FieldName name) => this.FieldData[name];


        public T GetFieldName<T>(FieldName name) => (T)this.FieldData[name];


        public bool HasField(FieldName name) => this.FieldData.ContainsKey(name);
    }
}
