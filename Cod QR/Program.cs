using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;
using Cod_QR;

//var test = new Generator();

var testing = new Generator();

var mesaj = new QREncodedMessage(DataType.Byte, new byte[] { 0, 0, 1, 1, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1, 1 }, "23f/");
var arr = mesaj.bitsArray;
var dataType = (int)mesaj.type;
Console.ReadLine();
int[][] blocksByECL = {
    new int[] { 19, 16, 13, 9 },
    new int[] { 34, 28, 22, 16 },
    new int[] { 55, 44, 34, 26 },
    new int[] { 80, 64, 48, 36 },
    new int[] { 108, 86, 62, 46 },
    new int[] { 136, 108, 76, 60 },
    new int[] { 156, 124, 88, 66 },
    new int[] { 194, 154, 110, 86 },
    new int[] { 232, 182, 132, 100 },
    new int[] { 274, 216, 154, 122 },
    new int[] { 324, 254, 180, 140 },
    new int[] { 370, 290, 206, 158 },
    new int[] { 428, 334, 244, 180 },
    new int[] { 461, 365, 261, 197 },
    new int[] { 523, 415, 295, 223 },
    new int[] { 589, 453, 325, 253 },
    new int[] { 647, 507, 367, 283 },
    new int[] { 721, 563, 397, 313 },
    new int[] { 795, 627, 445, 341 },
    new int[] { 861, 669, 485, 385 },
    new int[] { 932, 714, 512, 406 },
    new int[] { 1006, 782, 568, 442 },
    new int[] { 1094, 860, 614, 464 },
    new int[] { 1174, 914, 664, 514 },
    new int[] { 1276, 1000, 718, 538 },
    new int[] { 1370, 1062, 754, 596 },
    new int[] { 1468, 1128, 808, 628 },
    new int[] { 1531, 1193, 871, 661 },
    new int[] { 1631, 1267, 911, 701 },
    new int[] { 1735, 1373, 985, 745 },
    new int[] { 1843, 1455, 1033, 793 },
    new int[] { 1955, 1541, 1115, 845 },
    new int[] { 2071, 1631, 1171, 901 },
    new int[] { 2191, 1725, 1231, 961 },
    new int[] { 2306, 1812, 1286, 986 },
    new int[] { 2434, 1914, 1354, 1054 },
    new int[] { 2566, 1992, 1426, 1096 },
    new int[] { 2702, 2102, 1502, 1142 },
    new int[] { 2812, 2216, 1582, 1222 },
    new int[] { 2956, 2334, 1666, 1276 }
};
var minimum = 1;
var ECC = 0;
var version = -1;
var numberOfBlocks = -1;
var messageLength = 4;
var minBlocks = Math.Ceiling(arr.Length * 1.0 / 8);
var sizeOfArr = arr.Length;
var blocks = 0;
for (int i = minimum - 1; i <= 39; i++)
    if (blocksByECL[i][ECC] >= minBlocks) {
        version = i; numberOfBlocks = blocksByECL[i][ECC];
        break;
    }
var numberOfBits = numberOfBlocks * 8;
List<int> v = new List<int>();
int ct = 0;

int[] characterToBits(int ch, int size)
{
    int[] v = new int[size];
    int ct = 0;
    while(ch != 0)
    {
        v[ct++] = ch % 2;
        ch /= 2;
    }
    while (ct < size)
        v[ct++] = 0;

    for (int i = 0; i < size / 2; i++)
        (v[i], v[size - i - 1]) = (v[size - i - 1], v[i]);

    return v;
}

int[] rez = characterToBits(dataType, 4);
for (int i = 0; i < rez.Length; i++)
{ v.Add(rez[i]); ct ++;}
Console.WriteLine(10 - (int)(Math.Log2(dataType * 2 + 1)));
rez = characterToBits(messageLength, 10 - (int)(Math.Log2(dataType * 2 + 1)) + 1);
for (int i = 0; i < rez.Length; i++)
{ v.Add(rez[i]); ct++;}
for (int i = 0; i < arr.Length; i++)
{ v.Add(arr[i]); ct++; }
while (ct <= numberOfBits)
{
    v.Add(0); ct++;
    if (ct % 8 == 0)
        break;
}
while (ct < numberOfBits)
{
    rez = characterToBits(236, 8);
    for (int i = 0; i < rez.Length; i++)
    { v.Add(rez[i]); ct++; }
    if (ct >= numberOfBits)
        break;
    rez = characterToBits(17, 8);
    for (int i = 0; i < rez.Length; i++)
    { v.Add(rez[i]); ct++; }
}
for (int i = 0; i < numberOfBits; i++)
    Console.Write(v[i]);
Console.WriteLine(); 


List<byte> eccArray = new List<byte>();
int nr = 0, nr1 = 0, ap = 0;
for(int i = 0; i < v.Count; i ++)
{
    nr = (v[i] << 3) + (v[i + 1] << 2) + (v[i + 2] << 1) + v[i + 3];
    i = i + 3;
   // if (nr != 0 || ap == 1) {
        nr1 = nr1 * 16 + nr; ap += 1;
   // }
    if (ap == 2)
    {
        eccArray.Add((byte)nr1);
        nr1 = 0;
        ap = 0;
    }
}
if(nr1!= 0)
{
    nr1 = nr1 * 16;
    eccArray.Add((byte)nr1);
    nr1 = 0;
}
for (int i = 0; i < eccArray.Count; i++)
    Console.WriteLine($"{eccArray[i]:X}");



int total = 0;
version += 1;
for (int i = 0; i < version * 17 + 4; i++)
    for (int j = 0; j < version * 17 + 4; j++)
        if (IsData(i, j))
            total += 1;

GaloisField qr = new GaloisField((total - ct) / 8);
Console.WriteLine();
byte[] eccList = qr.Encode(eccArray.ToArray());
for (int i = 0; i < (total) / 8; i++)
    Console.WriteLine($"{eccList[i]:X}");

new QRCodeGenerator(eccList, version, 1);
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


int[][] code;
