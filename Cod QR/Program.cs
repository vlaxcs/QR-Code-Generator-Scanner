
for(int v = 1; v <= 40; v++) {
    for(int e = 0; e < 4; e++) {
        Console.WriteLine($"Testing V{v} E{e}");
        var qr = QRCodeGenerator.Generate("PROJECT NAYUKI 3141592653589", e, v);

        //qr.Print();

        qr.SaveToFile(@$"C:\Dalv\TEST\V{v}E{e}.png", 15);
        Console.WriteLine();
    }
}



//var qr = QRCodeImageParser.Parse(@"C:\Dalv\TEST\hello.png");

//qr.Print();

//Console.WriteLine(QRCodeDecoder.DecodeQR(qr));


///*
// CCCC  L      AAAAA  TTTTT III TTTTT EEEEE
//C      L     A     A   T    I    T   E
//C      L     AAAAAAA   T    I    T   EEEE
//C      L     A     A   T    I    T   E
// CCCC  LLLLL A     A   T   III   T   EEEEE

//<><><><><><> QR CODE SCANNER <><><><><><> */

//// names of files which contains QR codes (it must be a PNG)
//List<string> files = new List<string> { "Clatite", "QR2", "QR3", "QR10", "QR11", "QR12", "QR14", "QR16", "QR17", "QR19" };

//foreach(var filename in files) {
//    Utility.WriteNeutral($"Filename: {filename}.png");
//    try {
//        // if the filename is valid, parses, extracts information about image,
//        // generates the equivalent binary matrix | then, draws it
//        var code = QRCodeImageParser.Parse(@"../../../../INPUT/" + filename + ".png");
//        code.Print();

//        try {
//            // stores the decoded message's characters ASCII CODE in this array
//            // if there are no errors, writes it (character by character)
//            var decodedMessage = QRCodeDecoder.DecodeQR(code);
//            Utility.WriteValid("Rezultat >> ", decodedMessage.blocks);
//        } catch(Exception e) {
//            // somewhere an erorr is raised, it will be written
//            Utility.WriteInvalid($"Imposibil >> {e.ToString()}\n");
//        }
//    } catch {
//        // there was an error parsing the image
//        Utility.WriteInvalid($"Imposibil de generat matricea binară!");
//    }

//    Utility.ResetConsole();
//}

///*
// CCCC  L      AAAAA  TTTTT III TTTTT EEEEE
//C      L     A     A   T    I    T   E
//C      L     AAAAAAA   T    I    T   EEEE
//C      L     A     A   T    I    T   E
// CCCC  LLLLL A     A   T   III   T   EEEEE

//<><><><><><> QR CODE GENERATOR <><><><><><> */