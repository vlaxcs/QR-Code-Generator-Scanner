public struct ECCGrouping
{
    public int ECCodewordsPerBlock;
    public int G1Count, codewordsInG1;
    public int G2Count, codewordsInG2;

    public int TotalDataBlocks => codewordsInG1 * G1Count + codewordsInG2 * G2Count;
    public int TotalECCBlocks => ECCodewordsPerBlock * (G1Count + G2Count);
    public int TotalBlocks => TotalDataBlocks + TotalECCBlocks;

    public ECCGrouping(int ECCodewordsPerBlock, int G1Count, int codewordsInG1, int G2Count, int codewordsInG2)
    {
        this.ECCodewordsPerBlock = ECCodewordsPerBlock;
        this.G1Count = G1Count;
        this.codewordsInG1 = codewordsInG1;
        this.G2Count = G2Count;
        this.codewordsInG2 = codewordsInG2;
    }

    public override string ToString()
    {
        return $"{ECCodewordsPerBlock} {G1Count} {codewordsInG1} {G2Count} {codewordsInG2}";
    }
}