public class Testing {
    public void Main() {
        Dictionary<int, string> encodingTypes = new Dictionary<int, string>
        {
            { 1, "Numeric" },       // 0b0001 / 1 - Numeric encoding
            { 2, "Alphanumeric" },  // 0b0010 / 2 - Alphanumeric encoding
            { 4, "Byte" },          // 0b0100 / 4 - Byte encoding
            { 8, "Kanji" }          // 0b1000 / 8 - Kanji encoding
        };

        // From [Stefan]
        int[] bitArray = { 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0 };

        // Analizes the first 4 blocks of the encoding and gets the encoding type as decimal, let it be encodingID
        int encodingID = Utility.BinaryArea(bitArray, 0, 3);
        Console.WriteLine($"[RAW DATA] Encoding ID: {encodingID}");

        // Sets the encoding type, given the encoding ID (browsing a dictionary)
        string encodingType = encodingTypes[encodingID];
        Console.WriteLine($"[RAW DATA] Encoding TYPE: {encodingType}");

        // Compute the encoding length, given the length of the bit array
        int encodingLength = bitArray.Length;
        Console.WriteLine($"[RAW DATA] Encoding LENGTH: {encodingLength}");

        // From [Stefan]
        int encodingVersion = 1;
        Console.WriteLine($"[RAW DATA] Encoding VERSION: {encodingVersion}");

        // Sets the codeword blocks' size in 'encodingRange'
        int encodingRange = Utility.ComputeEncodingRange(encodingType, encodingVersion);
        Console.WriteLine($"[RAW DATA] Encoding RANGE: {encodingRange}");

        // From [Stefan]
        int maskID = -1;
        Console.WriteLine($"[RAW DATA] Encoding MASK: {maskID}");

        // From [Stefan]
        int errorCorrectionLevel = -1;
        Console.WriteLine($"[RAW DATA] Encoding ERROR CORRECTION LEVEL: {errorCorrectionLevel}");

        int ls = 4, lf = 3 + encodingRange;
        int decodedMessageLength = Utility.BinaryArea(bitArray, ls, lf);
        Console.WriteLine($"[RAW DATA] Decoded message LENGTH: {decodedMessageLength}");

        // TBC
        string decodedMessage = ""; int newDecodedMessageLength = 0, ecCodeWordsCount = 0;
        for(int i = lf + 1; i <= lf + encodingLength; i += encodingRange)
            try {
                int asciiCode = 0;
                if(newDecodedMessageLength < decodedMessageLength) {
                    for(int j = 0; j < encodingRange; ++j) {
                        Console.Write(bitArray[i + j]);
                        asciiCode += bitArray[i + j] * (int)Math.Pow(2, encodingRange - j - 1);
                    }
                    char messageSymbol = (char)asciiCode;
                    decodedMessage += messageSymbol;
                    newDecodedMessageLength++;
                    Console.WriteLine($" << ASCII CODE: {asciiCode} | Character: {messageSymbol}");
                } else {
                    for(int j = 0; j < encodingRange; ++j)
                        Console.Write(bitArray[i + j]);
                    ecCodeWordsCount++;

                    Console.WriteLine($" << Initial error correction block");
                }
            } catch(Exception e) { break; }

        Console.WriteLine($"[RAW DATA] Decoded message: {decodedMessage}");
        Console.WriteLine($"[ERROR CORRECTION] CODE WORDS NUMBER: {ecCodeWordsCount}");

        int[] ecCurrentPolynomial = new int[ecCodeWordsCount];
        Utility.SetCurrentPolynomial(ecCodeWordsCount, ecCurrentPolynomial);
        Console.Write("[ERROR CORRECTION] APPLYING POLYNOMIAL: ");
        for(int i = 0; i < ecCodeWordsCount; ++i)
            Console.Write($"{ecCurrentPolynomial[i]} ");
        Console.WriteLine();

        string decodedEC = "N/A";
        Console.WriteLine($"[ERROR CORRECTION] Decoded messages: {decodedEC}");
    }
}