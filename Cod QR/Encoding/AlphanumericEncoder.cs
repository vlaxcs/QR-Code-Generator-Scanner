namespace QREncoder {
    public class AlphanumericEncoder : IEncoder {
        static readonly char[] alphanumericConversion = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' ', '$', '%', '*', '+', '-', '.', '/', ':' };
        public byte[] Encode(string input) {
            var encoded = new List<byte>();

            int i = 0;
            while(i + 1 < input.Length) {
                var ind1 = EncodeChar(input[i]);
                var ind2 = EncodeChar(input[i + 1]);

                var v = 45 * ind1 + ind2;

                AppendElementBits(encoded, v);

                i += 2;
            }
            if(i < input.Length) {
                var ind = EncodeChar(input[input.Length - 1]);

                AppendElementBits(encoded, ind, 6);
            }


            return encoded.ToArray();
        }

        int EncodeChar(char c) {
            var ind = Array.IndexOf(alphanumericConversion, c);
            if(ind < 0) throw new Exception("Message cant be encoded alphanumerically");

            return ind;
        }

        void AppendElementBits(List<byte> bits, int elem, int blocksize = 11) {
            var bitsBuilder = new List<byte>();

            while(elem != 0 && blocksize > 0) {
                bitsBuilder.Add((byte)(elem & 1));
                elem >>= 1;

                blocksize--;
            }

            while(blocksize > 0) {
                bitsBuilder.Add(0);
                blocksize--;
            }

            for(int i = bitsBuilder.Count - 1; i >= 0; i--) {
                bits.Add(bitsBuilder[i]);
            }
        }
    }
}