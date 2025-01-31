using System.Text;

var arr = Utility.ReadMatrixFromFile(@"../../../input.txt");

var testing = new Generator();
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
}

code.Print();

var res = QRCodeDecoder.DecodeQR(code);
var sb = new StringBuilder();
for(int i = 0; i < res.Length; i++) {
    sb.Append((char)res[i]);
}
Console.WriteLine(sb.ToString());