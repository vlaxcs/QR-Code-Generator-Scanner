using System.Collections;
using System.Text;
using System.Xml.Linq;
using Cod_QR;

var test = new GaloisField(12);

var res = test.Decode(new byte[]{ 104, 101, 108, 108, 111, 32, 119, 111, 114, 88, 88, 88, 88, 121, 178, 88, 88, 1, 113, 185, 227, 226, 61 });
Console.WriteLine(Encoding.Default.GetString(res));


return;
int[][] ReadMatrixFromFile(string filePath) {
    var lines = File.ReadAllLines(filePath);
    int[][] res = new int[lines.Length][];
    for(int i = 0; i < lines.Length; i++) {
        res[i] = new int[lines[i].Length];
        for(int j = 0; j < lines[i].Length; j++) {
            res[i][j] = Convert.ToInt32(lines[i][j]) - 48;
            Console.Write(res[i][j] + " ");
        }
        Console.WriteLine();
    }
    return res;
}

var arr = ReadMatrixFromFile(@"../../../input.txt");

var code = new QRCode(arr);

code.Print();

for(int i = 0; i < 8; i++) {
    Console.WriteLine($"\n\n\nMask {i}:\n");
    int score = code.ApplyMask(i);
    code.Print();
    code.ApplyMask(i);
    Console.WriteLine($"\nScore {score}\n");
}