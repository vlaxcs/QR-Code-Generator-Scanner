using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;

/* Absolutely useful resources:
https://blog.qartis.com/decoding-small-qr-codes-by-hand/
https://people.inf.ethz.ch/gander/papers/qrneu.pdf
*/

public static class GaloisField
{
    public static int[] gf_exp = new int[512];
    public static int[] gf_log = new int[256];

    public static void generatePolynomials()
    {
        gf_exp[0] = 1;
        int x = 2;
        for (int i = 1; i < 256; ++i, x <<= 1)
        {
            if ((x & 0x100) != 0)
                x ^= 0x11d;

            gf_exp[i] = x;
            gf_log[x] = i;
        }

        for (int i = 255; i < 512; ++i)
            gf_exp[i] = gf_exp[i - 255];
    }

    public static int multiply(int x, int y)
    {
        // https://www.thonky.com/qr-code-tutorial/error-correction-coding#step-6-understand-multiplication-with-logs-and-antilogs
        // p * q with logarithms
        if (x == 0 || y == 0) return 0;
        return gf_exp[gf_log[x] + gf_log[y]];
    }

    public static int divide(int x, int y)
    {
        // https://www.thonky.com/qr-code-tutorial/error-correction-coding#step-6-understand-multiplication-with-logs-and-antilogs
        // p / q with logarithms
        if (y == 0) throw new Exception("Division by zero");
        if (x == 0) return 0;
        return gf_exp[gf_log[x] + 255 - gf_log[y]];
    }

    public static int[] poly_mul(int[] p, int[] q)
    {
        int pLen = p.Length, qLen = q.Length;
        int rLen = Math.Max(p.Length, q.Length);
        int[] r = new int[rLen];

        for (int j = 0; j < qLen; ++j)
            for (int i = 0; i < pLen; ++i)
                r[i + j] ^= multiply(p[i], q[j]);

        return r;
    }

    public static int[] poly_add(int[] p, int[] q)
    {
        int pLen = p.Length, qLen = q.Length;
        int rLen = Math.max(p.Length, q.Length);
        int[] r = new int[rLen];
        for (int i = 0; i < pLen; ++i) r[i + rLen - pLen] = p[i];
        for (int i = 0; i < qLen; ++i) r[i + rLen - qLen] ^= q[i];
        return r;
    }

    public static int[] poly_scale(int[] p, int x)
    {
        int pLen = p.Length();
        int[] result = new int[pLen];
        for (int i = 0; i < pLen; ++i) result[i] = multiply(p[i], x);
        return result;
    }

    public static int[] poly_scale(byte[] p, byte x)
    {
        pLen = p.Length;
        int[] r = new int[pLen];
        for (int i = 0; i < pLen; ++i) r[i] = gf_mul(p[i], x);
        return r;
    }

    public static int[] rs_generator_poly(int nsym)
    {
        int[] g = new int[nsym]; // not fucking sure
        for (int i = 0; i < nsym; i++) g = poly_mul(g, new int[] { 1, gf_exp[i] });
        return g;
    }

    public static int[] rs_encode_msg(int[] inputMessage, int nsym)
    {
        int inputMessageLength = inputMessage.Length;
        if (inputMessageLength + nsym > 255)
            throw new Exception("message too long");

        int[] gen = rs_generator_poly(nsym);
        int[] outputMessage = new int[inputMessageLength + nsym];
        outputMessage[..inputMessageLength] = inputMessage;
        for (int i = 0; i < inputMessageLength; i++)
        {
            int coef = inputMessage[i];
            if (coef != 0)
                for (int j = 0; j < gen.Length; j++)
                    inputMessage[i + j] ^= gf_mul(gen[j], coef);
        }
        outputMessage[..inputMessageLength] = inputMessage;
        return outputMessage;
    }

    public static int[] rs_calc_syndromes(int[] msg, int nsym)
    {
        int[] synd = new int[nsym];
        for (int i = 0; i < nsym; i++) synd[i] = gf_poly_eval(msg, gf_exp[i]);
        return synd;
    }

    public static int[] rs_forney_syndromes(int[] synd, int pos, int nmess)
    {
        int syndLen = synd.Length;
        int[] fsynd = new int[syndLen];
        Array.Copy(synd, fsynd, syndLen);
        for (int i = 0; i < pos.Length; i++)
        {
            int x = gf_exp[nmess - 1 - pos[i]];
            for (int j = 0; j < syndLen - 1; j++)
                fsynd[j] = gf_mul(fsynd[j], x) ^ fsynd[j + 1];

            Array.Resize(ref fsynd, syndLen - 1);
        }
        return fsynd;
    }

    public static int[] decode(int[] data)
    {
        if (typeof(data) == "string")
            data = bytearray(data, "utf-8");

        dec = bytearray();
        for (int i = 0; i < data.Length; i += 255)
        {
            int[] chunk = data.Skip(i).Take(255).ToArray();
            dec.AddRange(rs_correct_msg(chunk, nsym));
        }
        return dec;
    }
}

public static class Utility
{
    //  public static int computeDistance(int n, int k) { return n - k + 1; }

    public static int binaryArea(int[] array, int left, int right)
    {
        static int decify(int[] array, int currPow)
        {
            int result = 0;
            while (currPow-- > 0) result += (int)Math.Pow(2, currPow) * array[array.Length - currPow - 1];
            return result;
        }

        int upperPow = 0; int[] binary = new int[right - left + 1];
        for (int i = left; i <= right; ++i) binary[upperPow++] = array[i];
        return decify(binary, upperPow);
    }

    public static int computeEncodingRange(string type, int version)
    {
        int encodingMinVersion = 1, encodingMaxVersion = 40;
        if (encodingMinVersion > version || version > encodingMaxVersion) throw new Exception("Unsupported QR Code version!");
        int i = (version + 7) / 17;
        switch (type)
        {
            case "Numeric": { int[] blockSizes = { 10, 12, 14 }; return blockSizes[i]; }       // Numeric encoding (10/12/14 bits per 3 digits)
            case "Alphanumeric": { int[] blockSizes = { 9, 11, 13 }; return blockSizes[i]; }  // Alphanumeric encoding (9/11/13 bits per 2 characters)
            case "Byte": { int[] blockSizes = { 8, 16, 16 }; return blockSizes[i]; }         // Byte encoding (8/16/16 bits per character) 
            case "Kanji": { int[] blockSizes = { 8, 10, 12 }; return blockSizes[i]; }       // Kanji encoding (8/10/12 bits per character)
            default: { throw new Exception("Unsupported QR Code version!"); }
        }
    }

    public static void setCurrentPolynomial(int blockCount, int[] current)
    {
        GaloisField.generatePolynomials();
        for (int i = 0; i < blockCount; i++) current[i] = GaloisField.gf_exp[i];
    }


}

public class Class1
{
    public static void Main()
    {
        Dictionary<int, string> encodingTypes = new Dictionary<int, string>
        {
            { 1, "Numeric" },       // 0b0001 / 1 - Numeric encoding
            { 2, "Alphanumeric" },  // 0b0010 / 2 - Alphanumeric encoding
            { 4, "Byte" },          // 0b0100 / 4 - Byte encoding
            { 8, "Kanji" }          // 0b1000 / 8 - Kanji encoding
        };

        // From [Stefan]
        int[] bitArray = { 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0 };

        // Analizes the first 4 blocks of the encoding and gets the encoding type as decimal, let it be encodingID
        int encodingID = Utility.binaryArea(bitArray, 0, 3);
        Console.WriteLine($"[RAW DATA] Encoding ID: {encodingID}");

        // Sets the encoding type, given the encoding ID (browsing a dictionary)
        string encodingType = encodingTypes[encodingID];
        Console.WriteLine($"[RAW DATA] Encoding TYPE: {encodingType}");

        // Compute the encoding length, given the length of the bit array
        int encodingLength = bitArray.Length;
        Console.WriteLine($"[RAW DATA] Encoding LENGTH: {encodingLength}");

        // From [Stefan]
        int encodingVersion = 1;
        Console.WriteLine($"[RAW DATA] Encoding VERSION: {encodingVersion}");

        // Sets the codeword blocks' size in 'encodingRange'
        int encodingRange = Utility.computeEncodingRange(encodingType, encodingVersion);
        Console.WriteLine($"[RAW DATA] Encoding RANGE: {encodingRange}");

        // From [Stefan]
        int maskID = -1;
        Console.WriteLine($"[RAW DATA] Encoding MASK: {maskID}");

        // From [Stefan]
        int errorCorrectionLevel = -1;
        Console.WriteLine($"[RAW DATA] Encoding ERROR CORRECTION LEVEL: {errorCorrectionLevel}");

        int ls = 4, lf = 3 + encodingRange;
        int decodedMessageLength = Utility.binaryArea(bitArray, ls, lf);
        Console.WriteLine($"[RAW DATA] Decoded message LENGTH: {decodedMessageLength}");

        // TBC
        string decodedMessage = ""; int newDecodedMessageLength = 0, ecCodeWordsCount = 0;
        for (int i = lf + 1; i <= lf + encodingLength; i += encodingRange)
            try
            {
                int asciiCode = 0;
                if (newDecodedMessageLength < decodedMessageLength)
                {
                    for (int j = 0; j < encodingRange; ++j)
                    {
                        Console.Write(bitArray[i + j]);
                        asciiCode += bitArray[i + j] * (int)Math.Pow(2, encodingRange - j - 1);
                    }
                    char messageSymbol = (char)asciiCode;
                    decodedMessage += messageSymbol;
                    newDecodedMessageLength++;
                    Console.WriteLine($" << ASCII CODE: {asciiCode} | Character: {messageSymbol}");
                }
                else
                {
                    for (int j = 0; j < encodingRange; ++j)
                        Console.Write(bitArray[i + j]);
                    ecCodeWordsCount++;

                    Console.WriteLine($" << Initial error correction block");
                }
            }
            catch (Exception e)
            { break; }

        Console.WriteLine($"[RAW DATA] Decoded message: {decodedMessage}");
        Console.WriteLine($"[ERROR CORRECTION] CODE WORDS NUMBER: {ecCodeWordsCount}");

        int[] ecCurrentPolynomial = new int[ecCodeWordsCount];
        Utility.setCurrentPolynomial(ecCodeWordsCount, ecCurrentPolynomial);
        Console.Write("[ERROR CORRECTION] APPLYING POLYNOMIAL: ");
        for (int i = 0; i < ecCodeWordsCount; ++i)
            Console.Write($"{ecCurrentPolynomial[i]} ");
        Console.WriteLine();

        string decodedEC = "N/A";
        Console.WriteLine($"[ERROR CORRECTION] Decoded messages: {decodedEC}");
    }
}