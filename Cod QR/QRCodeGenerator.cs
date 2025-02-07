using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class QRCodeGenerator
{
    public int[][] code;
    public int version;
    public int ECCLevel;
    public int mask;

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


    public QRCodeGenerator(byte[] DataBlocks, int version, int ECCLevel)
    {
        this.mask= mask;
        this.version = version;
        this.ECCLevel = ECCLevel;
        code = new int[Utility.SizeForVersion(version)][];
        for (int i = 0; i < code.Length; i++)
        {
            code[i] = new int[Utility.SizeForVersion(version)];
        }


        PutStripes();
        PutBlocks();
        PutAligmentPoints();
        ApplyVersionBits();
        SetAllDataBlocks(DataBlocks);
        PlaceBestMask();
        
        Print();

    }
    void PlaceBestMask()
    {
        int MinScore = 999999999;
        int bestMask = -1;
        for (int  i=0; i <=7; i++)
        {
            PutMaskBits(ECCLevel, i);
            ApplyMask(i);
            int penaltyScore=CalculatePenaltyScore();
            if (penaltyScore < MinScore)
            {
                MinScore = penaltyScore;
                bestMask = i;
            }
            ApplyMask(i);
        }
        mask = bestMask;
        PutMaskBits(ECCLevel, mask);
        ApplyMask(mask);

    }
    void ApplyVersionBits()
    {
        if (version < 7)
        {
            return;
        }
        version=GetVersionBits(version);
        for (int j = 0; j < 6; j++)
        {
            for (int i = 0; i < 3; i++)
            {
                code[code.Length - 11 + i][j]=version>>(18-i*6+j)&1;
                code[j][code.Length - 11 +i] = version >> (18 - i * 6 + j) & 1;

            }
        }

    }
    void PutAligmentPoints()
    {
        var aligmentPoints = GetAlignmentPoints();
        for (int k = 0; k < aligmentPoints.Length; k+=2)
        {
            var allx=aligmentPoints[k];
            var ally=aligmentPoints[k+1];
            for (int i = -2; i <= 2; i++)
            {
                for (int j =-2; j < 2; j++)
                {
                    code[allx+i][ally+j] = 1;
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
    void PutBlocks()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                code[i][j] = code[code.Length - 1-i][j]=code[i][code.Length-1-j]=1;
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
    void PutStripes()
    {
        for (int i = 0; i < code.Length; i++)
        {
            code[6][i] = code[i][6] = i & 1 ^ 1;
        }

    }

    void PutMaskBits(int ECCLevel, int mask){
        int maskBits = GetMaskBits(ECCLevel, mask);
       
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
    }
    public void Print()
    {

        Console.BackgroundColor = ConsoleColor.White;
        Console.WriteLine(new string(' ', code.Length * 2 + 4));

        for (int i = 0; i < code.Length; i++)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write("  ");

            for (int j = 0; j < code.Length; j++)
            {
                if (code[i][j] == 1) Console.BackgroundColor = ConsoleColor.Black;
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
    }
    void SetAllDataBlocks(byte[] TotalData)
    {
        int nr = 0;
        for (int j = code.Length - 1; j >= 1; j -= 2)
        {
            int nj = j - (j > 6 ? 1 : 2);
            if (j % 4 != 2)
            {
                for (int i = code.Length - 1; i >= 0; i--)
                {
                    if (IsData(i, nj + 1))
                    {
                        code[i][nj + 1] = (TotalData[nr / 8] >> (7 - (nr % 8))) & 1;
                        nr++;
                    }
                    if (IsData(i, nj))
                    {
                        code[i][nj] = (TotalData[nr / 8] >> (7 - (nr % 8))) & 1;
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
                        code[i][nj + 1] = (TotalData[nr / 8] >> (7 - (nr % 8))) & 1;
                        nr++;
                    }
                    if (IsData(i, nj))
                    {
                        code[i][nj] = (TotalData[nr / 8] >> (7 - (nr % 8))) & 1;
                        nr++;
                    }
                }
            }
        }
    }
    bool IsData(int x, int y)
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
                return false;
        }
        return true;
    }
    int[] GetAlignmentCoords()
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
    int[] GetAlignmentPoints()
    {
        int[] AlignmentCoords = GetAlignmentCoords();
        if (AlignmentCoords.Length == 0)
            return new int[0];
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
    int GetMaskBits(int ECCLevel, int maskPattern)
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

    void ApplyMask(int mask)
    {
        for (int i = 0; i < code.Length; i++)
        {
            for (int j = 0; j < code.Length; j++)
            {
                if (!IsData(i, j) || !masks[mask](i, j)) continue;
                code[i][j] ^= 1;
            }
        }
    }
    int GetVersionBits(int Version)//TODO:TEST THIS 
    {
        if(version<7||version>40)
            throw new Exception("Version is not good");
        int initnr = Version;
        Version <<= 12;
        int gen = 0x1F25; // 1111100100101 is the generator=0x1F25
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
        Version += initnr << 12;
        return Version;
    }


    int CalculatePenaltyScore()
    {
        int consecutives = 0;
        int score = 0;

        // Calculate hroziontal consecutives penalties
        for (int i = 0; i < code.Length; i++)
        {
            if (consecutives == 5) score += 3;
            else if (consecutives > 5) score++;
            consecutives = 1;
            for (int j = 1; j < code.Length; j++)
            {
                if (code[i][j] == code[i][j - 1]) consecutives++;
                else
                {
                    if (consecutives == 5) score += 3;
                    else if (consecutives > 5) score += 3 + consecutives - 5;
                    consecutives = 1;
                }
            }
        }
        // Calculate vertical consecutives penalties
        for (int i = 0; i < code.Length; i++)
        {
            if (consecutives == 5) score += 3;
            else if (consecutives > 5) score++;
            consecutives = 1;
            for (int j = 1; j < code.Length; j++)
            {
                if (code[j][i] == code[j - 1][i]) consecutives++;
                else
                {
                    if (consecutives == 5) score += 3;
                    else if (consecutives > 5) score += 3 + consecutives - 5;
                    consecutives = 1;
                }
            }
        }

        // Calculate 2x2 penalties
        for (int i = 1; i < code.Length; i++)
        {
            for (int j = 1; j < code.Length; j++)
            {
                if (code[i][j] == code[i][j - 1] && code[i][j - 1] == code[i - 1][j - 1] && code[i - 1][j - 1] == code[i - 1][j])
                    score += 3;
            }
        }

        // Horizontal finder-like penalties
        for (int i = 0; i < code.Length; i++)
        {
            int nr = 0, mod = 1 << 11;

            for (int j = 0; j < 11; j++)
                nr = nr * 2 + code[i][j];
            if (nr == 1488 || nr == 93)
                score += 40;
            for (int j = 11; j < code.Length; j++)
            {
                nr = nr % mod * 2 + code[i][j];
                if (nr == 1488 || nr == 93)
                    score += 40;
            }
        }
        // Vertical finder-like penalties
        for (int i = 0; i < code.Length; i++)
        {
            int nr = 0, mod = 1 << 11;
            for (int j = 0; j < 11; j++)
                nr = nr * 2 + code[j][i];
            if (nr == 1488 || nr == 93)
                score += 40;
            for (int j = 11; j < code.Length; j++)
            {
                nr = nr % mod * 2 + code[j][i];
                if (nr == 1488 || nr == 93)
                    score += 40;
            }
        }

        // Dark/light balance penalties
        int darkModules = 0;
        for (int i = 0; i < code.Length; i++)
            for (int j = 0; j < code.Length; j++)
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



