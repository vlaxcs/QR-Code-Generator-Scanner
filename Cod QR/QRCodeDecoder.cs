using System.Text;
using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Xsl;

public static partial class QRCodeDecoder {
    static readonly int[][] blocksByECL = {
        new int[]{7, 10, 13, 17},
        new int[]{10, 16, 22, 28},
        new int[]{15, 26, 36, 44},
        new int[]{20, 36, 52, 64},
        new int[]{26, 48, 72, 88},
        new int[]{36, 64, 96, 112},
        new int[]{40, 72, 108, 130},
        new int[]{48, 88, 132, 156},
        new int[]{60, 110, 160, 192},
        new int[]{72, 130, 192, 224},
        new int[]{80, 150, 224, 264},
        new int[]{96, 176, 260, 308},
        new int[]{104, 198, 288, 352},
        new int[]{120, 216, 320, 384},
        new int[]{132, 240, 360, 432},
        new int[]{144, 280, 408, 480},
        new int[]{168, 308, 448, 532},
        new int[]{180, 338, 504, 588},
        new int[]{196, 364, 546, 650},
        new int[]{224, 416, 600, 700},
        new int[]{224, 442, 644, 750},
        new int[]{252, 476, 690, 816},
        new int[]{270, 504, 750, 900},
        new int[]{300, 560, 810, 960},
        new int[]{312, 588, 870, 1050},
        new int[]{336, 644, 952, 1110},
        new int[]{360, 700, 1020, 1200},
        new int[]{390, 728, 1050, 1260},
        new int[]{420, 784, 1140, 1350},
        new int[]{450, 812, 1200, 1440},
        new int[]{480, 868, 1290, 1530},
        new int[]{510, 924, 1350, 1620},
        new int[]{540, 980, 1440, 1710},
        new int[]{570, 1036, 1530, 1800},
        new int[]{570, 1064, 1590, 1890},
        new int[]{600, 1120, 1680, 1980},
        new int[]{630, 1204, 1770, 2100},
        new int[]{660, 1260, 1860, 2220},
        new int[]{720, 1316, 1950, 2310},
        new int[]{750, 1372, 2040, 2430}
    };

    public static QRMessage DecodeQR(QRCode code) {
        // Apply error correction
        int nsym = blocksByECL[code.version - 1][code.errorCorrectionLevel];

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

        int encodingRange = Utility.ComputeEncodingRange(code.datatype, code.version);

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
