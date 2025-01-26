using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Cod_QR {
    public static class QRCodeImageParser {
        const int BRIGHTNESS_THRESHOLD = 30;

        public static int[][] Parse(string imagePath) {
            Console.SetCursorPosition(0, 0);
            var img = Image.Load<Rgba32>(imagePath);

            var bounds = DetermineBounds(img);

            int pixelSize = DeterminePixelSize(img, bounds);

            int[][] rawQR = ExtractJaggedArray(img, bounds, pixelSize);

            return CheckOrientation(rawQR);
        }



        static bool isDark(Rgba32 pixel) {
            return (pixel.R + pixel.G + pixel.B) / 3 < BRIGHTNESS_THRESHOLD;
        }
        //static void PrintImage(Image<Rgba32> img) {
        //    for(int i = 0; i < img.Height; i++) {
        //        for(int j = 0; j < img.Width; j++) {
        //            PrintPixel(img[j, i]);
        //        }
        //        Console.BackgroundColor = ConsoleColor.Black;
        //        Console.WriteLine();
        //    }
        //}
        //static void PrintImage(Image<Rgba32> img, Bounds bounds) {
        //    for(int i = bounds.top; i < bounds.bottom; i++) {
        //        for(int j = bounds.left; j < bounds.right; j++) {
        //            PrintPixel(img[j, i]);
        //        }
        //        Console.BackgroundColor = ConsoleColor.Black;
        //        Console.WriteLine();
        //    }
        //}
        //static void PrintImage(int[][] img) {
        //    int n = img.Length;
        //    Console.BackgroundColor = ConsoleColor.White;
        //    Console.WriteLine("                                             ");
        //    for(int i = 0; i < n; i++) {
        //        Console.BackgroundColor = ConsoleColor.White;
        //        Console.Write("  ");
        //        for(int j = 0; j < n; j++) {

        //            if(img[i][j] == 1) Console.BackgroundColor = ConsoleColor.Black;
        //            else Console.BackgroundColor = ConsoleColor.White;
        //            Console.Write("  ");
        //            Console.BackgroundColor = ConsoleColor.Black;
        //        }
        //        Console.BackgroundColor = ConsoleColor.White;
        //        Console.Write("  ");
        //        Console.BackgroundColor = ConsoleColor.Black;
        //        Console.WriteLine();
        //    }
        //    Console.BackgroundColor = ConsoleColor.White;
        //    Console.WriteLine("                                             ");
        //    Console.BackgroundColor = ConsoleColor.Black;
        //}
        //static void PrintPixel(Rgba32 pixel) {
        //    if(isDark(pixel)) {
        //        Console.BackgroundColor = ConsoleColor.Black;
        //    } else {
        //        Console.BackgroundColor = ConsoleColor.White;
        //    }
        //    Console.Write("  ");
        //    Console.BackgroundColor = ConsoleColor.Black;
        //}



        static Bounds DetermineBounds(Image<Rgba32> img) {
            Bounds bounds = new Bounds();

            // Top bound
            int i = 0;
            bool isEmpty = true;
            while(i < img.Height) {
                for(int j = 0; j < img.Width; j++) {
                    if(!isDark(img[j, i])) continue;
                    isEmpty = false;
                    break;
                }

                if(!isEmpty) break;
                i++;
            }
            bounds.top = i;

            // Bottom bound
            i = img.Height - 1;
            isEmpty = true;
            while(i >= 0) {
                for(int j = 0; j < img.Width; j++) {
                    if(!isDark(img[j, i])) continue;
                    isEmpty = false;
                    break;
                }

                if(!isEmpty) break;
                i--;
            }
            bounds.bottom = i + 1;

            // Left bound
            i = 0;
            isEmpty = true;
            while(i < img.Width) {
                for(int j = 0; j < img.Height; j++) {
                    if(!isDark(img[i, j])) continue;
                    isEmpty = false;
                    break;
                }

                if(!isEmpty) break;
                i++;
            }
            bounds.left = i;

            // Right bound
            i = img.Width - 1;
            isEmpty = true;
            while(i >= 0) {
                for(int j = 0; j < img.Height; j++) {
                    if(!isDark(img[i, j])) continue;
                    isEmpty = false;
                    break;
                }

                if(!isEmpty) break;
                i--;
            }
            bounds.right = i + 1;

            return bounds;
        }
        static int DeterminePixelSize(Image<Rgba32> img, Bounds bounds) {
            int pixelSize = 0, aux = 0;
            for(int i = 0; i < img.Width; i++) {
                if(!isDark(img[bounds.left + i, bounds.top + i])) break;
                pixelSize++;
            }
            for(int i = 0; i < img.Width; i++) {
                if(!isDark(img[bounds.left + i, bounds.bottom - 1 - i])) break;
                aux++;
            }
            if (pixelSize == 0) pixelSize = aux;
            else if(aux != 0) pixelSize = Math.Min(pixelSize, aux);

            return pixelSize;
        }
        static int[][] ExtractJaggedArray(Image<Rgba32> img, Bounds bounds, int pixelSize) {
            int n = (bounds.right - bounds.left) / pixelSize;
            int[][] rawQR = new int[n][];
            for(int i = 0; i < n; i++) {
                rawQR[i] = new int[n];
                for(int j = 0; j < n; j++) {
                    if(i == 25 && j == 25) {
                        n = n + 1 - 1;
                    }

                    rawQR[i][j] = isDark(img[j * pixelSize + bounds.left + pixelSize / 2, i * pixelSize + bounds.top + pixelSize / 2]) ? 1 : 0;
                }
            }

            return rawQR;
        }


        static readonly Func<int, int, int, (int, int)>[] OrientationLUT = {
            (i, j, n) => (i, j), // Empty corner Bottom Right
            (i, j, n) => (j, n - i - 1), // Empty corner Bottom Left
            (i, j, n) => (n - i - 1, n - j - 1), // Empty corner Top Left
            (i, j, n) => (n - j - 1, i), // Empty corner Top Right
        };
        static int[][] CheckOrientation(int[][] rawQR) {
            int n = rawQR.Length;
            bool bottomSymmetric = true, rightSymmetric = true;
            // Check if bottom two alignment patterns match
            for(int i = 0; i < 7; i++) {
                for(int j = 0; j < 7; j++) {
                    if(rawQR[n - i - 1][j] != rawQR[n - i - 1][n - j - 1]) bottomSymmetric = false;
                    if(rawQR[i][n - j - 1] != rawQR[n - i - 1][n - j - 1]) rightSymmetric = false;
                }
            }

            // 0 is correct, 1 is with empty corner bottom left, 2 is with empty corner top left, 3 is with empty corner top right
            int orientation = 0;
            if(!bottomSymmetric && rightSymmetric) orientation = 1;
            if(bottomSymmetric && rightSymmetric) orientation = 2;
            if(bottomSymmetric && !rightSymmetric) orientation = 3;

            var Traverse = OrientationLUT[orientation];

            int[][] correctQR = new int[n][];
            for(int i = 0; i < n; i++) {
                correctQR[i] = new int[n];
                for(int j = 0; j < n; j++) {
                    var (ni, nj) = Traverse(i, j, n);
                    correctQR[i][j] = rawQR[ni][nj];
                }
            }

            return correctQR;
        }
    }
}
