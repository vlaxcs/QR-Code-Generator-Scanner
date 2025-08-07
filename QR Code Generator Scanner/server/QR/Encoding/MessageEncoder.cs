using QREncoder;

public static class MessageEncoder
{
    readonly static Dictionary<DataType, IEncoder> encoders = new() {
        { DataType.Numeric, new NumericEncoder() },
        { DataType.Alphanumeric, new AlphanumericEncoder() },
        { DataType.Byte, new ByteEncoder() }
    };

    public static QREncodedMessage EncodeMessage(DataType type, string message)
    {
        if (type == DataType.Kanji)
        {
            throw new Exception("Kanji not supported");
        }

        message = message.Trim();

        var bits = encoders[type].Encode(message);

        return new QREncodedMessage(type, bits, message);
    }
}
