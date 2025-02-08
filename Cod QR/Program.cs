var qr = QRCodeGenerator.Generate("Minecraft");
qr.Print();

qr.SaveToFile(@"C:\Dalv\TEST\firstqr.png", 30);