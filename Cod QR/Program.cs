using System.Collections;
using System.Xml.Linq;
using Cod_QR;

var code = QRCodeImageParser.Parse(@"../../../../Reference.png");

code.Print();
