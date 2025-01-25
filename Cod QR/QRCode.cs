using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


namespace Cod_QR {
    internal partial class QRCode {
        bool isDark(Rgba32 pixel) {
            return (pixel.R + pixel.G + pixel.B) / 3 < 120;
        }

        public QRCode(string imagePath) {
            var img = Image.Load<Rgba32>(imagePath);

            var p = DeterminePadding(img);
            Console.WriteLine(p.top);
        }

        private Padding DeterminePadding(Image<Rgba32> img) {
            Padding padding = new Padding();
            return padding;
        }
    }
}
