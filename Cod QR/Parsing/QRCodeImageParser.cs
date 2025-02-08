using Cod_QR;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public static class QRCodeImageParser {
    const int BRIGHTNESS_THRESHOLD = 5;

    public static QRCode Parse(string imagePath) {
        var img = Image.Load<Rgba32>(imagePath);

        var bounds = DetermineBounds(img);

        int pixelSize = DeterminePixelSize(img, bounds);

        int[][] rawQR = ExtractJaggedArray(img, bounds, pixelSize);

        rawQR = CheckOrientation(rawQR);

        return new QRCode(rawQR);
    }



    static bool IsDark(Rgba32 pixel) {
        return (pixel.R + pixel.G + pixel.B) / 3 < BRIGHTNESS_THRESHOLD;
    }




    static Bounds DetermineBounds(Image<Rgba32> img) {
        Bounds bounds = new Bounds();

        // Top bound
        int i = 0;
        bool isEmpty = true;
        while(i < img.Height) {
            for(int j = 0; j < img.Width; j++) {
                if(!IsDark(img[j, i])) continue;
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
                if(!IsDark(img[j, i])) continue;
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
                if(!IsDark(img[i, j])) continue;
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
                if(!IsDark(img[i, j])) continue;
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
            if(!IsDark(img[bounds.left + i, bounds.top + i])) {
                if(pixelSize == 0) continue;
                break;
            }
            pixelSize++;
        }
        for(int i = 0; i < img.Width; i++) {
            if(!IsDark(img[bounds.left + i, bounds.bottom - 1 - i])) {
                if(pixelSize == 0) continue;
                break;
            }
            aux++;
        }
        if(pixelSize == 0) pixelSize = aux;
        else if(aux != 0) pixelSize = Math.Min(pixelSize, aux);

        return pixelSize;
    }
    static int[][] ExtractJaggedArray(Image<Rgba32> img, Bounds bounds, int pixelSize) {
        int size = (bounds.right - bounds.left) / pixelSize;

        int v = (int)Math.Round((float)(size - 17) / 4, MidpointRounding.ToZero);

        size = v * 4 + 17;

        float xPixelSize = (float)(bounds.right - bounds.left) / size;
        float yPixelSize = (float)(bounds.bottom - bounds.top) / size;

        int[][] rawQR = new int[size][];
        for(int i = 0; i < size; i++) {
            rawQR[i] = new int[size];
            for(int j = 0; j < size; j++) {
                if(i == 25 && j == 25) {
                    size = size + 1 - 1;
                }

                var x = bounds.left + (int)Math.Round(xPixelSize * j + xPixelSize / 2, MidpointRounding.AwayFromZero);
                var y = bounds.top + (int)Math.Round(yPixelSize * i + yPixelSize / 2, MidpointRounding.AwayFromZero);


                var pixel = img[x, y];
                rawQR[i][j] = IsDark(pixel) ? 1 : 0;
            }
        }
        Console.ForegroundColor = ConsoleColor.Gray;

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
