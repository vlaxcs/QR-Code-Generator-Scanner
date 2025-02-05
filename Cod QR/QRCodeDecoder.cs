using System.Text;
using System;
using System.ComponentModel.DataAnnotations;

public static class QRCodeDecoder {
    static readonly int[][] blocksByECL = {
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

    public static int[] DecodeQR(QRCode code) {
        // Apply error correction
        Console.WriteLine($"Version: {code.version} | Error correction level: {code.errorCorrectionLevel}");
        int nsym = blocksByECL[code.version - 1][code.errorCorrectionLevel];
        var gf = new GaloisField(nsym);

        var data = gf.Decode(code.data);

        List<byte> bits = new List<byte>();
        for(int i = 0; i < data.Length; i++)
            for(int j = 7; j >= 0; j--)
                bits.Add((byte)((data[i] >> j) & 1));

        int encodingRange = Utility.ComputeEncodingRange(code.datatype, code.version);

        int messageLen = 0;
        for(int i = 4; i < 4 + encodingRange; i++)
            messageLen += bits[i] * (1 << (encodingRange - i + 3));

        int[] message = new int[messageLen];
        int blockSize = 8;
        var sb = new StringBuilder();
        for(int i = 0; i < message.Length; i++) {
            message[i] = 0;
            for(int b = 0; b < blockSize; b++)
                message[i] |= (bits[encodingRange + 4 + i * blockSize + b] & 1) << (blockSize - b - 1);
            
            sb.Append((char)message[i]);
        }

        return message;
    }
}