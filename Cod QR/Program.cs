using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
int n = 21, mask, version = 1;
int[,] mat = new int[n, n];



// stefan
int[] getAlignmentCoords()
   {
       if (version <= 1){
           return new int[0];
       }
       int num = (version / 7) + 2;
       int[] result = new int[num];
       result[0] = 6;
       if (num == 1)
       {
           for (int i = 0; i < num; i++)
           {
               result[i] = version * 4 + 17 - result[i] - 1;
           }
           return result;
       }
       result[num - 1] = 4 * version + 10;
       if (num == 2)
       {
           for (int i = 0; i < num; i++)
           {
               result[i] = version * 4 + 17 - result[i]-1;
           }
           return result;
       }
       result[num - 2] = 2 * ((result[0] + result[num - 1] * (num - 2)) / ((num - 1) * 2)); 
       if (num == 3)
       {
           for (int i = 0; i < num; i++)
           {
               result[i] = version * 4 + 17 - result[i]-1;
           }
           return result;
       }
       int step = result[num - 1] - result[num - 2];
       for (int i = num - 3; i > 0; i--)
       {
           result[i] = result[i + 1] - step;
       }
       for (int i = 0; i < num; i++)
       {
           result[i] = version * 4 + 17 - result[i]-1;
       }
       return result;
   }
  int[] getAlignmentPoints()
   {
       int[] AlignmentCoords=getAlignmentCoords();
       if (AlignmentCoords.Length == 0)
           return new int[0];
       int[] ans = new int[2*AlignmentCoords.Length * AlignmentCoords.Length - 6];
       int nr = 0;
       for (int i = 0; i < AlignmentCoords.Length; i++)
       {
           for (int j = 0; j < AlignmentCoords.Length; j++)
           {
               if (i == AlignmentCoords.Length - 1 && j == AlignmentCoords.Length - 1)
                   continue;
               if (i == 0 && j == AlignmentCoords.Length - 1)
                   continue;
               if (j == 0 && i == AlignmentCoords.Length - 1)
                   continue;
            
               ans[nr++]=AlignmentCoords[i];
               ans[nr++]=AlignmentCoords[j];
           }
       }

       return ans;
   }
  bool IsData(int x,int y)
   {
       int max_width= n;
       int[] alligmentPoints = getAlignmentPoints();
       if (x < 9 && y < 9) return false;
       if (x < 9 && y > max_width-9) return false;
       if (x > max_width-9 && y < 9) return false;
       if (version >= 7 && x < 7 && y > max_width - 12) return false;
       if (version >= 7 && y < 7 && x > max_width - 12) return false;

       if (x==6||y==6) return false;
       for(int i=0;i < alligmentPoints.Length; i += 2)
       {
           if (Math.Abs(alligmentPoints[i] - x) < 3 &&
               Math.Abs(alligmentPoints[i + 1] - y) < 3)
               return false;
       }
       return true;
   }
//stefan
bool valid(int i, int j, int mask)
{
    if (!IsData(i, j))
        return false;
    if (mask == 0)
    {
        if ((i + j) % 2 == 0)
            return true;
        return false;
    }
    if (mask == 1)
    {
        if (i % 2 == 0)
            return true;
        return false;
    }
    if (mask == 2)
    {
        if (j % 3 == 0)
            return true;
        return false;
    }
    if (mask == 3)
    {
        if ((i + j) % 3 == 0)
            return true;
        return false;
    }
    if (mask == 4)
    {
        if ((i / 2 + j / 3) % 2 == 0)
            return true;
        return false;
    }
    if (mask == 5)
    {
        if (i * j % 2 + i * j % 3 == 0)
            return true;
        return false;
    }
    if (mask == 6)
    {
        if (((i * j) % 2 + (i * j) % 3) % 2 == 0)
            return true;
        return false;
    }
    if (mask == 7)
    {
        if (((i + j) % 2 + i * j % 3) % 2 == 0)
            return true;
        return false;
    }
    return false;
}
void maskf(int[,] mat, int mask, int n)
{
    for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++)
            if (valid(i, j, mask))
            {
                mat[i, j] ^= 1;
            }
}
int detmask(int[,] mat)
{
    int nr = 0;
    int p = 1;
    for (int i = 4; i >= 2; i--)
    {
        nr = nr + mat[8, i] * p; p = p * 2;
    }
    return nr ^ 5;
}
/// cod generat
//mask = detmask(mat);
//maskf(mat, mask, n);

//// la generare
///


void copiaza(int[,] mat1, int[,] mat, int n)
{
    for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++)
            mat1[i, j] = mat[i, j];
}
int part1(int[,] mat)
{
    int ct = 0;
    int medie = 0;
    for (int i = 0; i < n; i++)
    {
        if (ct == 5)
            medie += 3;
        else if (ct > 5)
            medie++;
        ct = 1;
        for (int j = 1; j < n; j++)
        {
            if (mat[i, j] == mat[i, j - 1])
                ct++;
            else
            {
                if (ct == 5)
                    medie += 3;
                else if (ct > 5)
                    medie += 3 + ct - 5;
                ct = 1;
            }
        }
    }
    for (int i = 0; i < n; i++)
    {
        if (ct == 5)
            medie += 3;
        else if (ct > 5)
            medie++;
        ct = 1;
        for (int j = 1; j < n; j++)
        {
            if (mat[j, i] == mat[j - 1, i])
                ct++;
            else
            {
                if (ct == 5)
                    medie += 3;
                else if (ct > 5)
                    medie += 3 + ct - 5;
                ct = 1;
            }
        }
    }
    return medie;
}
int part2(int[,] mat)
{
    int medie = 0;
    for (int i = 1; i < n; i ++)
    {
        for (int j = 1; j < n; j++)
            if (mat[i, j] == mat[i, j - 1] && mat[i, j - 1] == mat[i - 1, j - 1] && mat[i - 1, j - 1] == mat[i - 1, j])
                medie += 3;
    }
    return medie;
}

int part3(int[,] mat)
{
    int medie = 0;
    for(int i = 0; i < n; i ++)
    {
        int nr = 0, mod = 1 << 11;

        for (int j = 0; j < 11; j++)
            nr = nr * 2 + mat[i, j];
        if (nr == 1488 || nr == 93)
            medie += 40;
        for (int j = 11; j < n; j ++)
        {
            nr = nr % mod * 2 + mat[i, j];
            if (nr == 1488 || nr == 93)
                medie += 40;
        }
    }
    for (int i = 0; i < n; i++)
    {
        int nr = 0, mod = 1 << 11;
        for (int j = 0; j < 11; j++)
            nr = nr * 2 + mat[j, i];
        if (nr == 1488 || nr == 93)
            medie += 40;
        for(int j = 11; j < n; j++)
        {
            nr = nr % mod * 2 + mat[j, i];
            if (nr == 1488 || nr == 93)
                medie += 40;
        }
    }
    return medie;
}
int part4(int[,] mat)
{
    int negru = 0;
    for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++)
            negru += mat[i, j];
    int tot = n * n;
    int m = tot / (negru);
    int mic = m / 5 * 5;
    int mare = (m / 5 + 1) * 5;
    mic -= 50; mare -= 50;
    mic = Math.Abs(mic);
    mare = Math.Abs(mare);
    mic /= 5; mare /= 5;
    int minn = Math.Min(mic, mare);
    minn *= 10;

    return minn;
}
int calculeaza(int[,] mat, int [,] mat1, int n)
{
    int minn = 1000000000;
    int poz = 0;
    for (int mask = 0; mask < 8; mask ++)
    {
        int m1 = 0, m2 = 0, m3 = 0, m4 = 0 ;
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
            {
                mat[i, j] = mat1[i, j];
                //mat[i, j] = 0;
                if (valid(i, j, mask))
                    mat[i, j] ^= 1;
            }
        scriemask(mat, mask, n);
        m1 += part1(mat);
        m2 += part2(mat);
        m3 += part3(mat);
        m4 += part4(mat);
        int medie = m1 + m2 + m3 + m4;
        Console.WriteLine($"Masca: {mask}   {medie}");
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (mat[i, j] == 0) Console.BackgroundColor = ConsoleColor.White;
                else Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("  ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
 
        if (medie < minn)
        {
            minn = medie; poz = mask;
        }
    }

    return poz;
}
void aplicamask(int[,] mat, int mask, int n)
{
    for (int i = 0; i < n; i ++)
    {
        for (int j = 0; j < n; j++)
            if (valid(i, j, mask))
                mat[i, j] ^= 1;
    }
}
void ReadMatrixFromFile(string filePath, int[,] mat, int n)
{
    var test = File.ReadAllLines(filePath);
    if (test == null) return;
    for(int i = 0; i < n; i++)
    {
        for(int j = 0; j < n; j++)
        {
            mat[i, j] = Convert.ToInt32(test[i][j]) - 48;
        }
    }
}

void scriemask(int[,] mat, int mask, int n)
{
    mask ^= 5;
    int p = 1;
    for (int i = 4; i >= 2; i--)
    {
        mat[8, i] = mask % 2;
        mat[n - i -1, 8] = mask % 2;
        mask /= 2; 
        
    }

}
Console.BackgroundColor = ConsoleColor.White;
ReadMatrixFromFile(@"D:\Gabi\Proiecte\Cod QR\Cod QR\input.txt", mat,n);

//for (int i = 0; i < n; i++)
//{
//    for (int j = 0; j < n; j++)
//    {
//        if (IsData(i, j) )Console.BackgroundColor = ConsoleColor.White;
//        else Console.BackgroundColor = ConsoleColor.Black;
//        Console.Write("  ");
//    }
//    Console.WriteLine();
//}
for (int i = 0; i < n; i++)
{
    for (int j = 0; j < n; j++)
    {
        if (mat[i, j] == 0) Console.BackgroundColor = ConsoleColor.White;
        else Console.BackgroundColor = ConsoleColor.Black;
        Console.Write("  ");
    }
    Console.WriteLine();
}
Console.WriteLine();
mask = detmask(mat);
//Console.WriteLine(mask);
maskf(mat, mask, n);
for (int i = 0; i < n; i++)
{
    for (int j = 0; j < n; j++)
    {
        if (mat[i, j] == 0) Console.BackgroundColor = ConsoleColor.White;
        else Console.BackgroundColor = ConsoleColor.Black;
        Console.Write("  ");
    }
    Console.WriteLine();
}
for (int i = 0; i < n; i++)
{
    for (int j = 0; j < n; j++)
    {

        Console.Write(mat[i, j]);
    }
    Console.WriteLine();
}
Console.WriteLine();
int[,] mat1 = new int[n, n];
for (int i = 0; i < n; i++)
{
    for (int j = 0; j < n; j++)
    {

        Console.Write(mat[i, j]);
    }
    Console.WriteLine();
}
Console.WriteLine();

mask = calculeaza(mat1, mat, n);
aplicamask(mat, mask, n);
scriemask(mat, mask, n);
for (int i = 0; i < n; i++)
{
    for (int j = 0; j < n; j++)
    {
        if (mat[i, j] == 0) Console.BackgroundColor = ConsoleColor.White;
        else Console.BackgroundColor = ConsoleColor.Black;
        Console.Write("  ");
    }
    Console.WriteLine();
}
Console.WriteLine();




