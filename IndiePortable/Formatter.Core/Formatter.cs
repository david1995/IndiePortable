// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="Formatter.cs" company="David Eiwen">
// Copyright © 2017 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the Formatter class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    public abstract class Formatter
    {




        public void Serialize(Stream output, object graph)
        {
            if (object.ReferenceEquals(output, null))
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (!output.CanWrite)
            {
                throw new ArgumentException("The output stream cannot be written.", nameof(output));
            }

            // TODO: get object graph data


        }


        public object Deserialize(Stream input)
        {

        }


        public bool TryDeserialize(Stream input, out object result)
        {

        }


        public T Deserialize<T>(Stream input)
        {

        }


        public bool TryDeserialize<T>(Stream input, out T result)
        {

        }


        protected abstract void SerializeOverride(Stream output, object graph);


        protected abstract object DeserializeOverride(Stream input);


        protected abstract object TryDeserializeOverride(Stream input, out object result);


        protected abstract T DeserializeOverride<T>(Stream input);


        protected abstract bool TryDeserializeOverride<T>(Stream input, out T result);
    }
}
