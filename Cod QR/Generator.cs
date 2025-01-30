/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        // INPUT: Message to decode
        Console.Write("Message to encode: ");
        string inputMessage = Console.ReadLine();

        // INPUT: Choose the error correction level
        Console.WriteLine("[OPTIONAL] Error correction levels");
        Console.WriteLine("[OPTIONAL] Type 'L' - Low");
        Console.WriteLine("[OPTIONAL] Type 'M' - Medium");
        Console.WriteLine("[OPTIONAL] Type 'Q' - Quiet");
        Console.WriteLine("[OPTIONAL] Type 'H' - High");
        Console.Write("[OPTIONAL] Choose error correction level, else ENTER: ");
        string errorCorrectionLevel = Console.ReadLine();

        // [EVENTUALLY FORCE] Minimum version
        Console.WriteLine("[OPTIONAL] Minimum version");
        Console.WriteLine("[OPTIONAL] Choose a number from ")
        int forcedVersion = int.Parse(Console.ReadLine());

        Console.WriteLine($"{inputMessage}");
    }
}
*/