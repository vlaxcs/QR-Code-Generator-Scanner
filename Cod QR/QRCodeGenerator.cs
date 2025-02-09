public static class QRCodeGenerator {
    static int[][] code;
    static int version;
    static int ECCLevel;
    static int mask;

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
    static int[][] nrOfDataBlocks = {
        new int[] { 19, 16, 13, 9 },
        new int[] { 34, 28, 22, 16 },
        new int[] { 55, 44, 34, 26 },
        new int[] { 80, 64, 48, 36 },
        new int[] { 108, 86, 62, 46 },
        new int[] { 136, 108, 76, 60 },
        new int[] { 156, 124, 88, 66 },
        new int[] { 194, 154, 110, 86 },
        new int[] { 232, 182, 132, 100 },
        new int[] { 274, 216, 154, 122 },
        new int[] { 324, 254, 180, 140 },
        new int[] { 370, 290, 206, 158 },
        new int[] { 428, 334, 244, 180 },
        new int[] { 461, 365, 261, 197 },
        new int[] { 523, 415, 295, 223 },
        new int[] { 589, 453, 325, 253 },
        new int[] { 647, 507, 367, 283 },
        new int[] { 721, 563, 397, 313 },
        new int[] { 795, 627, 445, 341 },
        new int[] { 861, 669, 485, 385 },
        new int[] { 932, 714, 512, 406 },
        new int[] { 1006, 782, 568, 442 },
        new int[] { 1094, 860, 614, 464 },
        new int[] { 1174, 914, 664, 514 },
        new int[] { 1276, 1000, 718, 538 },
        new int[] { 1370, 1062, 754, 596 },
        new int[] { 1468, 1128, 808, 628 },
        new int[] { 1531, 1193, 871, 661 },
        new int[] { 1631, 1267, 911, 701 },
        new int[] { 1735, 1373, 985, 745 },
        new int[] { 1843, 1455, 1033, 793 },
        new int[] { 1955, 1541, 1115, 845 },
        new int[] { 2071, 1631, 1171, 901 },
        new int[] { 2191, 1725, 1231, 961 },
        new int[] { 2306, 1812, 1286, 986 },
        new int[] { 2434, 1914, 1354, 1054 },
        new int[] { 2566, 1992, 1426, 1096 },
        new int[] { 2702, 2102, 1502, 1142 },
        new int[] { 2812, 2216, 1582, 1222 },
        new int[] { 2956, 2334, 1666, 1276 }
    };
    static int[][] nrOfECBlocks = {
        new int[]{7, 10, 13, 17},
        new int[]{10, 16, 22, 28},
        new int[]{15, 26, 36, 44},
        new int[]{20, 36, 52, 64},
        new int[]{26, 48, 72, 88},
        new int[]{36, 64, 96, 112},
        new int[]{40, 72, 108, 130},
        new int[]{48, 88, 132, 156},
        new int[]{60, 110, 160, 192},
        new int[]{72, 130, 192, 224},
        new int[]{80, 150, 224, 264},
        new int[]{96, 176, 260, 308},
        new int[]{104, 198, 288, 352},
        new int[]{120, 216, 320, 384},
        new int[]{132, 240, 360, 432},
        new int[]{144, 280, 408, 480},
        new int[]{168, 308, 448, 532},
        new int[]{180, 338, 504, 588},
        new int[]{196, 364, 546, 650},
        new int[]{224, 416, 600, 700},
        new int[]{224, 442, 644, 750},
        new int[]{252, 476, 690, 816},
        new int[]{270, 504, 750, 900},
        new int[]{300, 560, 810, 960},
        new int[]{312, 588, 870, 1050},
        new int[]{336, 644, 952, 1110},
        new int[]{360, 700, 1020, 1200},
        new int[]{390, 728, 1050, 1260},
        new int[]{420, 784, 1140, 1350},
        new int[]{450, 812, 1200, 1440},
        new int[]{480, 868, 1290, 1530},
        new int[]{510, 924, 1350, 1620},
        new int[]{540, 980, 1440, 1710},
        new int[]{570, 1036, 1530, 1800},
        new int[]{570, 1064, 1590, 1890},
        new int[]{600, 1120, 1680, 1980},
        new int[]{630, 1204, 1770, 2100},
        new int[]{660, 1260, 1860, 2220},
        new int[]{720, 1316, 1950, 2310},
        new int[]{750, 1372, 2040, 2430}
    };

    public static QRCode Generate(string message, int errorCorrectionLevel = 1, int minVersion = 1, DataType? type = null) {
        // Find the optimal dataType or use the one passed in
        QREncodedMessage encodedMessage = new QREncodedMessage();
        if(type == null) {
            var types = new DataType[] { DataType.Numeric, DataType.Alphanumeric, DataType.Byte, DataType.Kanji };

            bool foundAny = false;
            foreach(var dataType in types) {
                try {
                    encodedMessage = MessageEncoder.EncodeMessage(dataType, message);
                    type = dataType;

                    foundAny = true;

                    break;
                } catch { }
            }

            if(!foundAny) throw new Exception("Couldnt find a compatible encoding data type");
        } else {
            encodedMessage = MessageEncoder.EncodeMessage(type.Value, message);
        }

        version = DetermineFittingVersion(encodedMessage, minVersion) - 1;
        List<byte> res = new List<byte>();

        AppendBits(res, (int)type.Value, 4);
        AppendBits(res, encodedMessage.bitsArray.Length / 8, 10 - (int)Math.Log2((int)type * 2 + 1) + 1);
        for(int i = 0; i < encodedMessage.bitsArray.Length; i++) {
            res.Add(encodedMessage[i]);
        }

        var numberOfBlocks = nrOfDataBlocks[version][ECCLevel];
        var numberOfBits = numberOfBlocks * 8;
        while(res.Count < numberOfBits) {
            res.Add(0);
            if(res.Count % 8 == 0) break;
        }

        // Transform bits to bytes
        for(int i = 0; i < res.Count / 8; i++) {
            for(int b = 0; b < 8; b++) {
                res[i] <<= 1;
                res[i] |= res[i * 8 + b];
            }
        }

        res.RemoveRange(res.Count / 8, res.Count - res.Count / 8);

        while(res.Count < numberOfBlocks) {
            res.Add(0xEC);
            if(res.Count >= numberOfBlocks) break;
            res.Add(0x11);
        }

        var galoisField = new GaloisField(nrOfECBlocks[version][ECCLevel]);
        var finalRes = galoisField.Encode(res.ToArray()).ToList();
        finalRes.Add(0);
        
        version += 1;
        return Generate(finalRes.ToArray(), version, errorCorrectionLevel);
    }

    static QRCode Generate(byte[] dataBlocks, int qrVersion, int errorCorrectionLevel) {
        version = qrVersion;
        version = qrVersion;
        ECCLevel = errorCorrectionLevel;

        code = new int[Utility.SizeForVersion(version)][];
        for(int i = 0; i < code.Length; i++) {
            code[i] = new int[Utility.SizeForVersion(version)];
        }

        PutStripes();
        PutBlocks();
        PutAligmentPoints();
        ApplyVersionBits();
        Console.WriteLine("Setting data blocks...");
        SetAllDataBlocks(dataBlocks);
        Console.WriteLine("Data blocks set");
        Console.WriteLine("Placing best mask...");
        PlaceBestMask();
        Console.WriteLine("Placed best mask");

        return new QRCode(code);
    }


    static int DetermineFittingVersion(QREncodedMessage message, int minimum) {
        var minBlocks = Math.Ceiling(message.bitsArray.Length * 1.0 / 8);
        for(int i = minimum; i <= 40; i++) {
            if(nrOfDataBlocks[i - 1][ECCLevel] >= minBlocks) {
                version = i;
                return version;
            }
        }

        throw new Exception("There is no QR version that this message can fit into");
    }

    static void AppendBits(List<byte> bits, int nr, int nrOfBits) {
        int[] v = new int[nrOfBits];
        int ct = 0;
        while(nr != 0) {
            v[ct++] = nr % 2;
            nr /= 2;
        }
        while(ct < nrOfBits) {
            v[ct++] = 0;
        }

        for(int i = v.Length - 1; i >= 0; i--) {
            bits.Add((byte)v[i]);
        }
    }

    static void PlaceBestMask() {
        int MinScore = 999999999;
        int bestMask = -1;
        for(int i = 0; i <= 7; i++) {
            PutMaskBits(ECCLevel ^ 1, i);
            ApplyMask(i);
            int penaltyScore = CalculatePenaltyScore();
            if(penaltyScore < MinScore) {
                MinScore = penaltyScore;
                bestMask = i;
            }
            ApplyMask(i);
        }
        mask = bestMask;
        PutMaskBits(ECCLevel ^ 1, mask);
        ApplyMask(mask);
    }
    static void ApplyVersionBits() {
        if(version < 7) {
            return;
        }
        version = GetVersionBits(version);
        for(int j = 0; j < 6; j++) {
            for(int i = 0; i < 3; i++) {
                code[code.Length - 11 + i][j] = version >> (18 - i * 6 + j) & 1;
                code[j][code.Length - 11 + i] = version >> (18 - i * 6 + j) & 1;

            }
        }
    }
    static void PutAligmentPoints() {
        var aligmentPoints = GetAlignmentPoints();
        for(int k = 0; k < aligmentPoints.Length; k += 2) {
            var allx = aligmentPoints[k];
            var ally = aligmentPoints[k + 1];
            for(int i = -2; i <= 2; i++) {
                for(int j = -2; j <= 2; j++) {
                    code[allx + i][ally + j] = 1;
                }
            }
            for(int i = -1; i <= 1; i++) {
                for(int j = -1; j <= 1; j++) {
                    code[allx + i][ally + j] = 0;
                }
            }
            code[allx][ally] = 1;
        }
    }
    static void PutBlocks() {
        for(int i = 0; i < 7; i++) {
            for(int j = 0; j < 7; j++) {
                code[i][j] = code[code.Length - 1 - i][j] = code[i][code.Length - 1 - j] = 1;
            }
        }
        for(int i = 1; i < 6; i++) {
            for(int j = 1; j < 6; j++) {
                code[i][j] = code[code.Length - 1 - i][j] = code[i][code.Length - 1 - j] = 0;
            }
        }
        for(int i = 2; i < 5; i++) {
            for(int j = 2; j < 5; j++) {
                code[i][j] = code[code.Length - 1 - i][j] = code[i][code.Length - 1 - j] = 1;
            }
        }
    }
    static void PutStripes() {
        for(int i = 0; i < code.Length; i++) {
            code[6][i] = code[i][6] = i & 1 ^ 1;
        }
    }

    static void PutMaskBits(int ECCLevel, int mask) {
        int maskBits = GetMaskBits(ECCLevel, mask);

        for(int j = 0; j <= 5; j++) {
            code[8][j] = (maskBits >> (14 - j)) & 1;
        }
        for(int j = 6; j <= 7; j++) {
            code[8][j + 1] = (maskBits >> (14 - j)) & 1;
        }
        code[7][8] = (maskBits >> (14 - 8)) & 1;
        for(int i = 0; i <= 5; i++) {
            code[i][8] = (maskBits >> (i)) & 1;
        }
        for(int i = 0; i <= 6; i++) {
            code[code.Length - i - 1][8] = (maskBits >> (14 - i)) & 1;

        }
        for(int j = 0; j <= 7; j++) {
            code[8][code.Length - 1 - j] = (maskBits >> j) & 1;
        }
    }

    static void SetAllDataBlocks(byte[] TotalData) {
        int nr = 0;
        for(int j = code.Length - 1; j >= 1; j -= 2) {
            int nj = j - (j > 6 ? 1 : 2);
            if(j % 4 != 2) {
                for(int i = code.Length - 1; i >= 0; i--) {
                    if(IsData(i, nj + 1)) {
                        code[i][nj + 1] = (TotalData[nr / 8] >> (7 - (nr % 8))) & 1;
                        nr++;
                    }
                    if(IsData(i, nj)) {
                        code[i][nj] = (TotalData[nr / 8] >> (7 - (nr % 8))) & 1;
                        nr++;
                    }
                }
            } else {
                for(int i = 0; i < code.Length; i++) {
                    if(IsData(i, nj + 1)) {
                        code[i][nj + 1] = (TotalData[nr / 8] >> (7 - (nr % 8))) & 1;
                        nr++;
                    }
                    if(IsData(i, nj)) {
                        code[i][nj] = (TotalData[nr / 8] >> (7 - (nr % 8))) & 1;
                        nr++;
                    }
                }
            }
        }
    }
    static bool IsData(int x, int y) {
        int maxWidth = Utility.SizeForVersion(version);
        int[] alligmentPoints = GetAlignmentPoints();

        if(x < 9 && y < 9) return false;

        if(x < 9 && y > maxWidth - 9) return false;
        if(x > maxWidth - 9 && y < 9) return false;

        if(version >= 7 && x < 7 && y > maxWidth - 12) return false;
        if(version >= 7 && y < 7 && x > maxWidth - 12) return false;

        if(x == 6 || y == 6) return false;
        for(int i = 0; i < alligmentPoints.Length; i += 2) {
            if(Math.Abs(alligmentPoints[i] - x) < 3 && Math.Abs(alligmentPoints[i + 1] - y) < 3)
                return false;
        }
        return true;
    }
    static int[] GetAlignmentCoords() {
        if(version <= 1) {
            return new int[0];
        }
        int num = (version / 7) + 2;
        int[] result = new int[num];
        result[0] = 6;
        if(num == 1) {
            for(int i = 0; i < num; i++) {
                result[i] = version * 4 + 16 - result[i];
            }
            return result;
        }
        result[num - 1] = 4 * version + 10;
        if(num == 2) {
            for(int i = 0; i < num; i++) {
                result[i] = version * 4 + 16 - result[i];
            }
            return result;
        }
        result[num - 2] = 2 * ((result[0] + result[num - 1] * (num - 2)) / ((num - 1) * 2));
        if(num == 3) {
            for(int i = 0; i < num; i++) {
                result[i] = version * 4 + 16 - result[i];
            }
            return result;
        }
        int step = result[num - 1] - result[num - 2];
        for(int i = num - 3; i > 0; i--) {
            result[i] = result[i + 1] - step;
        }
        for(int i = 0; i < num; i++) {
            result[i] = version * 4 + 16 - result[i];
        }
        return result;
    }
    static int[] GetAlignmentPoints() {
        int[] AlignmentCoords = GetAlignmentCoords();
        if(AlignmentCoords.Length == 0)
            return new int[0];
        int[] ans = new int[2 * AlignmentCoords.Length * AlignmentCoords.Length - 6];
        int nr = 0;
        for(int i = 0; i < AlignmentCoords.Length; i++) {
            for(int j = 0; j < AlignmentCoords.Length; j++) {
                if(i == AlignmentCoords.Length - 1 && j == AlignmentCoords.Length - 1)
                    continue;
                if(i == 0 && j == AlignmentCoords.Length - 1)
                    continue;
                if(j == 0 && i == AlignmentCoords.Length - 1)
                    continue;

                ans[nr++] = AlignmentCoords[i];
                ans[nr++] = AlignmentCoords[j];
            }
        }

        return ans;
    }
    static int GetMaskBits(int ECCLevel, int maskPattern) {
        int nr = (ECCLevel << 3) + maskPattern;
        int initnr = nr;
        nr <<= 10;
        int gen = 0x0537; // 10100110111 is the generator=0x0537
        int initGen = gen;
        while(nr.MostSignificantBit() >= 1024) {
            while(gen.MostSignificantBit() < nr.MostSignificantBit()) {
                gen <<= 1;
            }
            nr ^= gen;
            gen = initGen;
        }
        nr += initnr << 10;
        nr ^= 0x5412;
        return nr;
    }

    static void ApplyMask(int mask) {
        for(int i = 0; i < code.Length; i++) {
            for(int j = 0; j < code.Length; j++) {
                if(!IsData(i, j) || !masks[mask](i, j)) continue;
                code[i][j] ^= 1;
            }
        }
    }
    static int GetVersionBits(int Version) { //TODO:TEST THIS 
        if(version < 7 || version > 40)
            throw new Exception("Version is not good");
        int initnr = Version;
        Version <<= 12;
        int gen = 0x1F25; // 1111100100101 is the generator=0x1F25
        int initGen = gen;
        while(Version.MostSignificantBit() >= 4096) {
            while(gen.MostSignificantBit() < Version.MostSignificantBit()) {
                gen <<= 1;
            }
            Version ^= gen;
            gen = initGen;
        }
        Version += initnr << 12;
        return Version;
    }


    static int CalculatePenaltyScore() {
        int consecutives = 0;
        int score = 0;

        // Calculate hroziontal consecutives penalties
        for(int i = 0; i < code.Length; i++) {
            if(consecutives == 5) score += 3;
            else if(consecutives > 5) score++;
            consecutives = 1;
            for(int j = 1; j < code.Length; j++) {
                if(code[i][j] == code[i][j - 1]) consecutives++;
                else {
                    if(consecutives == 5) score += 3;
                    else if(consecutives > 5) score += 3 + consecutives - 5;
                    consecutives = 1;
                }
            }
        }
        // Calculate vertical consecutives penalties
        for(int i = 0; i < code.Length; i++) {
            if(consecutives == 5) score += 3;
            else if(consecutives > 5) score++;
            consecutives = 1;
            for(int j = 1; j < code.Length; j++) {
                if(code[j][i] == code[j - 1][i]) consecutives++;
                else {
                    if(consecutives == 5) score += 3;
                    else if(consecutives > 5) score += 3 + consecutives - 5;
                    consecutives = 1;
                }
            }
        }

        // Calculate 2x2 penalties
        for(int i = 1; i < code.Length; i++) {
            for(int j = 1; j < code.Length; j++) {
                if(code[i][j] == code[i][j - 1] && code[i][j - 1] == code[i - 1][j - 1] && code[i - 1][j - 1] == code[i - 1][j])
                    score += 3;
            }
        }

        // Horizontal finder-like penalties
        for(int i = 0; i < code.Length; i++) {
            int nr = 0, mod = 1 << 11;

            for(int j = 0; j < 11; j++)
                nr = nr * 2 + code[i][j];
            if(nr == 1488 || nr == 93)
                score += 40;
            for(int j = 11; j < code.Length; j++) {
                nr = nr % mod * 2 + code[i][j];
                if(nr == 1488 || nr == 93)
                    score += 40;
            }
        }
        // Vertical finder-like penalties
        for(int i = 0; i < code.Length; i++) {
            int nr = 0, mod = 1 << 11;
            for(int j = 0; j < 11; j++)
                nr = nr * 2 + code[j][i];
            if(nr == 1488 || nr == 93)
                score += 40;
            for(int j = 11; j < code.Length; j++) {
                nr = nr % mod * 2 + code[j][i];
                if(nr == 1488 || nr == 93)
                    score += 40;
            }
        }

        // Dark/light balance penalties
        int darkModules = 0;
        for(int i = 0; i < code.Length; i++)
            for(int j = 0; j < code.Length; j++)
                darkModules += code[i][j];
        int totalModules = code.Length * code.Length;
        int m = totalModules / (darkModules);
        int intervalStart = m / 5 * 5;
        int intervalEnd = (m / 5 + 1) * 5;
        intervalStart -= 50; intervalEnd -= 50;
        intervalStart = Math.Abs(intervalStart);
        intervalEnd = Math.Abs(intervalEnd);
        intervalStart /= 5; intervalEnd /= 5;

        score += Math.Min(intervalStart, intervalEnd) * 10;

        return score;
    }
}