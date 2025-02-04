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
    public static string inputMessage;
    public static int version, errorCorrectionLevel;

    // Valid input statement
    public static void valid(string output)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(output);
    }

    // Invalid input statement
    public static void invalid(string output)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(output);
    }

    // Gets the message to decode from STDIN
    public static string getInputMessage()
    {

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Message to encode: ");
        return Console.ReadLine();
    }

    // OPTIONALS
        // Gets the error correction level from STDIN
        public static int getInputECCLevel()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("[OPTIONAL] Error correction levels");
            Console.WriteLine("[OPTIONAL] Type 'L' for Low");
            Console.WriteLine("[OPTIONAL] Type 'M' for Medium");
            Console.WriteLine("[OPTIONAL] Type 'Q' for Quiet");
            Console.WriteLine("[OPTIONAL] Type 'H' for High");
            Console.Write("[OPTIONAL] Choose error correction level, else ENTER: ");
            string input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                char errorCorrectionLevel = char.ToUpper(input[0]);
                switch (errorCorrectionLevel)
                {
                    case 'L': return 1;
                    case 'M': return 0;
                    case 'Q': return 3;
                    case 'H': return 2;
                    default: return -1;
                }
            }
            return -1;
        }

        // Gets the minimum version from STDIN    
        public static int getVersion()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("[OPTIONAL] Minimum version");
            Console.Write("[OPTIONAL] Choose a number from 1 to 40, else ENTER: ");
            try
            {
                int forcedVersion = Convert.ToInt32(Console.ReadLine());
                if (forcedVersion >= 1 && forcedVersion <= 40)
                    return forcedVersion;
                else
                    return -1;
            }
            catch (Exception e)
            { return -1; }
        }

    // Gets all data from user
    public static void input()
    {
        inputMessage = getInputMessage();
        version = getVersion();
        errorCorrectionLevel = getInputECCLevel();

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Message to encode: {inputMessage}");
        Console.WriteLine($"Message's length: {inputMessage.Length}");

        if (version != -1) valid($"Forced version: {version}");
        else invalid("Unable to fetch the version. It will be automatically set.");

        if (errorCorrectionLevel != -1) // Ensure it's a valid level
        {
            switch (errorCorrectionLevel)
            {
                case 1: valid("Error correction level: L"); break;
                case 0: valid("Error correction level: M"); break;
                case 3: valid("Error correction level: Q"); break;
                case 2: valid("Error correction level: H"); break;
                default: invalid("Unable to fetch the error correction level. It will be automatically set."); break;
            }
        }
        else invalid($"Unable to fetch the error correction level. It will be automatically set.");
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void doMath()
    {
        Console.WriteLine(inputMessage.Length);
        for (var i = 0; i < inputMessage.Length; ++i)
        {
            char c = inputMessage[i]; // Change this character for testing

            Console.WriteLine($"Character: {c}");
            Console.WriteLine($"Is Number: {char.IsDigit(c)}");
            Console.WriteLine($"Is Alphanumeric: {char.IsLetterOrDigit(c)}");
            Console.WriteLine($"Is Byte (0-255): {c <= 255}");
            Console.WriteLine($"Is Kanji: {(c >= '\u4E00' && c <= '\u9FFF') || (c >= '\u3400' && c <= '\u4DBF')}");
        }
    }
}

class Generator
{
    // Compute message length
    //Donot // Try to set the best encoding

    // Choose adequate version
    
    // Generate blocks for each message character
    
    // Encode
    
    // Compute the best score for all masks (if not specified)

    // Apply mask
    //DOnot  // Draw the QR Code

    public Generator()
    {
        Tools.input();
        Tools.doMath();

    }
}
