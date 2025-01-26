using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Cod_QR {
    class QRCodeImageParser {
        const int BRIGHTNESS_THRESHOLD = 120;
        static bool[][] alignmentMarker = {
            new bool[] {true, true , true , true , true , true , true},
            new bool[] {true, false, false, false, false, false, true},
            new bool[] {true, false, true , true , true , false, true},
            new bool[] {true, false, true , true , true , false, true},
            new bool[] {true, false, true , true , true , false, true},
            new bool[] {true, false, false, false, false, false, true},
            new bool[] {true, true , true , true , true , true , true}
        };

        bool isDark(Rgba32 pixel) {
            return (pixel.R + pixel.G + pixel.B) / 3 < BRIGHTNESS_THRESHOLD;
        }

        public QRCodeImageParser(string imagePath) {
            var img = Image.Load<Rgba32>(imagePath);

            var bounds = DetermineBounds(img);

            int pixelSize = DeterminePixelSize(img, bounds);

            // Create 2D array
            int n = (bounds.right - bounds.left) / pixelSize;
            bool[][] rawQR = new bool[n][];
            for(int i = 0; i < n; i++) {
                rawQR[i] = new bool[n];
                for(int j = 0; j < n; j++) {
                    rawQR[i][j] = DetermineModule(img, bounds, i, j, pixelSize);

                    if(rawQR[i][j]) Console.BackgroundColor = ConsoleColor.Black;
                    else Console.BackgroundColor = ConsoleColor.White;

                    Console.Write("  ");
                }
                Console.WriteLine();
            }

            //TODO: orientation check
        }

        private void PrintImage(Image<Rgba32> img) {
            for(int i = 0; i < img.Height; i++) {
                for(int j = 0; j < img.Width; j++) {
                    PrintPixel(img[j, i]);
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();
            }
        }
        private void PrintImage(Image<Rgba32> img, Bounds bounds) {
            for(int i = bounds.top; i < bounds.bottom; i++) {
                for(int j = bounds.left; j < bounds.right; j++) {
                    PrintPixel(img[j, i]);
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();
            }
        }


        private void PrintPixel(Rgba32 pixel) {
            if(isDark(pixel)) {
                Console.BackgroundColor = ConsoleColor.Black;
            } else {
                Console.BackgroundColor = ConsoleColor.White;
            }
            Console.Write("  ");
        }

        private Bounds DetermineBounds(Image<Rgba32> img) {
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

        private int DeterminePixelSize(Image<Rgba32> img, Bounds bounds) {
            int pixelSize = 0, aux = 0;
            for(int i = 0; i < img.Width; i++) {
                if(!isDark(img[bounds.left + i, bounds.top + i])) break;
                pixelSize++;
            }
            for(int i = 0; i < img.Width; i++) {
                if(!isDark(img[bounds.left + i, bounds.bottom - 1 - i])) break;
                aux++;
            }
            if(pixelSize == 0) pixelSize = aux;
            else pixelSize = Math.Min(pixelSize, aux);

            return pixelSize;
        }

        private bool DetermineModule(Image<Rgba32> img, Bounds bounds, int i, int j, int pixelSize) {
            int whites = 0, blacks = 0;
            int im = (i + 1) * pixelSize; // i max
            int jm = (j + 1) * pixelSize; // j max
            for(int x = j * pixelSize; x < jm; x++) {
                for(int y = i * pixelSize; y < im; y++) {
                    if(isDark(img[x + bounds.left, y + bounds.top])) blacks++;
                    else whites++;
                }
            }
            return blacks > whites / 2;
        }
    }
}
