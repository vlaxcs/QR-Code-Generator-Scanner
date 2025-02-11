﻿/*<><><><>< QR CODE GENERATOR ><><><><><>

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
Console.WriteLine($"Scanning QR Code...\n" + QRCodeDecoder.DecodeQR(qr) + "\n\n");

Console.WriteLine($"And then comes, our team name \"Clatite\"");
qr = QRCodeGenerator.Generate("Clatite");
qr.Print();
qr.SaveToFile(@"../../../../Example Output/teamName.png");
Console.WriteLine($"Scanning QR Code...\n" + QRCodeDecoder.DecodeQR(qr) + "\n\n");

while (true)
{
    Console.WriteLine("What message would you want to make a QR of?");
    string message = Console.ReadLine();

    Console.WriteLine("Enter the error correction level (0 - Low, 1 - Medium, 2 - Quartile, 3 - High):");
    int eccLevel = Convert.ToInt32(Console.ReadLine());

    Console.WriteLine("Enter the minimum version (1 - 40):");
    int minVersion = Convert.ToInt32(Console.ReadLine());

    Console.WriteLine("Generating QR Code...");
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

/*

//names of files which contains QR codes (it must be a PNG)
List<string> files = new List<string> { "Clatite", "QR2"};

foreach(var filename in files) {
    Utility.WriteNeutral($"Filename: {filename}.png");
    try {
        // if the filename is valid, parses, extracts information about image,
        // generates the equivalent binary matrix | then, draws it
        var code = QRCodeImageParser.Parse(@"../../../../INPUT/" + filename + ".png");
        code.Print();

        try {
            // stores the decoded message's characters ASCII CODE in this array
            // if there are no errors, writes it (character by character)
            var decodedMessage = QRCodeDecoder.DecodeQR(code);
            Utility.WriteValid("Rezultat >> ", decodedMessage.blocks);
        } catch(Exception e) {
            // somewhere an erorr is raised, it will be written
            Utility.WriteInvalid($"Imposibil >> {e.ToString()}\n");
        }
    } catch {
        // there was an error parsing the image
        Utility.WriteInvalid($"Imposibil de generat matricea binară!");
    }
    Utility.ResetConsole();
}

*/