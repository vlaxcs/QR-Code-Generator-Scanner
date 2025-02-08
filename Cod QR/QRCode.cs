using System.Text;

public partial class QRCode {
    readonly int[][] code;
    public int Version { get; private set; }
    public int ErrorCorrectionLevel { get; private set; }

    int maskUsed;
    public byte[] data;

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


    public QRCode(int[][] mat) {
        code = mat;

        Version = (code.Length - 17) / 4;
        if(Version < 1 || Version > 40) {
            throw new Exception("Version is invalid");
        }

        if((code.Length - 17) % 4 != 0) {
            throw new Exception("Size is invalid");
        }

        Initialize();
    }


    void Initialize() {
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
        ErrorCorrectionLevel = (mask1 >> 13) ^ 2;

        ApplyMask(maskUsed);
        data = GetAllDataBlocks();
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
        int maxWidth = Utility.SizeForVersion(Version);
        int[] alligmentPoints = GetAlignmentPoints();

        if(x < 9 && y < 9) return false;

        if(x < 9 && y > maxWidth - 9) return false;
        if(x > maxWidth - 9 && y < 9) return false;

        if(Version >= 7 && x < 7 && y > maxWidth - 12) return false;
        if(Version >= 7 && y < 7 && x > maxWidth - 12) return false;

        if(x == 6 || y == 6) return false;
        for(int i = 0; i < alligmentPoints.Length; i += 2) {
            if(Math.Abs(alligmentPoints[i] - x) < 3 && Math.Abs(alligmentPoints[i + 1] - y) < 3)
                return false;
        }
        return true;
    }


    int[] GetAlignmentCoords() {
        if(Version <= 1) {
            return new int[0];
        }
        int num = (Version / 7) + 2;
        int[] result = new int[num];
        result[0] = 6;
        if(num == 1) {
            for(int i = 0; i < num; i++) {
                result[i] = Version * 4 + 16 - result[i];
            }
            return result;
        }
        result[num - 1] = 4 * Version + 10;
        if(num == 2) {
            for(int i = 0; i < num; i++) {
                result[i] = Version * 4 + 16 - result[i];
            }
            return result;
        }
        result[num - 2] = 2 * ((result[0] + result[num - 1] * (num - 2)) / ((num - 1) * 2));
        if(num == 3) {
            for(int i = 0; i < num; i++) {
                result[i] = Version * 4 + 16 - result[i];
            }
            return result;
        }
        int step = result[num - 1] - result[num - 2];
        for(int i = num - 3; i > 0; i--) {
            result[i] = result[i + 1] - step;
        }
        for(int i = 0; i < num; i++) {
            result[i] = Version * 4 + 16 - result[i];
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

    

    // Get all the bits for ecc and mask pattern
    int GetMaskBits(int ECCLevel, int maskPattern) {
        int nr = (ECCLevel << 3) + maskPattern;
        int initnr = nr;
        nr <<= 10;
        int gen = 0x0537; // 10100110111 is the generator=0x0537
        int initGen = gen;
        while(nr.MostSignificantBit() >= 1024) {
            while(gen.MostSignificantBit() < nr.MostSignificantBit()) {
                gen <<= 1;
            }
            nr ^= gen;
            gen = initGen;
        }
        nr += initnr << 10;
        nr ^= 0x5412;
        return nr;
    }


    void ApplyMask(int mask) {
        for(int i = 0; i < code.Length; i++) {
            for(int j = 0; j < code.Length; j++) {
                if(!IsData(i, j) || !masks[mask](i, j)) continue;
                code[i][j] ^= 1;
            }
        }
    }
}

// TO PUT IN GENERATOR:
/*

*/
