using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DataMasking {
    static Func<int, int, bool>[] masks = {
        (i, j) => (i + j) % 2 == 0,                     // Mask 0
        (i, j) => i % 2 == 0,                           // Mask 1
        (i, j) => j % 3 == 0,                           // Mask 2
        (i, j) => (i + j) % 3 == 0,                     // Mask 3
        (i, j) => (i / 2 + j / 3) % 2 == 0,             // Mask 4
        (i, j) => i * j % 2 + i * j % 3 == 0,           // Mask 5
        (i, j) => (i * j % 2 + i * j % 3) % 2 == 0,     // Mask 6
        (i, j) => ((i + j) % 2 + i * j % 3) % 2 == 0    // Mask 7
    };

    public static void ApplyMask()
}
