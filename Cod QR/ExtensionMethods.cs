using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cod_QR {
    public static class ExtensionMethods {
        public static int[][] CopyArrayLinq(this int[][] source) {
            return source.Select(s => s.ToArray()).ToArray();
        }
    }
}
