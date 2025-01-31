using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

public static class Tools
{
    public static string inputMessage, errorCorrectionLevel, tryForcedVersion;
    public static int forcedVersion;

    public static void valid(string output)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(output);
    }

    public static void invalid(string output)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(output);
    }

    public static string getInputMessage()
    {
        // INPUT: Message to decode
        Console.Write("Message to encode: ");
        return Console.ReadLine();
    }

    public static int getECCLevel()
    {
        // OPTIONAL INPUT: Choose the error correction level
        Console.WriteLine("[OPTIONAL] Error correction levels");
        Console.WriteLine("[OPTIONAL] Type 'L' for Low");
        Console.WriteLine("[OPTIONAL] Type 'M' for Medium");
        Console.WriteLine("[OPTIONAL] Type 'Q' for Quiet");
        Console.WriteLine("[OPTIONAL] Type 'H' for High");
        Console.Write("[OPTIONAL] Choose error correction level, else ENTER: ");
        errorCorrectionLevel = Console.ReadLine();
    }

    public static void setup()
    {
        inputMessage = getInputMessage();
        errorCorrectionLevel = getECCLevel();

        // OPTIONAL INPUT: Minimum version
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("[OPTIONAL] Minimum version");
        Console.Write("[OPTIONAL] Choose a number from 1 to 40, else ENTER: "); 
        try
        {
            forcedVersion = Convert.ToInt32(Console.ReadLine());
            if (forcedVersion >= 1 && forcedVersion <= 40)
                valid($"Forced version: {forcedVersion}");
        }
        catch (Exception e)
        {
            forcedVersion = -1;
            invalid("Unable to fetch the version. It will be automatically set.");
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Message to encode: {inputMessage}");
    }
}

class Generator
{
    // INPUT: Message to decode
    // INPUT: Choose the error correction level
    // [EVENTUALLY FORCE] Minimum version
    // [EVENTUALLY FORCE] Mask pattern
    // Compute message length
    // Try to set the best encoding

    // Choose adequate version
    // Generate blocks for each message character
    // Encode
    // Compute the best score for all masks (if not specified)
    // Apply mask
    // Draw the QR Code

    public Generator()
    {
        Tools.setup();
    }
}
