using System.Text;

var arr = Utility.ReadMatrixFromFile(@"../../../input.txt");

//var code = new QRCode(arr);
var code = QRCodeImageParser.Parse(@"../../../../Alphanumeric.png");

code.Print();

var res = QRCodeDecoder.DecodeQR(code);