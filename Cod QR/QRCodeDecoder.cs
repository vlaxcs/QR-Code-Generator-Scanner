using System.Text;
using System;
using System.ComponentModel.DataAnnotations;

public static class QRCodeDecoder {
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

    public static int[] DecodeQR(QRCode code) {
        // Apply error correction
        int nsym = blocksByECL[code.version - 1][code.errorCorrectionLevel - 1];
        var gf = new GaloisField(nsym);

        var data = gf.Decode(code.data);

        List<byte> bits = new List<byte>();
        for(int i = 0; i < data.Length; i++) {
            for(int j = 7; j >= 0; j--) {
                bits.Add((byte)((data[i] >> j) & 1));
            }
        }

        int encodingRange = Utility.ComputeEncodingRange(code.datatype, code.version);

        int messageLen = 0;
        for(int i = 4; i < 4 + encodingRange; i++)
            messageLen += bits[i] * (1 << (encodingRange - i + 3));

        int[] message = new int[messageLen];
        int blockSize = 8;
        var sb = new StringBuilder();
        for(int i = 0; i < message.Length; i++) {
            message[i] = 0;
            for(int b = 0; b < blockSize; b++) {
                message[i] |= (bits[encodingRange + 4 + i * blockSize + b] & 1) << (blockSize - b - 1);
            }
            sb.Append((char)message[i]);
        }

        return message;
    }
}
