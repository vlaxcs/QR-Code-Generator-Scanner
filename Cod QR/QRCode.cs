using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class QRCode {
    readonly int[][] code;
    public readonly int version;
    public readonly int[] data;

    /* Data masking */
     Func<int, int, bool>[] masks = {
        (i, j) => (i + j) % 2 == 0,                     // Mask 0
        (i, j) => i % 2 == 0,                           // Mask 1
        (i, j) => j % 3 == 0,                           // Mask 2
        (i, j) => (i + j) % 3 == 0,                     // Mask 3
        (i, j) => (i / 2 + j / 3) % 2 == 0,             // Mask 4
        (i, j) => i * j % 2 + i * j % 3 == 0,           // Mask 5
        (i, j) => (i * j % 2 + i * j % 3) % 2 == 0,     // Mask 6
        (i, j) => ((i + j) % 2 + i * j % 3) % 2 == 0    // Mask 7
    };

    public DataType Datatype;
    public enum DataType {
        None = 0,
        Numeric = 1,
        Alphanumeric = 2,
        Byte = 4,
        Kanji = 8
    }


    public QRCode(int[][] mat) {
        code = mat;
        if((code.GetLength(0) - 17) % 4 != 0) {
            throw new Exception("Size is invalid");
        }

        version = (code.GetLength(0) - 17) / 4;
        if(version < 1 || version > 40) {
            throw new Exception("Version is invalid");
        }

        data = GetData();
    }



    public void Print() {
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
    int[] GetData() {
        List<int> ans = new List<int>();
        for(int j = code.Length - 1; j >= 0; j -= 2) {
            int nj = j - (j > 6 ? 1 : 0);
            if(nj % 4 != 1) {
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
        Datatype = (DataType)((ans[0]) << 3 | ans[1] << 2 | ans[2] << 1 | ans[3]);
        return ans.ToArray();
    }



    public int ApplyMask(int mask) {
        for(int i = 0; i < code.Length; i++) {
            for(int j = 0; j < code.Length; j++) {
                if(!IsData(i, j) || !masks[mask](i, j)) continue;
                code[i][j] ^= 1;
            }
        }

        //TODO: Write mask bits 

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
