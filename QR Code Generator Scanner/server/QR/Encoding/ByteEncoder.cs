using System.Text;

namespace QREncoder 
{
    public class ByteEncoder : IEncoder
    {
        public byte[] Encode(string input)
        {
            var bytes = Encoding.ASCII.GetBytes(input);

            var encoded = new List<byte>();

            for(int i = 0; i < bytes.Length; i++)
            {
                AppendElementBits(encoded, bytes[i]);
            }

            return encoded.ToArray();
        }
        void AppendElementBits(List<byte> bits, byte elem, int blocksize = 8)
        {
            for(int b = 0; b < blocksize; b++)
            {
                bits.Add((byte)((elem & 0b1000_0000) >> 7));
                elem <<= 1;
            }
        }
    }
}