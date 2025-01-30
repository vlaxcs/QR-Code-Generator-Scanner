using System.Collections;
using System.Text;
using System.Xml.Linq;
using Cod_QR;

var arr = Utility.ReadMatrixFromFile(@"../../../input.txt");

var code = new QRCode(arr);

code.Print();