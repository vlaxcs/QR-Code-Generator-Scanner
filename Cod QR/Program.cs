




for (int i = 39; i >= 1; i--) {
    Console.WriteLine($"Testing V{i}");
    try {
        var qr = QRCodeGenerator.Generate("Minecraft", 1, i);
        qr.Print();
    }catch(Exception e) {
        Console.WriteLine($"V{i} Error: {e.Message}");
    }
}

//qr.SaveToFile(@"C:\Dalv\TEST\firstqr.png", 30);

// ERRORS FOR:
// var qr = QRCodeGenerator.Generate("Minecraft", 4, 30);
// var qr = QRCodeGenerator.Generate("Minecraft", 1, 2);