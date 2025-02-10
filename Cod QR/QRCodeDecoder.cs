using System.Text;

public static partial class QRCodeDecoder {
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

    public static QRMessage DecodeQR(QRCode code) {
        // Apply error correction
        var grouping = blocksInfo[code.Version - 1][code.ErrorCorrectionLevel];

        var totalGroups = grouping.G1Count + grouping.G2Count;
        byte[][] groupedData = new byte[totalGroups][];
        for(int g = 0; g < groupedData.Length; g++) {
            if(g < grouping.G1Count) {
                groupedData[g] = new byte[grouping.codewordsInG1 + grouping.ECCodewordsPerBlock];
            } else {
                groupedData[g] = new byte[grouping.codewordsInG2 + grouping.ECCodewordsPerBlock];
            }
        }

        for(int i = 0; i < totalGroups; i++) {
            for(int j = 0; j < grouping.codewordsInG1; j++) {
                groupedData[i][j] = code.data[j * totalGroups + i];
            }
        }
        int offset = totalGroups * grouping.codewordsInG1;
        for(int i = 0; i < grouping.G2Count; i++) {
            for(int j = 0; j < grouping.codewordsInG2 - grouping.codewordsInG1; j++) {
                groupedData[i + grouping.G1Count][grouping.codewordsInG1 + j] = code.data[offset + j + i];
            }
        }

        // Put ECC blocks for each group
        offset = grouping.G1Count * grouping.codewordsInG1 + grouping.G2Count * grouping.codewordsInG2;
        for(int i = 0; i < totalGroups; i++) {
            for(int j = 0; j < grouping.ECCodewordsPerBlock; j++) {
                if(i < grouping.G1Count) {
                    groupedData[i][grouping.codewordsInG1 + j] = code.data[offset + j * totalGroups + i];
                } else {
                    groupedData[i][grouping.codewordsInG2 + j] = code.data[offset + j * totalGroups + i];
                }
            }
        }

        var gf = new GaloisField(grouping.ECCodewordsPerBlock);

        for(int i = 0; i < totalGroups; i++) {
            groupedData[i] = gf.Decode(groupedData[i]);
        }

        //Console.WriteLine($"GROUPED:");
        //for(int i = 0; i < totalGroups; i++) {
        //    Console.Write($"G{i}: ");
        //    PrintHexa(groupedData[i]);
        //    Console.WriteLine();
        //}
        //Console.WriteLine();


        List<byte> dataReconstruction = new List<byte>();
        for(int i = 0; i < totalGroups; i++) {
            for(int j = 0; j < groupedData[i].Length; j++) {
                dataReconstruction.Add(groupedData[i][j]);
            }
        }
        var data = dataReconstruction.ToArray();

        //Console.WriteLine($"RECONSTRUCTION:");
        //for(int i = 0; i < data.Length; i++) {
        //    Console.Write($"{Convert.ToString(data[i], 16).PadLeft(2, '0').ToUpper()} ");
        //}
        //Console.WriteLine();


        List<int> bits = new List<int>();
        for(int i = 0; i < data.Length; i++) {
            for(int j = 7; j >= 0; j--) {
                bits.Add((data[i] >> j) & 1);
            }
        }

        int encodingRange = Utility.ComputeEncodingRange(code.datatype, code.Version);

        int messageLen = 0;
        for(int i = 4; i < 4 + encodingRange; i++)
            messageLen += bits[i] * (1 << (encodingRange - i + 3));

        QRMessage message = new QRMessage();

        switch(code.datatype) {
            case DataType.Numeric:
                message = DecodeNumericMessage(bits.ToArray(), messageLen, encodingRange);
                break;
            case DataType.Alphanumeric:
                message = DecodeAlphanumericMessage(bits.ToArray(), messageLen, encodingRange);
                break;
            case DataType.Byte:
                message = DecodeByteMessage(bits.ToArray(), messageLen, encodingRange);
                break;
            case DataType.Kanji:
                throw new Exception("Kanji decoding not supported");
        }

        Console.WriteLine($"QR {message.type} message: {message}");

        return message;
    }

    static void PrintHexa(byte[] arr) {
        for(int i = 0; i < arr.Length; i++) {
            Console.Write($"{Convert.ToString(arr[i], 16).PadLeft(2, '0').ToUpper()} ");
        }
    }

    static QRMessage DecodeNumericMessage(int[] bits, int messageLen, int encodingRange) {
        encodingRange = 4 + encodingRange;

        int blockSize = 10;

        int[] message = new int[messageLen];
        for(int i = 0; i < message.Length / 3; i++) {
            message[i * 3] = 0;
            for(int b = 0; b < blockSize; b++) {
                message[i * 3] |= (bits[encodingRange + i * blockSize + b] & 1) << (blockSize - b - 1);
            }

            message[i * 3 + 2] = message[i * 3] % 10;
            message[i * 3 + 1] = message[i * 3] / 10 % 10;
            message[i * 3] = message[i * 3] / 100;
        }


        if(message.Length % 3 != 0) {
            int remainingBlock = 0;
            for(int b = 0; b < blockSize - (3 - message.Length % 3) * 3; b++) {
                remainingBlock |= (bits[encodingRange + (message.Length / 3) * blockSize + b] & 1) << (blockSize - b - 1 - (3 - message.Length % 3) * 3);
            }
            for(int i = 0; remainingBlock != 0 && i < message.Length; i++, remainingBlock /= 10) {
                message[message.Length - i - 1] = remainingBlock % 10;
            }
        }

        //Console.WriteLine("Message: ");
        //for(int i = 0; i < message.Length; i++) {
        //    Console.Write($"{message[i]}");
        //}

        return new QRMessage(DataType.Numeric, message);
    }

    static QRMessage DecodeAlphanumericMessage(int[] bits, int messageLen, int encodingRange) {
        encodingRange = 4 + encodingRange;

        int blockSize = 11;

        int[] message = new int[messageLen];
        for(int i = 0; i < message.Length / 2; i++) {
            message[i * 2] = 0;
            for(int b = 0; b < blockSize; b++) {
                message[i * 2] |= (bits[encodingRange + i * blockSize + b] & 1) << (blockSize - b - 1);
            }

            message[i * 2 + 1] = message[i * 2] % 45;
            message[i * 2] = message[i * 2] / 45;
        }

        if(messageLen % 2 == 1) {
            message[message.Length - 1] = 0;
            int i = message.Length / 2;
            for(int b = 0; b < 6; b++) {
                message[message.Length - 1] |= (bits[encodingRange + i * blockSize + b] & 1) << (6 - b - 1);
            }
        }

        return new QRMessage(DataType.Alphanumeric, message);
    }

    static QRMessage DecodeByteMessage(int[] bits, int messageLen, int encodingRange) {
        int[] message = new int[messageLen];
        int blockSize = 8;
        for(int i = 0; i < message.Length; i++) {
            message[i] = 0;
            for(int b = 0; b < blockSize; b++) {
                message[i] |= (bits[encodingRange + 4 + i * blockSize + b] & 1) << (blockSize - b - 1);
            }
        }

        return new QRMessage(DataType.Byte, message);
    }
}