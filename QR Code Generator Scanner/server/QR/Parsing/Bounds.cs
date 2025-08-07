namespace Cod_QR 
{
    struct Bounds
    {
        public int left, right, top, bottom;

        public Bounds(int left, int right, int top, int bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        public override string ToString()
        {
            return $"{left} {top} {right} {bottom}";
        }
    }
}
