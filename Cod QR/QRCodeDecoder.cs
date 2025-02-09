using System.Text;

public static partial class QRCodeDecoder {
    static readonly int[][] nrOfECBlocks = {
        new int[]{7, 10, 13, 17},
        new int[]{10, 16, 22, 28},
        new int[]{15, 26, 18, 22},
        new int[]{20, 18, 26, 16},
        new int[]{26, 24, 18, 22},
        new int[]{18, 16, 24, 28},
        new int[]{20, 18, 18, 26},
        new int[]{24, 22, 22, 26},
        new int[]{30, 22, 20, 24},
        new int[]{18, 26, 24, 28},
        new int[]{20, 30, 28, 24},
        new int[]{24, 22, 26, 28},
        new int[]{26, 22, 24, 22},
        new int[]{30, 24, 20, 24},
        new int[]{22, 24, 30, 24},
        new int[]{24, 28, 24, 30},
        new int[]{28, 28, 28, 28},
        new int[]{30, 26, 28, 28},
        new int[]{28, 26, 26, 26},
        new int[]{28, 26, 30, 28},
        new int[]{28, 26, 28, 30},
        new int[]{28, 28, 30, 24},
        new int[]{30, 28, 30, 30},
        new int[]{30, 28, 30, 30},
        new int[]{26, 28, 30, 30},
        new int[]{28, 28, 28, 30},
        new int[]{30, 28, 30, 30},
        new int[]{30, 28, 30, 30},
        new int[]{30, 28, 30, 30},
        new int[]{30, 28, 30, 30},
        new int[]{30, 28, 30, 30},
        new int[]{30, 28, 30, 30},
        new int[]{30, 28, 30, 30},
        new int[]{30, 28, 30, 30},
        new int[]{30, 28, 30, 30},
        new int[]{30, 28, 30, 30},
        new int[]{30, 28, 30, 30},
        new int[]{30, 28, 30, 30},
        new int[]{30, 28, 30, 30},
        new int[]{30, 28, 30, 30}
    };

    public static QRMessage DecodeQR(QRCode code) {
        // Apply error correction
        int nsym = nrOfECBlocks[code.Version - 1][code.ErrorCorrectionLevel];

        /*for(int bi = 1; bi < 2000; bi++) {
            try {
                var test = new GaloisField(bi);

                Console.WriteLine(code.data.Length);
                Console.ReadLine();
                for(int di = 0; di < code.data.Length; di++) {
                    Console.WriteLine($"{code.data[di]:X}");
                }

                var data = test.Decode(code.data);

                List<int> bits = new List<int>();
                for(int i = 0; i < data.Length; i++) {
                    for(int j = 7; j >= 0; j--) {
                        bits.Add((data[i] >> j) & 1);
                    }
                }

                int encodingRange = Utility.ComputeEncodingRange(code.datatype, code.version);

                int messageLen = 0;
                for(int i = 4; i < 4 + encodingRange; i++)
                    messageLen += bits[i] * (1 << (encodingRange - i + 3));

                int[] message = null;

                Console.WriteLine($"NSYM:{bi} (supposed to be {nsym})\nDatatype: {code.datatype}");

                switch(code.datatype) {
                    case DataType.Numeric:
                        throw new Exception("UNIMPLEMENTED decoding data type");
                    case DataType.Alphanumeric:
                        message = DecodeAlphanumericMessage(bits.ToArray(), messageLen, encodingRange);
                        break;
                    case DataType.Byte:
                        message = DecodeByteMessage(bits.ToArray(), messageLen, encodingRange);
                        break;
                    case DataType.Kanji:
                        throw new Exception("UNIMPLEMENTED decoding data type");
                }
            } catch(Exception e) {
                
            }
        }

        return new int[] { 1 };*/

        var gf = new GaloisField(nsym);

        var data = gf.Decode(code.data);

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