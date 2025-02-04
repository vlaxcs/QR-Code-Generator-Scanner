using System.Text;

var message = "314159265358979323846264s338327950288419716939";

MessageEncoder.EncodeMessage(DataType.Numeric, message);

return;

var arr = Utility.ReadMatrixFromFile(@"../../../input.txt");

//var code = new QRCode(arr);
var code = QRCodeImageParser.Parse(@"../../../../Alphanumeric.png");

code.Print();

var res = QRCodeDecoder.DecodeQR(code);