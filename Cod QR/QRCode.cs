using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class QRCode {
    readonly int[][] code;

    public readonly int version;

    public readonly int[] data;

    public DataType Datatype;
    public enum DataType {
        none = 0,
        numeric = 1,
        Alphanumeric = 2,
        Byte = 4,
        kanji = 8,

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

        //for(int i = 0; i < code.Length; i++) {
        //    for(int j = 0; j < code.Length; j++) {
        //        Console.Write(IsData(i, j) ? "#" : "_");
        //    }
        //    Console.WriteLine();
        //}

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
        int max_width = SizeForVersion(version);
        int[] alligmentPoints = GetAlignmentPoints();

        if(x < 9 && y < 9) return false;

        if(x < 9 && y > max_width - 9) return false;
        if(x > max_width - 9 && y < 9) return false;

        if(version >= 7 && x < 7 && y > max_width - 12) return false;
        if(version >= 7 && y < 7 && x > max_width - 12) return false;

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
}
