using System.Text;

public struct QRMessage
{
    public DataType type;
    public int[] blocks;

    public QRMessage(DataType type, int[] blocks)
    {
        this.type = type;
        this.blocks = blocks;
    }

    public override string ToString()
    {

        switch (type)
        {
            case DataType.Numeric:
                return NumericToString();

            case DataType.Alphanumeric:
                return AlphanumericToString();

            case DataType.Byte:
                return ByteToString();

            default:
                return "Sorry not implemented yet";
        }
    }

    string NumericToString()
    {
        var sb = new StringBuilder();

        for (int i = 0; i < blocks.Length; i++)
        {
            sb.Append(blocks[i]);
        }

        return sb.ToString();
    }

    static readonly char[] alphanumericConversion = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' ', '$', '%', '*', '+', '-', '.', '/', ':' };

    string AlphanumericToString()
    {
        var sb = new StringBuilder();

        for (int i = 0; i < blocks.Length; i++)
        {
            sb.Append(alphanumericConversion[blocks[i]]);
        }

        return sb.ToString();
    }

    string ByteToString()
    {
        var sb = new StringBuilder();

        for (int i = 0; i < blocks.Length; i++)
        {
            sb.Append((char)blocks[i]);
        }

        return sb.ToString();
    }
}
