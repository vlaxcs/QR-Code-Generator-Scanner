using System;

public static class UI
{
    public static void validOutput(string output)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(output);
    }
    public static void validOutput(string output, int[] array)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(output);
        if (array != null)
        {
            for (int j = 0; j < array.Length; j++)
                Console.Write(Convert.ToChar(array[j]));
        }
        Console.WriteLine();
    }

    public static void invalidOutput(string output)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(output);
    }

    public static void neutralOutput(string output)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(output);
    }

    public static void resetOutput()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine();
    }
}