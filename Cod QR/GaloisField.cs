using System.Text;

public class GaloisField {
    public int[] gf_exp, gf_log;
    public int nsym;

    public GaloisField(int nsym = 10) {
        this.nsym = nsym;
    }

    public int[] rs_correct_msg(int[] msg_in, int nsym) {
        if(msg_in.Length > 255) throw new Exception("Message too long");
        int[] msg_out = (int[])msg_in.Clone();

        // Find erasures
        List<int> erase_pos = new List<int>();
        for(int i = 0; i < msg_out.Length; i++) {
            if(msg_out[i] >= 0) continue;
            msg_out[i] = 0;
            erase_pos.Add(i);
        }
        if(erase_pos.Count > nsym) throw new Exception("Too many erasures to correct");

        int[] synd = rs_calc_syndromes(msg_out, nsym);
        if(synd.Max() == 0) {
            return msg_out.Take(msg_out.Length - nsym).ToArray();
        }

        int[] fsynd = rs_forney_syndromes(synd, erase_pos, msg_out.Length);
        int[] errPos = rs_find_errors(fsynd, msg_out.Length);
        if(errPos == null) throw new Exception("Could not locate error");

        var res = new int[erase_pos.Count + errPos.Length];
        erase_pos.CopyTo(res, 0);
        errPos.CopyTo(res, erase_pos.Count);
        rs_correct_errata(msg_out, synd, res);

        synd = rs_calc_syndromes(msg_out, nsym);
        if(synd.Max() > 0) throw new Exception("Could not correct message");

        return msg_out.Take(msg_out.Length - nsym).ToArray();
    }

    private void rs_correct_errata(int[] msg, int[] synd, int[] pos) {
        int[] q = { 1 };
        for(int i = 0; i < pos.Length; i++) {
            int x = gf_exp[msg.Length - 1 - pos[i]];
            q = gf_poly_mul(q, new int[] { x, 1 });
        }
        var p = synd.Take(pos.Length).ToArray();
        p = p[^pos.Length..];

        // formal derivative of error locator eliminates even terms
        int start = q.Length % 2;
        int[] aux = new int[(q.Length - start + 1) / 2];
        for(int i = 0, j = start; j < q.Length; j+=2, i++) {
            aux[i] = q[j];
        }
        q = aux;
        for(int i = 0; i < pos.Length; i++) {
            int x = gf_exp[pos[i] + 256 - msg.Length];
            int y = gf_poly_eval(p, x);
            int z = gf_poly_eval(q, gf_mul(x, x));
            msg[pos[i]] ^= gf_div(y, gf_mul(x, z));
        }
    }

    private List<int> rs_find_errors(int[] synd, int nmess) {
        int[] err_poly = { 1 };
        List<int> old_poly = new List<int>() { 1 };
        for(int i = 0; i < synd.Length; i++) {
            old_poly.Add(0);
            int delta = synd[i];
            for(int j = 1; j < err_poly.Length; j++) {
                delta ^= gf_mul(err_poly[err_poly.Length - 1 - j], synd[i - j]);
            }
            if(delta != 0) {
                if(old_poly.Count > err_poly.Length) {
                    int[] new_poly = gf_poly_scale(old_poly, delta);
                    old_poly = gf_poly_scale(err_poly, gf_div(1, delta));
                    err_poly = new_poly;
                }
                err_poly = gf_poly_add(err_poly, gf_poly_scale(old_poly, delta));
            }
        }
        int errs = err_poly.Length - 1;
        if(errs * 2 > synd.Length) throw new Exception("Too many errors to connect");
        List<int> err_pos = new List<int>();
        for(int i = 0; i < nmess; i++) {
            if(gf_poly_eval(err_poly, gf_exp[255-i]) == 0) {
                err_pos.Add(nmess - i - 1);
            }
        }
        if(err_pos.Count != errs) return null;
        return err_pos;
    }
    private int gf_poly_eval(int[] p, int x) {
        int y = p[0];
        for(int i = 1; i < p.Length; i++) {
            y = gf_mul(y, x) ^ p[i];
        }
        
        return y;
    }

    public List<byte> encode(byte[] data) {
        int chunk_size = 255 - nsym;
        var enc = new List<byte>();
        for(int i = 0; i < data.Length; i += chunk_size) {
            var chunk = data[i..(i + chunk_size)];
            enc.AddRange(rs_encode_msg(chunk, nsym));
        }
        return enc;
    }
}

/*
def rs_correct_msg(msg_in, nsym):
    if len(msg_in) > 255:
        raise ValueError("message too long")
    msg_out = list(msg_in)     # copy of message
    # find erasures
    erase_pos = []
    for i in range(0, len(msg_out)):
        if msg_out[i] < 0:
            msg_out[i] = 0
            erase_pos.append(i)
    if len(erase_pos) > nsym:
        raise ReedSolomonError("Too many erasures to correct")
    synd = rs_calc_syndromes(msg_out, nsym)
    if max(synd) == 0:
        return msg_out[:-nsym]  # no errors
    fsynd = rs_forney_syndromes(synd, erase_pos, len(msg_out))
    err_pos = rs_find_errors(fsynd, len(msg_out))
    if err_pos is None:
        raise ReedSolomonError("Could not locate error")
    rs_correct_errata(msg_out, synd, erase_pos + err_pos)
    synd = rs_calc_syndromes(msg_out, nsym)
    if max(synd) > 0:
        raise ReedSolomonError("Could not correct message")
    return msg_out[:-nsym]
*/