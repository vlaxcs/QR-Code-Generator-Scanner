using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

public class QRCode {
    readonly int[][] code;
    public readonly int version, errorCorrectionLevel;

    //For Debuging Propurses
    public int maskUsed;
    public readonly byte[] data; 

    // Data masking
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

    public DataType datatype;
    public enum DataType {
        None = 0,
        Numeric = 1,
        Alphanumeric = 2,
        Byte = 4,
        Kanji = 8
    }

    int[][] blocksByECC = {
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

    public QRCode(int[][] mat) {
        code = mat;
        if((code.Length - 17) % 4 != 0) {
            throw new Exception("Size is invalid");
        }

        version = (code.Length - 17) / 4;
        if(version < 1 || version > 40) {
            throw new Exception("Version is invalid");
        }

        int mask1 = 0;
        int mask2 = 0;
        for(int j = 0; j <= 5; j++) {
            mask1 |= code[8][j] << (14 - j);
        }
        for(int j = 6; j <= 7; j++) {
            mask1 |= code[8][j + 1] << (14 - j);
        }
        mask1 |= code[7][8] << (14 - 8);
        for(int i = 0; i <= 5; i++) {
            mask1 |= code[i][8] << (i);
        }
        for(int i = 0; i <= 6; i++) {
            mask2 |= code[code.Length - i - 1][8] << (14 - i);

        }
        for(int j = 0; j <= 7; j++) {
            mask2 |= code[8][code.Length - 1 - j] << j;
        }
        mask1 = GetClosestDataEC(mask1);
        mask2 = GetClosestDataEC(mask2);
        if(mask1 != mask2) throw new Exception("Mask bits dont match");
        maskUsed = (mask1 >> 10) & 7 ^ 5;
        errorCorrectionLevel = (mask1 >> 13) ^ 2;

        //TODO:Find Something more elegant
        ApplyMask(maskUsed);
        data = GetAllDataBlocks();

        // Apply error correction
        int nsym = blocksByECC[version - 1][errorCorrectionLevel - 1];
        var gf = new GaloisField(nsym);

        data = gf.Decode(data);

        List<byte> bits = new List<byte>();
        for(int i = 0; i < data.Length; i++) {
            for(int j = 7; j >= 0; j--) {
                bits.Add((byte)((data[i] >> j) & 1));
            }
        }

        int messageLen = 0;
        for(int i = 4; i < 12; i++) {
            messageLen |= (bits[i] & 1) << (11 - i);
        }
        int[] message = new int[messageLen];
        int blockSize = 8;
        var sb = new StringBuilder();
        for(int i = 0; i < message.Length; i++) {
            message[i] = 0;
            for(int b = 0; b < blockSize; b++) {
                message[i] |= (bits[12 + i * blockSize + b] & 1) << (blockSize - b - 1);
            }
            sb.Append((char)message[i]);
        }
        Console.WriteLine(sb.ToString());
    }

    public void Print(bool masked = true) {
        if(masked) ApplyMask(maskUsed);

        Console.BackgroundColor = ConsoleColor.White;
        Console.WriteLine(new string(' ', code.Length * 2 + 4));

        for(int i = 0; i < code.Length; i++) {
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write("  ");

            for(int j = 0; j < code.Length; j++) {
                if(code[i][j] == 1) Console.BackgroundColor = ConsoleColor.Black;
                else Console.BackgroundColor = ConsoleColor.White;
                Console.Write("  ");
                Console.BackgroundColor = ConsoleColor.Black;
            }
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write("  ");


            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
        }

        Console.BackgroundColor = ConsoleColor.White;
        Console.WriteLine(new string(' ', code.Length * 2 + 4));

        Console.BackgroundColor = ConsoleColor.Black;
        Console.WriteLine("           ");

        if(masked) ApplyMask(maskUsed);
    }
    public int GetClosestDataEC(int DataEC) {
        int ans = 0;
        int dist = 15;
        int nrans = 0;
        for(int i = 0; i <= 31; i++) {
            int nr = 0;
            int ValidMask = GetMaskBits(i >> 3, i & 7);
            for(int j = 0; j <= 31; j++) {
                if((ValidMask & (1 << j)) != (DataEC & (1 << j))) {
                    nr++;
                }
            }
            if(nr < dist) {
                dist = nr;
                ans = ValidMask;
                nrans = 1;
            } else if(nr == dist) {
                nrans++;
            }
        }

        if(nrans != 1) {
            throw new Exception("There are 2 codes at same distance");
            //TODO: IMPLEMENT SOMETHING FOR 2 CODES AT SAME DISTANCE:)
        }
        return ans;
    }
    public bool IsData(int x, int y) {
        int maxWidth = SizeForVersion(version);
        int[] alligmentPoints = GetAlignmentPoints();

        if(x < 9 && y < 9) return false;

        if(x < 9 && y > maxWidth - 9) return false;
        if(x > maxWidth - 9 && y < 9) return false;

        if(version >= 7 && x < 7 && y > maxWidth - 12) return false;
        if(version >= 7 && y < 7 && x > maxWidth - 12) return false;

        if(x == 6 || y == 6) return false;
        for(int i = 0; i < alligmentPoints.Length; i += 2) {
            if(Math.Abs(alligmentPoints[i] - x) < 3 && Math.Abs(alligmentPoints[i + 1] - y) < 3)
                return false;
        }
        return true;
    }

    int SizeForVersion(int version) => version * 4 + 17;

    int[] GetAlignmentCoords() {
        if(version <= 1) {
            return new int[0];
        }
        int num = (version / 7) + 2;
        int[] result = new int[num];
        result[0] = 6;
        if(num == 1) {
            for(int i = 0; i < num; i++) {
                result[i] = version * 4 + 16 - result[i];
            }
            return result;
        }
        result[num - 1] = 4 * version + 10;
        if(num == 2) {
            for(int i = 0; i < num; i++) {
                result[i] = version * 4 + 16 - result[i];
            }
            return result;
        }
        result[num - 2] = 2 * ((result[0] + result[num - 1] * (num - 2)) / ((num - 1) * 2));
        if(num == 3) {
            for(int i = 0; i < num; i++) {
                result[i] = version * 4 + 16 - result[i];
            }
            return result;
        }
        int step = result[num - 1] - result[num - 2];
        for(int i = num - 3; i > 0; i--) {
            result[i] = result[i + 1] - step;
        }
        for(int i = 0; i < num; i++) {
            result[i] = version * 4 + 16 - result[i];
        }
        return result;
    }
    int[] GetAlignmentPoints() {
        int[] AlignmentCoords = GetAlignmentCoords();
        if(AlignmentCoords.Length == 0)
            return new int[0];
        int[] ans = new int[2 * AlignmentCoords.Length * AlignmentCoords.Length - 6];
        int nr = 0;
        for(int i = 0; i < AlignmentCoords.Length; i++) {
            for(int j = 0; j < AlignmentCoords.Length; j++) {
                if(i == AlignmentCoords.Length - 1 && j == AlignmentCoords.Length - 1)
                    continue;
                if(i == 0 && j == AlignmentCoords.Length - 1)
                    continue;
                if(j == 0 && i == AlignmentCoords.Length - 1)
                    continue;

                ans[nr++] = AlignmentCoords[i];
                ans[nr++] = AlignmentCoords[j];
            }
        }

        return ans;
    }
    byte[] GetAllDataBlocks() {
        List<int> ans = new List<int>();
        
        for(int j = code.Length - 1; j >= 1; j -= 2) {
            int nj = j - (j > 6 ? 1 : 2);
            if(j % 4 != 2) {
                for(int i = code.Length - 1; i >= 0; i--) {
                    if(IsData(i, nj + 1))
                        ans.Add(code[i][nj + 1]);
                    if(IsData(i, nj))
                        ans.Add(code[i][nj]);
                }
            } else {
                for(int i = 0; i < code.Length; i++) {
                    if(IsData(i, nj + 1))
                        ans.Add(code[i][nj + 1]);
                    if(IsData(i, nj))
                        ans.Add(code[i][nj]);
                }
            }
        }

        byte[] blocks = new byte[ans.Count / 8];
        for(int i = 0; i < blocks.Length; i++) {
            blocks[i] = (byte)(ans[i * 8] << 7);
            for(int j = 1; j < 8; j++) {
                blocks[i] |= (byte)(ans[i * 8 + j] << (7 - j));
            }
        }

        datatype = (DataType)((ans[0]) << 3 | ans[1] << 2 | ans[2] << 1 | ans[3]);
        return blocks;
    }

    
    
    // Get the most simnificant bit
    int MostSignificantBit(int x) {
        int cx = x;
        while(x != 0) {
            cx = x;
            x = x & (x - 1);
        }
        return cx;
    }

    // Get all the bits for ecc and mask pattern
    int GetMaskBits(int ECCLevel, int maskPattern) {
        int nr = (ECCLevel << 3) + maskPattern;
        int initnr = nr;
        nr <<= 10;
        int gen = 0x0537; // 10100110111 is the generator=0x0537
        int initGen = gen;
        while(MostSignificantBit(nr) >= 1024) {
            while(MostSignificantBit(gen) < MostSignificantBit(nr)) {
                gen <<= 1;
            }
            nr ^= gen;
            gen = initGen;
        }
        nr += initnr << 10;
        nr ^= 0x5412;
        return nr;
    }



    public int ApplyMask(int mask) {
        for(int i = 0; i < code.Length; i++) {
            for(int j = 0; j < code.Length; j++) {
                if(!IsData(i, j) || !masks[mask](i, j)) continue;
                code[i][j] ^= 1;
            }
        }
        int ECCLevel = errorCorrectionLevel;
        int maskBits = GetMaskBits(ECCLevel, mask);
        // THIS CODE IS FOR DEBUGGING PROPURSES IF YOU WANT TO SEE ANOTHER MASK
        for(int j = 0; j <= 5; j++) {
            code[8][j] = (maskBits >> (14 - j)) & 1;
        }
        for(int j = 6; j <= 7; j++) {
            code[8][j + 1] = (maskBits >> (14 - j)) & 1;
        }
        code[7][8] = (maskBits >> (14 - 8)) & 1;
        for(int i = 0; i <= 5; i++) {
            code[i][8] = (maskBits >> (i)) & 1;
        }
        for(int i = 0; i <= 6; i++) {
            code[code.Length - i - 1][8] = (maskBits >> (14 - i)) & 1;

        }
        for(int j = 0; j <= 7; j++) {
            code[8][code.Length - 1 - j] = (maskBits >> j) & 1;
        }
        maskUsed = mask;
        return CalculatePenaltyScore();
    }

    int CalculatePenaltyScore() {
        int consecutives = 0;
        int score = 0;

        // Calculate hroziontal consecutives penalties
        for(int i = 0; i < code.Length; i++) {
            if(consecutives == 5) score += 3;
            else if(consecutives > 5) score++;
            consecutives = 1;
            for(int j = 1; j < code.Length; j++) {
                if(code[i][j] == code[i][j - 1]) consecutives++;
                else {
                    if(consecutives == 5) score += 3;
                    else if(consecutives > 5) score += 3 + consecutives - 5;
                    consecutives = 1;
                }
            }
        }
        // Calculate vertical consecutives penalties
        for(int i = 0; i < code.Length; i++) {
            if(consecutives == 5) score += 3;
            else if(consecutives > 5) score++;
            consecutives = 1;
            for(int j = 1; j < code.Length; j++) {
                if(code[j][i] == code[j - 1][i]) consecutives++;
                else {
                    if(consecutives == 5) score += 3;
                    else if(consecutives > 5) score += 3 + consecutives - 5;
                    consecutives = 1;
                }
            }
        }

        // Calculate 2x2 penalties
        for(int i = 1; i < code.Length; i++) {
            for(int j = 1; j < code.Length; j++) {
                if(code[i][j] == code[i][j - 1] && code[i][j - 1] == code[i - 1][j - 1] && code[i - 1][j - 1] == code[i - 1][j])
                    score += 3;
            }
        }

        // Horizontal finder-like penalties
        for(int i = 0; i < code.Length; i++) {
            int nr = 0, mod = 1 << 11;

            for(int j = 0; j < 11; j++)
                nr = nr * 2 + code[i][j];
            if(nr == 1488 || nr == 93)
                score += 40;
            for(int j = 11; j < code.Length; j++) {
                nr = nr % mod * 2 + code[i][j];
                if(nr == 1488 || nr == 93)
                    score += 40;
            }
        }
        // Vertical finder-like penalties
        for(int i = 0; i < code.Length; i++) {
            int nr = 0, mod = 1 << 11;
            for(int j = 0; j < 11; j++)
                nr = nr * 2 + code[j][i];
            if(nr == 1488 || nr == 93)
                score += 40;
            for(int j = 11; j < code.Length; j++) {
                nr = nr % mod * 2 + code[j][i];
                if(nr == 1488 || nr == 93)
                    score += 40;
            }
        }

        // Dark/light balance penalties
        int darkModules = 0;
        for(int i = 0; i < code.Length; i++)
            for(int j = 0; j < code.Length; j++)
                darkModules += code[i][j];
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
