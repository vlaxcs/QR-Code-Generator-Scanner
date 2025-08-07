using System.Linq;

public static partial class QRCodeGenerator {
    static int[][] code;
    static int version;
    static int ECCLevel;
    static int mask;

    static Func<int, int, bool>[] masks = {
        (i, j) => (i + j) % 2 == 0,                     // Mask 0
        (i, j) => i % 2 == 0,                           // Mask 1
        (i, j) => j % 3 == 0,                           // Mask 2
        (i, j) => (i + j) % 3 == 0,                     // Mask 3
        (i, j) => (i / 2 + j / 3) % 2 == 0,             // Mask 4
        (i, j) => i * j % 2 + i * j % 3 == 0,           // Mask 5
        (i, j) => (i * j % 2 + i * j % 3) % 2 == 0,     // Mask 6
        (i, j) => ((i + j) % 2 + i * j % 3) % 2 == 0    // Mask 7
    };

    // https://www.thonky.com/qr-code-tutorial/error-correction-table
    static ECCGrouping[][] blocksInfo = {
        new ECCGrouping[]{ new ECCGrouping(7, 1, 19, 0, 0), new ECCGrouping(10, 1, 16, 0, 0), new ECCGrouping(13, 1, 13, 0, 0), new ECCGrouping(17, 1, 9, 0, 0)},
        new ECCGrouping[]{ new ECCGrouping(10, 1, 34, 0, 0), new ECCGrouping(16, 1, 28, 0, 0), new ECCGrouping(22, 1, 22, 0, 0), new ECCGrouping(28, 1, 16, 0, 0)},
        new ECCGrouping[]{ new ECCGrouping(15, 1, 55, 0, 0), new ECCGrouping(26, 1, 44, 0, 0), new ECCGrouping(18, 2, 17, 0, 0), new ECCGrouping(22, 2, 13, 0, 0)},
        new ECCGrouping[]{ new ECCGrouping(20, 1, 80, 0, 0), new ECCGrouping(18, 2, 32, 0, 0), new ECCGrouping(26, 2, 24, 0, 0), new ECCGrouping(16, 4, 9, 0, 0)},
        new ECCGrouping[]{ new ECCGrouping(26, 1, 108, 0, 0), new ECCGrouping(24, 2, 43, 0, 0), new ECCGrouping(18, 2, 15, 2, 16), new ECCGrouping(22, 2, 11, 2, 12)},
        new ECCGrouping[]{ new ECCGrouping(18, 2, 68, 0, 0), new ECCGrouping(16, 4, 27, 0, 0), new ECCGrouping(24, 4, 19, 0, 0), new ECCGrouping(28, 4, 15, 0, 0)},
        new ECCGrouping[]{ new ECCGrouping(20, 2, 78, 0, 0), new ECCGrouping(18, 4, 31, 0, 0), new ECCGrouping(18, 2, 14, 4, 15), new ECCGrouping(26, 4, 13, 1, 14)},
        new ECCGrouping[]{ new ECCGrouping(24, 2, 97, 0, 0), new ECCGrouping(22, 2, 38, 2, 39), new ECCGrouping(22, 4, 18, 2, 19), new ECCGrouping(26, 4, 14, 2, 15)},
        new ECCGrouping[]{ new ECCGrouping(30, 2, 116, 0, 0), new ECCGrouping(22, 3, 36, 2, 37), new ECCGrouping(20, 4, 16, 4, 17), new ECCGrouping(24, 4, 12, 4, 13)},
        new ECCGrouping[]{ new ECCGrouping(18, 2, 68, 2, 69), new ECCGrouping(26, 4, 43, 1, 44), new ECCGrouping(24, 6, 19, 2, 20), new ECCGrouping(28, 6, 15, 2, 16)},
        new ECCGrouping[]{ new ECCGrouping(20, 4, 81, 0, 0), new ECCGrouping(30, 1, 50, 4, 51), new ECCGrouping(28, 4, 22, 4, 23), new ECCGrouping(24, 3, 12, 8, 13)},
        new ECCGrouping[]{ new ECCGrouping(24, 2, 92, 2, 93), new ECCGrouping(22, 6, 36, 2, 37), new ECCGrouping(26, 4, 20, 6, 21), new ECCGrouping(28, 7, 14, 4, 15)},
        new ECCGrouping[]{ new ECCGrouping(26, 4, 107, 0, 0), new ECCGrouping(22, 8, 37, 1, 38), new ECCGrouping(24, 8, 20, 4, 21), new ECCGrouping(22, 12, 11, 4, 12)},
        new ECCGrouping[]{ new ECCGrouping(30, 3, 115, 1, 116), new ECCGrouping(24, 4, 40, 5, 41), new ECCGrouping(20, 11, 16, 5, 17), new ECCGrouping(24, 11, 12, 5, 13)},
        new ECCGrouping[]{ new ECCGrouping(22, 5, 87, 1, 88), new ECCGrouping(24, 5, 41, 5, 42), new ECCGrouping(30, 5, 24, 7, 25), new ECCGrouping(24, 11, 12, 7, 13)},
        new ECCGrouping[]{ new ECCGrouping(24, 5, 98, 1, 99), new ECCGrouping(28, 7, 45, 3, 46), new ECCGrouping(24, 15, 19, 2, 20), new ECCGrouping(30, 3, 15, 13, 16)},
        new ECCGrouping[]{ new ECCGrouping(28, 1, 107, 5, 108), new ECCGrouping(28, 10, 46, 1, 47), new ECCGrouping(28, 1, 22, 15, 23), new ECCGrouping(28, 2, 14, 17, 15)},
        new ECCGrouping[]{ new ECCGrouping(30, 5, 120, 1, 121), new ECCGrouping(26, 9, 43, 4, 44), new ECCGrouping(28, 17, 22, 1, 23), new ECCGrouping(28, 2, 14, 19, 15)},
        new ECCGrouping[]{ new ECCGrouping(28, 3, 113, 4, 114), new ECCGrouping(26, 3, 44, 11, 45), new ECCGrouping(26, 17, 21, 4, 22), new ECCGrouping(26, 9, 13, 16, 14)},
        new ECCGrouping[]{ new ECCGrouping(28, 3, 107, 5, 108), new ECCGrouping(26, 3, 41, 13, 42), new ECCGrouping(30, 15, 24, 5, 25), new ECCGrouping(28, 15, 15, 10, 16)},
        new ECCGrouping[]{ new ECCGrouping(28, 4, 116, 4, 117), new ECCGrouping(26, 17, 42, 0, 0), new ECCGrouping(28, 17, 22, 6, 23), new ECCGrouping(30, 19, 16, 6, 17)},
        new ECCGrouping[]{ new ECCGrouping(28, 2, 111, 7, 112), new ECCGrouping(28, 17, 46, 0, 0), new ECCGrouping(30, 7, 24, 16, 25), new ECCGrouping(24, 34, 13, 0, 0)},
        new ECCGrouping[]{ new ECCGrouping(30, 4, 121, 5, 122), new ECCGrouping(28, 4, 47, 14, 48), new ECCGrouping(30, 11, 24, 14, 25), new ECCGrouping(30, 16, 15, 14, 16)},
        new ECCGrouping[]{ new ECCGrouping(30, 6, 117, 4, 118), new ECCGrouping(28, 6, 45, 14, 46), new ECCGrouping(30, 11, 24, 16, 25), new ECCGrouping(30, 30, 16, 2, 17)},
        new ECCGrouping[]{ new ECCGrouping(26, 8, 106, 4, 107), new ECCGrouping(28, 8, 47, 13, 48), new ECCGrouping(30, 7, 24, 22, 25), new ECCGrouping(30, 22, 15, 13, 16)},
        new ECCGrouping[]{ new ECCGrouping(28, 10, 114, 2, 115), new ECCGrouping(28, 19, 46, 4, 47), new ECCGrouping(28, 28, 22, 6, 23), new ECCGrouping(30, 33, 16, 4, 17)},
        new ECCGrouping[]{ new ECCGrouping(30, 8, 122, 4, 123), new ECCGrouping(28, 22, 45, 3, 46), new ECCGrouping(30, 8, 23, 26, 24), new ECCGrouping(30, 12, 15, 28, 16)},
        new ECCGrouping[]{ new ECCGrouping(30, 3, 117, 10, 118), new ECCGrouping(28, 3, 45, 23, 46), new ECCGrouping(30, 4, 24, 31, 25), new ECCGrouping(30, 11, 15, 31, 16)},
        new ECCGrouping[]{ new ECCGrouping(30, 7, 116, 7, 117), new ECCGrouping(28, 21, 45, 7, 46), new ECCGrouping(30, 1, 23, 37, 24), new ECCGrouping(30, 19, 15, 26, 16)},
        new ECCGrouping[]{ new ECCGrouping(30, 5, 115, 10, 116), new ECCGrouping(28, 19, 47, 10, 48), new ECCGrouping(30, 15, 24, 25, 25), new ECCGrouping(30, 23, 15, 25, 16)},
        new ECCGrouping[]{ new ECCGrouping(30, 13, 115, 3, 116), new ECCGrouping(28, 2, 46, 29, 47), new ECCGrouping(30, 42, 24, 1, 25), new ECCGrouping(30, 23, 15, 28, 16)},
        new ECCGrouping[]{ new ECCGrouping(30, 17, 115, 0, 0), new ECCGrouping(28, 10, 46, 23, 47), new ECCGrouping(30, 10, 24, 35, 25), new ECCGrouping(30, 19, 15, 35, 16)},
        new ECCGrouping[]{ new ECCGrouping(30, 17, 115, 1, 116), new ECCGrouping(28, 14, 46, 21, 47), new ECCGrouping(30, 29, 24, 19, 25), new ECCGrouping(30, 11, 15, 46, 16)},
        new ECCGrouping[]{ new ECCGrouping(30, 13, 115, 6, 116), new ECCGrouping(28, 14, 46, 23, 47), new ECCGrouping(30, 44, 24, 7, 25), new ECCGrouping(30, 59, 16, 1, 17)},
        new ECCGrouping[]{ new ECCGrouping(30, 12, 121, 7, 122), new ECCGrouping(28, 12, 47, 26, 48), new ECCGrouping(30, 39, 24, 14, 25), new ECCGrouping(30, 22, 15, 41, 16)},
        new ECCGrouping[]{ new ECCGrouping(30, 6, 121, 14, 122), new ECCGrouping(28, 6, 47, 34, 48), new ECCGrouping(30, 46, 24, 10, 25), new ECCGrouping(30, 2, 15, 64, 16)},
        new ECCGrouping[]{ new ECCGrouping(30, 17, 122, 4, 123), new ECCGrouping(28, 29, 46, 14, 47), new ECCGrouping(30, 49, 24, 10, 25), new ECCGrouping(30, 24, 15, 46, 16)},
        new ECCGrouping[]{ new ECCGrouping(30, 4, 122, 18, 123), new ECCGrouping(28, 13, 46, 32, 47), new ECCGrouping(30, 48, 24, 14, 25), new ECCGrouping(30, 42, 15, 32, 16)},
        new ECCGrouping[]{ new ECCGrouping(30, 20, 117, 4, 118), new ECCGrouping(28, 40, 47, 7, 48), new ECCGrouping(30, 43, 24, 22, 25), new ECCGrouping(30, 10, 15, 67, 16)},
        new ECCGrouping[]{ new ECCGrouping(30, 19, 118, 6, 119), new ECCGrouping(28, 18, 47, 31, 48), new ECCGrouping(30, 34, 24, 34, 25), new ECCGrouping(30, 20, 15, 61, 16)},
    };

    public static QRCode Generate(string message, int errorCorrectionLevel = 1, int minVersion = 1, DataType? type = null) {

        // Find the optimal dataType or use the one passed in
        QREncodedMessage encodedMessage = new QREncodedMessage();

        if (type == null)
        {
            var types = new DataType[] { DataType.Numeric, DataType.Alphanumeric, DataType.Byte, DataType.Kanji };

            bool foundAny = false;
            foreach (var dataType in types)
            {
                try
                {
                    encodedMessage = MessageEncoder.EncodeMessage(dataType, message);
                    type = dataType;
                    foundAny = true;
                    break;

                }
                catch { }
            }

            if (!foundAny)
            {
                throw new Exception("Couldnt find a compatible encoding data type");
            }
        }
        else
        {
            encodedMessage = MessageEncoder.EncodeMessage(type.Value, message);
        }

        ECCLevel = errorCorrectionLevel;
        version = DetermineFittingVersion(encodedMessage, minVersion, type.Value) - 1;

        ECCGrouping grouping = blocksInfo[version][ECCLevel];

        byte[] dataBlocks = new byte[grouping.TotalDataBlocks];
        byte[] ECCBlocks = new byte[grouping.TotalECCBlocks];
        var galoisField = new GaloisField(grouping.ECCodewordsPerBlock);

        List<byte> bitConversion = new List<byte>();

        // Append encoding method
        AppendBits(bitConversion, (int)type.Value, 4);

        // Append bits length
        int nrOfLengthBits = GetNumberOfLengthBits(version, type.Value);
        AppendBits(bitConversion, message.Length, nrOfLengthBits);
        
        for (int i = 0; i < encodedMessage.bitsArray.Length; i++)
        {
            bitConversion.Add(encodedMessage[i]);
        }
        
        for (int i = 0; i <= 3; i++)
        {
            bitConversion.Add(0);
        }

        var numberOfBlocks = grouping.TotalDataBlocks;
        var numberOfBits = numberOfBlocks * 8;
        
        while (bitConversion.Count < numberOfBits)
        {
            bitConversion.Add(0);

            if (bitConversion.Count % 8 == 0)
            {
                break;
            }
        }

        // Transform bits to bytes
        for(int i = 0; i < bitConversion.Count / 8; i++)
        {
            dataBlocks[i] = bitConversion[i];
            
            for (int b = 0; b < 8; b++)
            {
                dataBlocks[i] <<= 1;
                dataBlocks[i] |= bitConversion[i * 8 + b];
            }
        }

        bool alternateFiller = true; // Used to alternate between 0xEC and 0x11
        for(int i = bitConversion.Count / 8; i < numberOfBlocks; i++)
        {
            if (alternateFiller)
            {
                dataBlocks[i] = 0xEC;
            }
            else
            {
                dataBlocks[i] = 0x11;
            }
            alternateFiller = !alternateFiller;
        }

        // Populate G1 type (short) blocks
        for(int i = 0; i < grouping.G1Count; i++)
        {
            var range = dataBlocks[(i * grouping.codewordsInG1)..((i + 1) * grouping.codewordsInG1)];
            var ECCBlock = galoisField.Encode(range);

            // Loop over just the error correction blocks
            for(int c = 0; c < grouping.ECCodewordsPerBlock; c++)
            {
                ECCBlocks[i * grouping.ECCodewordsPerBlock + c] = ECCBlock[grouping.codewordsInG1 + c];
            }
        }

        // Populate G2 type (long) blocks
        int G2Offset = grouping.G1Count * grouping.codewordsInG1;
        for(int i = 0; i < grouping.G2Count; i++)
        {
            var range = dataBlocks[(G2Offset + i * grouping.codewordsInG2)..(G2Offset + (i + 1) * grouping.codewordsInG2)];
            var ECCBlock = galoisField.Encode(range);

            // Loop over just the error correction blocks
            for(int c = 0; c < grouping.ECCodewordsPerBlock; c++)
            {
                ECCBlocks[(grouping.G1Count + i) * grouping.ECCodewordsPerBlock + c] = ECCBlock[grouping.codewordsInG2 + c];
            }
        }

        var result = new byte[dataBlocks.Length + ECCBlocks.Length];
        var totalGroupings = grouping.G1Count + grouping.G2Count;

        // Interweave G1 and G2, first G1 length:
        int crntIndex = 0;
        for(int i = 0; i < grouping.codewordsInG1; i++)
        {
            for(int j = 0; j < totalGroupings; j++)
            {
                if(j < grouping.G1Count)
                {
                    result[crntIndex++] = dataBlocks[j * grouping.codewordsInG1 + i];
                }
                else
                {
                    result[crntIndex++] = dataBlocks[G2Offset + (j - grouping.G1Count) * grouping.codewordsInG2 + i];
                }
            }
        }
        
        // Put the rest of G2s
        for (int i = grouping.codewordsInG1; i < grouping.codewordsInG2; i++)
        {
            for (int j = 0; j < grouping.G2Count; j++)
            {
                result[crntIndex++] = dataBlocks[G2Offset + j * grouping.codewordsInG2 + i];
            }
        }

        // Interweave ECC Blocks
        for(int i = 0; i < grouping.ECCodewordsPerBlock; i++)
        {
            for(int j = 0; j < totalGroupings; j++)
            {
                result[crntIndex++] = ECCBlocks[j * grouping.ECCodewordsPerBlock + i];
            }
        }

        version += 1;
        return Generate(result, version, errorCorrectionLevel);
    }

    static QRCode Generate(byte[] dataBlocks, int qrVersion, int errorCorrectionLevel) {
        version = qrVersion;
        version = qrVersion;
        ECCLevel = errorCorrectionLevel;

        code = new int[Utility.SizeForVersion(version)][];
        for(int i = 0; i < code.Length; i++)
        {
            code[i] = new int[Utility.SizeForVersion(version)];
        }

        PutStripes();
        PutBlocks();
        PutAligmentPoints();
        ApplyVersionBits();
        SetAllDataBlocks(dataBlocks);
        PlaceBestMask();

        return new QRCode(code);
    }

    static int DetermineFittingVersion(QREncodedMessage message, int minimum, DataType type) {
        var minBlocks = (int)Math.Ceiling(message.bitsArray.Length * 1.0f / 8);
        
        for (int i = minimum; i <= 40; i++)
        {
            int addedLength = (int)Math.Ceiling((4 + GetNumberOfLengthBits(i - 1, type) * 1.0f) / 8);
            if (blocksInfo[i - 1][ECCLevel].TotalDataBlocks >= minBlocks + addedLength)
            {
                version = i;
                return version;
            }
        }

        throw new Exception("There is no QR version that this message can fit into");
    }

    static int GetNumberOfLengthBits(int version, DataType type) {
        int nrOfLengthBits = 0;
        
        switch (type)
        {
            case DataType.Numeric:
                if (version < 9) nrOfLengthBits = 10;
                else if (version < 26) nrOfLengthBits = 12;
                else nrOfLengthBits = 14;
                break;
            case DataType.Alphanumeric:
                if (version < 9) nrOfLengthBits = 9;
                else if (version < 26) nrOfLengthBits = 11;
                else nrOfLengthBits = 13;
                break;
            case DataType.Byte:
                if (version < 9) nrOfLengthBits = 8;
                else if (version < 26) nrOfLengthBits = 16;
                else nrOfLengthBits = 16;
                break;
            default:
                throw new Exception("Kanji not supported");
        }

        return nrOfLengthBits;
    }

    static void AppendBits(List<byte> bits, int nr, int nrOfBits)
    {
        int[] v = new int[nrOfBits];
        int ct = 0;
        
        while (nr != 0)
        {
            v[ct++] = nr % 2;
            nr /= 2;
        }
        
        while (ct < nrOfBits)
        {
            v[ct++] = 0;
        }

        for(int i = v.Length - 1; i >= 0; i--)
        {
            bits.Add((byte)v[i]);
        }
    }

    static void PlaceBestMask()
    {
        int MinScore = 999999999;
        int bestMask = -1;
        
        for (int i = 0; i <= 7; i++)
        {
            PutMaskBits(ECCLevel, i);
            ApplyMask(i);
            int penaltyScore = CalculatePenaltyScore();

            if (penaltyScore < MinScore)
            {
                MinScore = penaltyScore;
                bestMask = i;
            }

            ApplyMask(i);
        }

        mask = bestMask;
        PutMaskBits(ECCLevel ^ 1, mask);
        ApplyMask(mask);
    }
    
    static void ApplyVersionBits()
    {
        if (version < 7)
        {
            return;
        }
        int ECCVersion = GetVersionBits(version);

        for (int j = 0; j < 6; j++)
        {
            for (int i = 0; i < 3; i++)
            {
                code[code.Length - 11 + (2 - i)][5 - j] = ECCVersion >> (17 - j * 3 - i) & 1;
                code[5 - j][code.Length - 11 + (2 - i)] = ECCVersion >> (17 - j * 3 - i) & 1;
            }
        }
    }
    
    static void PutAligmentPoints()
    {
        var aligmentPoints = GetAlignmentPoints();
        for (int k = 0; k < aligmentPoints.Length; k += 2)
        {
            var allx = aligmentPoints[k];
            var ally = aligmentPoints[k + 1];

            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    code[allx + i][ally + j] = 1;
                }
            }

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    code[allx + i][ally + j] = 0;
                }
            }

            code[allx][ally] = 1;
        }
    }
    
    static void PutBlocks()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                code[i][j] = code[code.Length - 1 - i][j] = code[i][code.Length - 1 - j] = 1;
            }
        }

        for (int i = 1; i < 6; i++)
        {
            for (int j = 1; j < 6; j++)
            {
                code[i][j] = code[code.Length - 1 - i][j] = code[i][code.Length - 1 - j] = 0;
            }
        }

        for (int i = 2; i < 5; i++)
        {
            for (int j = 2; j < 5; j++)
            {
                code[i][j] = code[code.Length - 1 - i][j] = code[i][code.Length - 1 - j] = 1;
            }
        }
    }
    
    static void PutStripes()
    {
        for (int i = 0; i < code.Length; i++)
        {
            code[6][i] = code[i][6] = i & 1 ^ 1;
        }
    }

    static void PutMaskBits(int ECCLevel, int mask)
    {
        int maskBits = GetMaskBits(ECCLevel, mask);

        for(int j = 0; j <= 5; j++)
        {
            code[8][j] = (maskBits >> (14 - j)) & 1;
        }
        
        for (int j = 6; j <= 7; j++)
        {
            code[8][j + 1] = (maskBits >> (14 - j)) & 1;
        }

        code[7][8] = (maskBits >> (14 - 8)) & 1;
        
        for (int i = 0; i <= 5; i++)
        {
            code[i][8] = (maskBits >> (i)) & 1;
        }
        
        for (int i = 0; i <= 6; i++)
        {
            code[code.Length - i - 1][8] = (maskBits >> (14 - i)) & 1;
        }
        
        for (int j = 0; j <= 7; j++)
        {
            code[8][code.Length - 1 - j] = (maskBits >> j) & 1;
        }
    }

    static void SetAllDataBlocks(byte[] totalData) {
        var lengthened = new byte[totalData.Length + 1];
        totalData.CopyTo(lengthened, 0);
        lengthened[totalData.Length] = 0;

        int nr = 0;
        for(int j = code.Length - 1; j >= 1; j -= 2)
        {
            int nj = j - (j > 6 ? 1 : 2);
            
            if (j % 4 != 2)
            {
                for (int i = code.Length - 1; i >= 0; i--)
                {
                    if (IsData(i, nj + 1))
                    {
                        code[i][nj + 1] = (lengthened[nr / 8] >> (7 - (nr % 8))) & 1;
                        nr++;
                    }

                    if (IsData(i, nj))
                    {
                        code[i][nj] = (lengthened[nr / 8] >> (7 - (nr % 8))) & 1;
                        nr++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < code.Length; i++)
                {
                    if (IsData(i, nj + 1))
                    {
                        code[i][nj + 1] = (lengthened[nr / 8] >> (7 - (nr % 8))) & 1;
                        nr++;
                    }
                    if (IsData(i, nj))
                    {
                        code[i][nj] = (lengthened[nr / 8] >> (7 - (nr % 8))) & 1;
                        nr++;
                    }
                }
            }
        }
    }
    
    static bool IsData(int x, int y)
    {
        int maxWidth = Utility.SizeForVersion(version);
        int[] alligmentPoints = GetAlignmentPoints();

        if (x < 9 && y < 9) return false;
        if (x < 9 && y > maxWidth - 9) return false;
        if (x > maxWidth - 9 && y < 9) return false;
        if (version >= 7 && x < 7 && y > maxWidth - 12) return false;
        if (version >= 7 && y < 7 && x > maxWidth - 12) return false;
        if (x == 6 || y == 6) return false;

        for (int i = 0; i < alligmentPoints.Length; i += 2)
        {
            if (Math.Abs(alligmentPoints[i] - x) < 3 && Math.Abs(alligmentPoints[i + 1] - y) < 3)
            {
                return false;
            }
        }

        return true;
    }
    
    static int[] GetAlignmentCoords()
    {
        if (version <= 1)
        {
            return new int[0];
        }

        int num = (version / 7) + 2;
        int[] result = new int[num];
        result[0] = 6;

        if (num == 1)
        {
            for (int i = 0; i < num; i++)
            {
                result[i] = version * 4 + 16 - result[i];
            }
            return result;
        }

        result[num - 1] = 4 * version + 10;

        if (num == 2)
        {
            for (int i = 0; i < num; i++)
            {
                result[i] = version * 4 + 16 - result[i];
            }

            return result;
        }

        result[num - 2] = 2 * ((result[0] + result[num - 1] * (num - 2)) / ((num - 1) * 2));

        if (num == 3)
        {
            for (int i = 0; i < num; i++)
            {
                result[i] = version * 4 + 16 - result[i];
            }
            return result;
        }

        int step = result[num - 1] - result[num - 2];

        for (int i = num - 3; i > 0; i--)
        {
            result[i] = result[i + 1] - step;
        }
        
        for (int i = 0; i < num; i++)
        {
            result[i] = version * 4 + 16 - result[i];
        }

        return result;
    }
    
    static int[] GetAlignmentPoints()
    {
        int[] AlignmentCoords = GetAlignmentCoords();

        if (AlignmentCoords.Length == 0)
        {
            return new int[0];
        }

        int[] ans = new int[2 * AlignmentCoords.Length * AlignmentCoords.Length - 6];
        int nr = 0;

        for (int i = 0; i < AlignmentCoords.Length; i++)
        {
            for (int j = 0; j < AlignmentCoords.Length; j++)
            {
                if (i == AlignmentCoords.Length - 1 && j == AlignmentCoords.Length - 1)
                    continue;
                if (i == 0 && j == AlignmentCoords.Length - 1)
                    continue;
                if (j == 0 && i == AlignmentCoords.Length - 1)
                    continue;

                ans[nr++] = AlignmentCoords[i];
                ans[nr++] = AlignmentCoords[j];
            }
        }

        return ans;
    }
    
    static int GetMaskBits(int ECCLevel, int maskPattern)
    {
        int nr = (ECCLevel << 3) + maskPattern;
        int initnr = nr;
        nr <<= 10;
        int gen = 0x0537; // 10100110111 is the generator=0x0537
        int initGen = gen;
        while (nr.MostSignificantBit() >= 1024)
        {
            while (gen.MostSignificantBit() < nr.MostSignificantBit())
            {
                gen <<= 1;
            }
            nr ^= gen;
            gen = initGen;
        }
        nr += initnr << 10;
        nr ^= 0x5412;
        return nr;
    }

    static void ApplyMask(int mask)
    {
        for(int i = 0; i < code.Length; i++)
        {
            for(int j = 0; j < code.Length; j++)
            {
                if(!IsData(i, j) || !masks[mask](i, j)) continue;
                code[i][j] ^= 1;
            }
        }
    }
    
    static int GetVersionBits(int Version)
    {
        if (Version < 7 || Version > 40)
        {
            throw new Exception("Version is not good");
        }

        int initnr = Version;
        Version <<= 12;
        int gen = 0x1F25; // 1 1111 0010 0101 is the generator=0x1F25
        int initGen = gen;

        while (Version.MostSignificantBit() >= 4096)
        {
            while (gen.MostSignificantBit() < Version.MostSignificantBit())
            {
                gen <<= 1;
            }

            Version ^= gen;
            gen = initGen;
        }

        Version |= initnr << 12;
        return Version;
    }


    static int CalculatePenaltyScore() {
        int consecutives = 0;
        int score = 0;

        // Calculate hroziontal consecutives penalties
        for(int i = 0; i < code.Length; i++) {
            if (consecutives == 5)
            {
                score += 3;
            }
            else if (consecutives > 5)
            {
                score++;
            }

            consecutives = 1;

            for (int j = 1; j < code.Length; j++)
            {
                if (code[i][j] == code[i][j - 1])
                {
                    consecutives++;
                }
                else
                {
                    if (consecutives == 5)
                    {
                        score += 3;
                    }
                    else if (consecutives > 5)
                    {
                        score += 3 + consecutives - 5;
                    }
                    consecutives = 1;
                }
            }
        }
        
        // Calculate vertical consecutives penalties
        for (int i = 0; i < code.Length; i++)
        {
            if (consecutives == 5)
            {
                score += 3;
            }
            else if (consecutives > 5)
            {
                score++;
            } 
            consecutives = 1;

            for (int j = 1; j < code.Length; j++)
            {
                if (code[j][i] == code[j - 1][i])
                {
                    consecutives++;
                }
                else
                {
                    if (consecutives == 5)
                    {
                        score += 3;
                    }
                    else if (consecutives > 5)
                    {
                        score += 3 + consecutives - 5;
                    }
                    consecutives = 1;
                }
            }
        }

        // Calculate 2x2 penalties
        for(int i = 1; i < code.Length; i++)
        {   
            for (int j = 1; j < code.Length; j++)
            {
                if(code[i][j] == code[i][j - 1] && code[i][j - 1] == code[i - 1][j - 1] && code[i - 1][j - 1] == code[i - 1][j])
                    score += 3;
            }
        }

        // Horizontal finder-like penalties
        for(int i = 0; i < code.Length; i++)
        {
            int nr = 0, mod = 1 << 11;

            for (int j = 0; j < 11; j++)
            {
                nr = nr * 2 + code[i][j];
            }
            
            if (nr == 1488 || nr == 93)
            {
                score += 40;
            }

            for (int j = 11; j < code.Length; j++)
            {
                nr = nr % mod * 2 + code[i][j];
                
                if (nr == 1488 || nr == 93)
                {
                    score += 40;
                }
            }
        }
        
        // Vertical finder-like penalties
        for (int i = 0; i < code.Length; i++)
        {
            int nr = 0, mod = 1 << 11;

            for (int j = 0; j < 11; j++)
            {
                nr = nr * 2 + code[j][i];
            }

            if (nr == 1488 || nr == 93)
            {
                score += 40;
            }

            for (int j = 11; j < code.Length; j++)
            {
                nr = nr % mod * 2 + code[j][i];

                if (nr == 1488 || nr == 93)
                {
                    score += 40;
                }
            }
        }

        // Dark/light balance penalties
        int darkModules = 0;
        for (int i = 0; i < code.Length; i++)
        {
            for (int j = 0; j < code.Length; j++)
            {
                darkModules += code[i][j];
            }
        }

        int totalModules = code.Length * code.Length;
        int m = totalModules / (darkModules);
        int intervalStart = m / 5 * 5;
        int intervalEnd = (m / 5 + 1) * 5;

        intervalStart -= 50; intervalEnd -= 50;
        intervalStart = Math.Abs(intervalStart);
        intervalEnd = Math.Abs(intervalEnd);
        intervalStart /= 5; intervalEnd /= 5;

        score += Math.Min(intervalStart, intervalEnd) * 10;

        return score;
    }
}