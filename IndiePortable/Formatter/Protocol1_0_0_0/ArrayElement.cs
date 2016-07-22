using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndiePortable.Formatter.Protocol1_0_0_0
{



    internal class ArrayElement
    {

        public ArrayElement(int[] indices, object value)
        {
            this.Indices = indices;
            this.Value = value;
        }


        public int[] Indices { get; private set; }


        public object Value { get; private set; }
    }
}
