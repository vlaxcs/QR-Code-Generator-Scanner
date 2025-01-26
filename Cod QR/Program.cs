using Cod_QR;
//Console.WriteLine("hi");
//Console.ReadLine();

Console.WriteLine(Directory.GetCurrentDirectory());

while(true) {
    QRCodeImageParser.Parse(@"../../../../Reference.png");
    Console.ReadLine();
}
