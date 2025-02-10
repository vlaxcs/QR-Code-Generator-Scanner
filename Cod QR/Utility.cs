public static class Utility {

    public static int SizeForVersion(int version) => version * 4 + 17;

    public static int MostSignificantBit(this int x) {
        int cx = x;
        while(x != 0) {
            cx = x;
            x = x & (x - 1);
        }
        return cx;
    }

    public static int[][] ReadMatrixFromFile(string filePath) {
        var lines = File.ReadAllLines(filePath);
        int[][] res = new int[lines.Length][];
        for(int i = 0; i < lines.Length; i++) {
            res[i] = new int[lines[i].Length];
            for(int j = 0; j < lines[i].Length; j++) {
                res[i][j] = Convert.ToInt32(lines[i][j]) - 48;
            }
        }
        return res;
    }

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

    public static int ComputeEncodingRange(DataType type, int version) {
        int encodingMinVersion = 1, encodingMaxVersion = 40;
        if(encodingMinVersion > version || version > encodingMaxVersion) throw new Exception("Unsupported QR Code version!");
        int i = (version + 7) / 17;
        switch(type) {
            case DataType.Numeric: { int[] blockSizes = { 10, 12, 14 }; return blockSizes[i]; }       // Numeric encoding (10/12/14 bits per 3 digits)
            case DataType.Alphanumeric: { int[] blockSizes = { 9, 11, 13 }; return blockSizes[i]; }  // Alphanumeric encoding (9/11/13 bits per 2 characters)
            case DataType.Byte: { int[] blockSizes = { 8, 16, 16 }; return blockSizes[i]; }         // Byte encoding (8/16/16 bits per character) 
            case DataType.Kanji: { int[] blockSizes = { 8, 10, 12 }; return blockSizes[i]; }       // Kanji encoding (8/10/12 bits per character)
            default: { throw new Exception("Unsupported QR Code version!"); }
        }
    }

    public static void SetCurrentPolynomial(int blockCount, int[] current) {
        var field = new GaloisField();
        field.GeneratePolynomials();
        for(int i = 0; i < blockCount; i++) current[i] = field.exp[i];
    }




    // For console outputs
    public static void WriteValid(string output) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(output);
    }
    public static void WriteValid(string output, int[] array) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(output);
        if(array != null) {
            for(int j = 0; j < array.Length; j++)
                Console.Write(Convert.ToChar(array[j]));
        }
        Console.WriteLine();
    }

    public static void WriteInvalid(string output) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(output);
    }

    public static void WriteNeutral(string output) {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(output);
    }

    public static void ResetConsole() {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine();
    }
}