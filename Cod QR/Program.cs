//List<string> items = new List<string> {"Clatite", "QR1", "QR2", "QR3", "QR4", "QR5", "QR6", "QR7", "QRdalv", "QR8", "QR9" };
List<string> items = new List<string> { "QR10", "QR11", "QR12", "QR14", "QR16", "QR17", "QR19" };

for (int i = 0; i < items.Count; ++i)
{
    var code = QRCodeImageParser.Parse(@"../../../../" + items[i] + ".png");
    code.Print();
    Console.Write($"Filename: {items[i]}.png | ");
    try
    {
        var res = QRCodeDecoder.DecodeQR(code);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Rezultat >> ");
        for (int j = 0; j < res.Length; j++)
            Console.Write(Convert.ToChar(res[j]));
        Console.WriteLine();
    }
    catch (Exception e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"Imposibil >> {e.ToString()}\n");
    }
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine();
}