/*<><><><>< QR CODE GENERATOR ><><><><><>

 CCCC  L      AAAAA  TTTTT III TTTTT EEEEE
C      L     A     A   T    I    T   E
C      L     AAAAAAA   T    I    T   EEEE
C      L     A     A   T    I    T   E
 CCCC  LLLLL A     A   T   III   T   EEEEE

<><><><><>< QR CODE GENERATOR ><><><><><> */

Console.WriteLine("Lectures link:");
var qr = QRCodeGenerator.Generate("https://cs.unibuc.ro/~crusu/asc/lectures.html");
qr.Print();
qr.SaveToFile(@"../../../../Example Input/lecturesLink.png");
Console.WriteLine($"Scanning qr...");
Console.WriteLine(QRCodeDecoder.DecodeQR(qr));
Console.WriteLine();

Console.WriteLine("\n");
Console.WriteLine($"And then comes, our team name \"Clatite\"");
qr = QRCodeGenerator.Generate("Clatite");
qr.Print();
qr.SaveToFile(@"../../../../Example Output/teamName.png");
Console.WriteLine($"Scanning qr...");
Console.WriteLine(QRCodeDecoder.DecodeQR(qr));
Console.WriteLine();

Console.WriteLine();

while(true) {
    Console.WriteLine("What message would you want to make a QR of?");
    string message = Console.ReadLine();

    Console.WriteLine("Enter the error correction level (0-Low, 1-Medium, 2-Quartile, 3-High):");
    int eccLevel = Convert.ToInt32(Console.ReadLine());

    Console.WriteLine("Enter the minimum version (1-40):");
    int minVersion = Convert.ToInt32(Console.ReadLine());

    Console.WriteLine("Generating QR...");
    qr = QRCodeGenerator.Generate(message, eccLevel, minVersion);
    qr.Print();
    qr.SaveToFile("../../../../Example Output/myqr.png");

    Console.WriteLine("\n\n");
}

/* <><><><><> QR CODE SCANNER <><><><><><>

 CCCC  L      AAAAA  TTTTT III TTTTT EEEEE
C      L     A     A   T    I    T   E
C      L     AAAAAAA   T    I    T   EEEE
C      L     A     A   T    I    T   E
 CCCC  LLLLL A     A   T   III   T   EEEEE

<><><><><><> QR CODE SCANNER <><><><><><> */