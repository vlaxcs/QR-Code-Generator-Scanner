namespace QREncoder {
    public class NumericEncoder : IEncoder {
        public byte[] Encode(string input) {
            var encoded = new List<byte>();

            int i = 0;
            while(i + 2 < input.Length) {
                var a = EncodeChar(input[i]);
                var b = EncodeChar(input[i + 1]);
                var c = EncodeChar(input[i + 2]);

                var v = 100 * a + 10 * b + c;

                AppendElementBits(encoded, v);

                i += 3;
            }

            var digitsLeft = input.Length % 3;
            i = input.Length - digitsLeft;
            if(digitsLeft > 0) {
                var v = 0;

                for(; i < input.Length; i++) {
                    var a = EncodeChar(input[i]);
                    v = v * 10 + a;
                }
                AppendElementBits(encoded, v, 10 - 3 * (3 - digitsLeft));
            }


            return encoded.ToArray();
        }

        int EncodeChar(char c) {
            if(c < '0' || c > '9') throw new Exception("Message cant be encoded numerically");

            return c - '0';
        }

        void AppendElementBits(List<byte> bits, int elem, int blocksize = 10) {
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