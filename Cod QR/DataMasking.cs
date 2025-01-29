using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public  class DataMasking {
     Func<int, int, bool>[] masks = {
        (i, j) => (i + j) % 2 == 0,                     // Mask 0
        (i, j) => i % 2 == 0,                           // Mask 1
        (i, j) => j % 3 == 0,                           // Mask 2
        (i, j) => (i + j) % 3 == 0,                     // Mask 3
        (i, j) => (i / 2 + j / 3) % 2 == 0,             // Mask 4
        (i, j) => i * j % 2 + i * j % 3 == 0,           // Mask 5
        (i, j) => (i * j % 2 + i * j % 3) % 2 == 0,     // Mask 6
        (i, j) => ((i + j) % 2 + i * j % 3) % 2 == 0    // Mask 7
    };
}

/*
 int part1(int[,] mat) {
    int ct = 0;
    int medie = 0;
    for(int i = 0; i < n; i++) {
        if(ct == 5)
            medie += 3;
        else if(ct > 5)
            medie++;
        ct = 1;
        for(int j = 1; j < n; j++) {
            if(mat[i, j] == mat[i, j - 1])
                ct++;
            else {
                if(ct == 5)
                    medie += 3;
                else if(ct > 5)
                    medie += 3 + ct - 5;
                ct = 1;
            }
        }
    }
    for(int i = 0; i < n; i++) {
        if(ct == 5)
            medie += 3;
        else if(ct > 5)
            medie++;
        ct = 1;
        for(int j = 1; j < n; j++) {
            if(mat[j, i] == mat[j - 1, i])
                ct++;
            else {
                if(ct == 5)
                    medie += 3;
                else if(ct > 5)
                    medie += 3 + ct - 5;
                ct = 1;
            }
        }
    }
    return medie;
}
int part2(int[,] mat) {
    int medie = 0;
    for(int i = 1; i < n; i++) {
        for(int j = 1; j < n; j++)
            if(mat[i, j] == mat[i, j - 1] && mat[i, j - 1] == mat[i - 1, j - 1] && mat[i - 1, j - 1] == mat[i - 1, j])
                medie += 3;
    }
    return medie;
}

int part3(int[,] mat) {
    int medie = 0;
    for(int i = 0; i < n; i++) {
        int nr = 0, mod = 1 << 11;

        for(int j = 0; j < 11; j++)
            nr = nr * 2 + mat[i, j];
        if(nr == 1488 || nr == 93)
            medie += 40;
        for(int j = 11; j < n; j++) {
            nr = nr % mod * 2 + mat[i, j];
            if(nr == 1488 || nr == 93)
                medie += 40;
        }
    }
    for(int i = 0; i < n; i++) {
        int nr = 0, mod = 1 << 11;
        for(int j = 0; j < 11; j++)
            nr = nr * 2 + mat[j, i];
        if(nr == 1488 || nr == 93)
            medie += 40;
        for(int j = 11; j < n; j++) {
            nr = nr % mod * 2 + mat[j, i];
            if(nr == 1488 || nr == 93)
                medie += 40;
        }
    }
    return medie;
}
int part4(int[,] mat) {
    int negru = 0;
    for(int i = 0; i < n; i++)
        for(int j = 0; j < n; j++)
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
int calculeaza(int[,] mat, int[,] mat1, int n) {
    int minn = 1000000000;
    int poz = 0;
    for(int mask = 0; mask < 8; mask++) {
        int m1 = 0, m2 = 0, m3 = 0, m4 = 0;
        for(int i = 0; i < n; i++)
            for(int j = 0; j < n; j++) {
                mat[i, j] = mat1[i, j];
                
                if(valid(i, j, mask))
                    mat[i, j] ^= 1;
            }
        scriemask(mat, mask, n);
        m1 += part1(mat);
        m2 += part2(mat);
        m3 += part3(mat);
        m4 += part4(mat);
        int medie = m1 + m2 + m3 + m4;
        Console.WriteLine($"Masca: {mask}   {medie}");
        for(int i = 0; i < n; i++) {
            for(int j = 0; j < n; j++) {
                if(mat[i, j] == 0) Console.BackgroundColor = ConsoleColor.White;
                else Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("  ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();

        if(medie < minn) {
            minn = medie; poz = mask;
        }
    }

    return poz;
}
void aplicamask(int[,] mat, int mask, int n) {
    for(int i = 0; i < n; i++) {
        for(int j = 0; j < n; j++)
            if(valid(i, j, mask))
                mat[i, j] ^= 1;
    }
}
void ReadMatrixFromFile(string filePath, int[,] mat, int n) {
    var test = File.ReadAllLines(filePath);
    if(test == null) return;
    for(int i = 0; i < n; i++) {
        for(int j = 0; j < n; j++) {
            mat[i, j] = Convert.ToInt32(test[i][j]) - 48;
        }
    }
}

void scriemask(int[,] mat, int mask, int n) {
    mask ^= 5;
    int p = 1;
    for(int i = 4; i >= 2; i--) {
        mat[8, i] = mask % 2;
        mat[n - i - 1, 8] = mask % 2;
        mask /= 2;

    }

}
Console.BackgroundColor = ConsoleColor.White;
ReadMatrixFromFile(@"..\..\..\input.txt", mat, n);

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
for(int i = 0; i < n; i++) {
    for(int j = 0; j < n; j++) {
        if(mat[i, j] == 0) Console.BackgroundColor = ConsoleColor.White;
        else Console.BackgroundColor = ConsoleColor.Black;
        Console.Write("  ");
    }
    Console.WriteLine();
}
Console.WriteLine();
mask = detmask(mat);
//Console.WriteLine(mask);
maskf(mat, mask, n);
for(int i = 0; i < n; i++) {
    for(int j = 0; j < n; j++) {
        if(mat[i, j] == 0) Console.BackgroundColor = ConsoleColor.White;
        else Console.BackgroundColor = ConsoleColor.Black;
        Console.Write("  ");
    }
    Console.WriteLine();
}
for(int i = 0; i < n; i++) {
    for(int j = 0; j < n; j++) {

        Console.Write(mat[i, j]);
    }
    Console.WriteLine();
}
Console.WriteLine();
int[,] mat1 = new int[n, n];
for(int i = 0; i < n; i++) {
    for(int j = 0; j < n; j++) {

        Console.Write(mat[i, j]);
    }
    Console.WriteLine();
}
Console.WriteLine();

mask = calculeaza(mat1, mat, n);
aplicamask(mat, mask, n);
scriemask(mat, mask, n);
for(int i = 0; i < n; i++) {
    for(int j = 0; j < n; j++) {
        if(mat[i, j] == 0) Console.BackgroundColor = ConsoleColor.White;
        else Console.BackgroundColor = ConsoleColor.Black;
        Console.Write("  ");
    }
    Console.WriteLine();
}
Console.WriteLine();
 */