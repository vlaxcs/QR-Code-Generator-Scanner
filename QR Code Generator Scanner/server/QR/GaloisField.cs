using System.Text;

public class GaloisField
{
    public int[] exp = new int[512];
    public int[] log = new int[256];
    int nsym;

    public GaloisField(int nsym = 10)
    {
        this.nsym = nsym;
        GeneratePolynomials();
    }

    // generates the log/antilog values
    // https://www.thonky.com/qr-code-tutorial/log-antilog-table
    public void GeneratePolynomials()
    {

        exp[0] = 1;
        int x = 2;

        for (int i = 1; i < 256; ++i, x <<= 1)
        {
            if ((x & 0x100) != 0)
            {
                x ^= 0x11d;
            }
            
            exp[i] = x; log[x] = i;
        }

        for (int i = 255; i < 512; ++i)
        {
            exp[i] = exp[i - 255];
        }
    }

    // multiplies two numbers x * y using logs and antilogs
    // https://www.thonky.com/qr-code-tutorial/error-correction-coding#step-6-understand-multiplication-with-logs-and-antilogs
    public int Multiply(int x, int y)
    {

        if (x == 0 || y == 0)
        {
            return 0;
        }

        return exp[log[x] + log[y]];
    }

    // divides two numbers x / y using logs and antilogs
    // https://www.thonky.com/qr-code-tutorial/error-correction-coding#step-6-understand-multiplication-with-logs-and-antilogs
    public int Divide(int x, int y)
    {
        if (y == 0)
        {
            throw new Exception("Division by zero");
        }

        if (x == 0)
        {
            return 0;
        }

        return exp[log[x] + 255 - log[y]];
    }


    // multiplies two polynomials P and Q
    // https://www.thonky.com/qr-code-tutorial/error-correction-coding#step-6-understand-multiplication-with-logs-and-antilogs
    public int[] PolyMul(int[] p, int[] q)
    {
        int rLen = p.Length + q.Length - 1;
        int[] r = new int[rLen];

        for (int i = 0; i < rLen; ++i)
        {
            r[i] = 0;
        }

        for (int j = 0; j < q.Length; ++j)
        {
            for (int i = 0; i < p.Length; ++i)
            {
                r[i + j] ^= Multiply(p[i], q[j]);
            }
        }

        return r;
    }

    // sums up two polynomials P and Q
    // https://www.thonky.com/qr-code-tutorial/error-correction-coding#step-6-understand-multiplication-with-logs-and-antilogs
    public int[] PolyAdd(int[] p, int[] q)
    {
        int pLen = p.Length, qLen = q.Length;
        int rLen = Math.Max(pLen, qLen);
        int[] r = new int[rLen];

        for (int i = 0; i < pLen; ++i)
        {
            r[i + rLen - pLen] = p[i];
        }

        for (int i = 0; i < qLen; ++i)
        {
            r[i + rLen - qLen] ^= q[i];
        }

        return r;
    }

    // scales a polynomial by x
    public int[] PolyScale(int[] p, int x)
    {
        int pLen = p.Length;
        int[] result = new int[pLen];

        for (int i = 0; i < pLen; ++i)
        {
            result[i] = Multiply(p[i], x);
        }

        return result;
    }

    // scales a polynomial by x
    public int[] PolyScale(List<int> p, int x)
    {

        int pLen = p.Count;
        int[] result = new int[pLen];

        for (int i = 0; i < pLen; ++i)
        {
            result[i] = Multiply(p[i], x);
        }

        return result;
    }

    // evaluates a polynomial at a given position x
    public int PolyEval(int[] p, int x)
    {
        int y = p[0];
        for (int i = 1; i < p.Length; i++)
        {
            y = Multiply(y, x) ^ p[i];
        }

        return y;
    }

    // generates the Reed Solomon polynomial according to QR's number of error correction blocks
    public int[] RSGeneratorPoly(int nsym)
    {
        int[] g = new int[] { 1 };

        for (int i = 0; i < nsym; i++)
        {
            g = PolyMul(g, new int[] { 1, exp[i] });
        }

        return g;
    }

    public int[] RSEncodeMsg(int[] inputMessage, int nsym)
    {
        if (inputMessage.Length + nsym > 255)
        {
            throw new Exception("Message too long");
        }

        int[] gen = RSGeneratorPoly(nsym);
        int[] outputMessage = new int[inputMessage.Length + nsym];

        for (int i = 0; i < inputMessage.Length; i++)
        {
            outputMessage[i] = inputMessage[i];
        }

        for (int i = 0; i < inputMessage.Length; i++)
        {
            int coef = outputMessage[i];

            if (coef != 0)
            {
                for (int j = 0; j < gen.Length; j++)
                {
                    outputMessage[i + j] ^= Multiply(gen[j], coef);
                }
            }
        }

        for (int i = 0; i < inputMessage.Length; i++)
        {
            outputMessage[i] = inputMessage[i];
        }

        return outputMessage;
    }

    public int[] RSCalcSyndromes(int[] msg, int nsym)
    {
        int[] synd = new int[nsym];

        for (int i = 0; i < nsym; i++)
        {
            synd[i] = PolyEval(msg, exp[i]);
        }

        return synd;
    }

    public int[] RSForneySyndromes(int[] synd, int[] pos, int nmess)
    {
        int[] fsynd = new int[synd.Length];
        Array.Copy(synd, fsynd, synd.Length);

        for (int i = 0; i < pos.Length; i++)
        {
            int x = exp[nmess - 1 - pos[i]];

            for (int j = 0; j < fsynd.Length - 1; j++)
            {
                fsynd[j] = Multiply(fsynd[j], x) ^ fsynd[j + 1];
            }

            Array.Resize(ref fsynd, synd.Length - 1);
        }

        return fsynd;
    }

    public void RSCorrectErrata(int[] msg, int[] synd, int[] pos)
    {
        int[] q = new int[] { 1 };

        for (int i = 0; i < pos.Length; i++)
        {
            int x = exp[msg.Length - 1 - pos[i]];
            q = PolyMul(q, new int[] { x, 1 });
        }

        var p = synd.Take(pos.Length).ToArray();
        p = p.Reverse().ToArray();
        p = PolyMul(p, q);

        p = p[^pos.Length..];

        // formal derivative of error locator eliminates even terms
        q = q.Where((_, index) => (q.Length & 1) == index % 2).ToArray();

        for (int i = 0; i < pos.Length; i++)
        {
            int x = exp[pos[i] + 256 - msg.Length];
            int y = PolyEval(p, x);
            int z = PolyEval(q, Multiply(x, x));
            msg[pos[i]] ^= Divide(y, Multiply(x, z));
        }
    }

    public int[]? RSFindErrors(int[] synd, int nmess)
    {
        int[] err_poly = { 1 };
        List<int> old_poly = new List<int>() { 1 };
        for (int i = 0; i < synd.Length; i++)
        {
            old_poly.Add(0);
            int delta = synd[i];
            for (int j = 1; j < err_poly.Length; j++)
            {
                delta ^= Multiply(err_poly[err_poly.Length - 1 - j], synd[i - j]);
            }

            if (delta != 0)
            {
                if (old_poly.Count > err_poly.Length)
                {
                    int[] new_poly = PolyScale(old_poly, delta);
                    old_poly = PolyScale(err_poly, Divide(1, delta)).ToList();
                    err_poly = new_poly;
                }

                err_poly = PolyAdd(err_poly, PolyScale(old_poly, delta));
            }
        }

        int errs = err_poly.Length - 1;
        if (errs * 2 > synd.Length) throw new Exception("Too many errors to correct");
        List<int> err_pos = new List<int>();
        for (int i = 0; i < nmess; i++)
        {
            if (PolyEval(err_poly, exp[255 - i]) == 0)
            {
                err_pos.Add(nmess - i - 1);
            }
        }

        if (err_pos.Count != errs)
        {
            return null;
        }

        return err_pos.ToArray();
    }

    public int[] RSCorrectMsg(byte[] msg_in, int nsym)
    {

        if (msg_in.Length > 255)
        {
            throw new Exception("Message too long");
        }

        int[] msg_out = msg_in.Select((x) => (int)x).ToArray();

        // Find erasures
        List<int> erase_pos = new List<int>();
        for (int i = 0; i < msg_out.Length; i++)
        {
            if (msg_out[i] >= 0)
            {
                continue;
            }

            msg_out[i] = 0;
            erase_pos.Add(i);
        }

        if (erase_pos.Count > nsym)
        {
            throw new Exception("Too many erasures to correct");
        }

        int[] synd = RSCalcSyndromes(msg_out, nsym);

        if (synd.Max() == 0)
        {
            return msg_out.Take(msg_out.Length - nsym).ToArray();
        }

        int[] fsynd = RSForneySyndromes(synd, erase_pos.ToArray(), msg_out.Length);
        int[] errPos = RSFindErrors(fsynd, msg_out.Length);

        if (errPos == null)
        {
            throw new Exception("Could not locate error");
        }

        var res = new int[erase_pos.Count + errPos.Length];
        erase_pos.CopyTo(res, 0);
        errPos.CopyTo(res, erase_pos.Count);
        RSCorrectErrata(msg_out, synd, res);

        synd = RSCalcSyndromes(msg_out, nsym);

        if (synd.Max() > 0)
        {
            throw new Exception("Could not correct message");
        }

        return msg_out.Take(msg_out.Length - nsym).ToArray();
    }

    public byte[] Decode(byte[] data)
    {
        // decodes the binary data, chunk by chunk
        var dec = new List<byte>();
        for (int i = 0; i < data.Length; i += 255)
        {
            // creates the chunk slicing data array
            var chunk = data[i..Math.Clamp(i + 255, 0, data.Length)];

            // decodes current chunk and append the result to the final decoding
            dec.AddRange(RSCorrectMsg(chunk, nsym).Select((x) => (byte)x));
        }

        return dec.ToArray();
    }

    public byte[] Decode(string data)
    {
        // converts the input string to a byte array (using UTF-8 encoding)
        return Decode(Encoding.UTF8.GetBytes(data));
    }

    public byte[] Encode(byte[] data)
    {
        // enncodes the binary data, chunk by chunk
        int chunk_size = 255 - nsym;
        var enc = new List<byte>();

        for (int i = 0; i < data.Length; i += chunk_size)
        {
            // creates the chunk slicing data array
            var chunk = data[i..Math.Clamp(i + chunk_size, 0, data.Length)];

            // encodes current chunk and append the result to the final encoding
            enc.AddRange(RSEncodeMsg(chunk.Select((x) => (int)x).ToArray(), nsym).Select((x) => (byte)x));
        }

        return enc.ToArray();
    }

    public byte[] Encode(string data)
    {
        // converts the input string to a byte array (using UTF-8 encoding)
        return Encode(Encoding.UTF8.GetBytes(data));
    }
}