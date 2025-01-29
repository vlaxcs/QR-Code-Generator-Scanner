public class GaloisField {
    public int[] gf_exp, gf_log;

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
        if(errPos.Length == 0) throw new Exception("Could not locate error");

        var res = new int[erase_pos.Count() + errPos.Count()];
        erase_pos.CopyTo(res, 0);
        errPos.CopyTo(res, erase_pos.Count());
        rs_correct_errata(msg_out, synd, res);

        synd = rs_calc_syndromes(msg_out, nsym);
        if(synd.Max() > 0) throw new Exception("Could not correct message");

        return msg_out.Take(msg_out.Length - nsym).ToArray();
    }

    private void rs_correct_errata(int[] msg, int[] synd, int[] pos) {
        List<int> q = new List<int>() { 1 };
        for(int i = 0; i < pos.Length; i++) {
            int x = gf_exp[msg.Length - 1 - pos[i]];
            q = gf_poly_mul(q, new int[] { x, 1 });
        }
        var p = synd.Take(pos.Length).ToArray(); 
    }

    private List<int> gf_poly_mul(List<int> q, int[] ints) {
        throw new NotImplementedException();
    }

    private int[] rs_find_errors(int[] fsynd, int length) {
        throw new NotImplementedException();
    }

    private int[] rs_forney_syndromes(int[] synd, List<int> erase_pos, int length) {
        throw new NotImplementedException();
    }

    private int[] rs_calc_syndromes(int[] msg_out, int nsym) {
        throw new NotImplementedException();
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