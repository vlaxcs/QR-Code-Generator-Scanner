public static class Utility {
    public static int BinaryArea(int[] array, int left, int right) {
        int decify(int[] array, int currPow) {
            int result = 0;
            while(currPow-- > 0) result += (1 << currPow) * array[array.Length - currPow - 1];
            return result;
        }

        int upperPow = 0; int[] binary = new int[right - left + 1];
        for(int i = left; i <= right; ++i) binary[upperPow++] = array[i];
        return decify(binary, upperPow);
    }

    public static int ComputeEncodingRange(string type, int version) {
        int encodingMinVersion = 1, encodingMaxVersion = 40;
        if(encodingMinVersion > version || version > encodingMaxVersion) throw new Exception("Unsupported QR Code version!");
        int i = (version + 7) / 17;
        switch(type) {
            case "Numeric": { int[] blockSizes = { 10, 12, 14 }; return blockSizes[i]; }       // Numeric encoding (10/12/14 bits per 3 digits)
            case "Alphanumeric": { int[] blockSizes = { 9, 11, 13 }; return blockSizes[i]; }  // Alphanumeric encoding (9/11/13 bits per 2 characters)
            case "Byte": { int[] blockSizes = { 8, 16, 16 }; return blockSizes[i]; }         // Byte encoding (8/16/16 bits per character) 
            case "Kanji": { int[] blockSizes = { 8, 10, 12 }; return blockSizes[i]; }       // Kanji encoding (8/10/12 bits per character)
            default: { throw new Exception("Unsupported QR Code version!"); }
        }
    }

    public static void SetCurrentPolynomial(int blockCount, int[] current) {
        var field = new GaloisField();
        field.GeneratePolynomials();
        for(int i = 0; i < blockCount; i++) current[i] = field.exp[i];
    }
}