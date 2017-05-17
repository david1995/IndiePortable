

namespace IndiePortable.Formatter.Protocol1_0_0_0
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    internal class BinaryArrayRepresentation
        : BinaryObjectRepresentation
    {

        public BinaryArrayRepresentation(int objectId, byte[] fullType, string clrType, byte[] objectBody)
            : base(objectId, fullType, clrType, objectBody)
        {
            this.Elements = new List<BinaryArrayElement>();
        }


        internal List<BinaryArrayElement> Elements { get; private set; }


        internal int[] DimensionSizes { get; set; }


        internal int Rank { get; set; }
    }
}
