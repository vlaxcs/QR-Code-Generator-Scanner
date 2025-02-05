public struct QREncodedMessage
{
    public DataType type;
    public byte[] bitsArray;
    public string originalMessage;

    public QREncodedMessage(DataType type, byte[] bitsArray, string originalMessage)
    {
        this.type = type;
        this.bitsArray = bitsArray;
        this.originalMessage = originalMessage;
    }
}
