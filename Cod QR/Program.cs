using System.Text;

var arr = Utility.ReadMatrixFromFile(@"../../../input.txt");

// var code = new QRCode(arr);
var code = QRCodeImageParser.Parse(@"../../../../Reference.png");

code.Print();

var res = QRCodeDecoder.DecodeQR(code);
var sb = new StringBuilder();
for(int i = 0; i < res.Length; i++) {
    sb.Append((char)res[i]);
}
Console.WriteLine(sb.ToString());