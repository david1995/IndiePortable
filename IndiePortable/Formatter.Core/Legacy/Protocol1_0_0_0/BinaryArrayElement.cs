

namespace IndiePortable.Formatter.Protocol1_0_0_0
{
    internal class BinaryArrayElement
    {

        public BinaryArrayElement(int[] indices, int serialIndex, byte[] elementType, byte[] body)
        {
            this.Indices = indices;
            this.SerialIndex = serialIndex;
            this.ElementType = elementType;
            this.BodyBytes = body;
        }


        public int[] Indices { get; private set; }


        public int SerialIndex { get; private set; }


        public byte[] ElementType { get; private set; }


        public byte[] BodyBytes { get; private set; }
    }
}
