using System;
using System.Formats.Asn1;
using System.Xml.Linq;

class QrCodeData
{
    private int[][] qrCodeArray;
    private int[] array;
    private int version;
    public List<int> data;
    public int len;
    public enum datatype
    {   
        none=0,
        numeric=1,
        Alphanumeric=2,
        Byte=4,
        kanji=8,

    }
    public datatype Datatype;
    public int getVersion() { return version;}
    private int sizeForVersion(int version)
    {
        return version * 4 + 17;
    }
    private int[] getAlignmentCoords()
    {
        if (version <= 1){
            return new int[0];
        }
        int num = (version / 7) + 2;
        int[] result = new int[num];
        result[0] = 6;
        if (num == 1)
        {
            for (int i = 0; i < num; i++)
            {
                result[i] = version * 4 + 16 - result[i];
            }
            return result;
        }
        result[num - 1] = 4 * version + 10;
        if (num == 2)
        {
            for (int i = 0; i < num; i++)
            {
                result[i] = version * 4 + 16 - result[i];
            }
            return result;
        }
        result[num - 2] = 2 * ((result[0] + result[num - 1] * (num - 2)) / ((num - 1) * 2)); 
        if (num == 3)
        {
            for (int i = 0; i < num; i++)
            {
                result[i] = version * 4 + 16 - result[i];
            }
            return result;
        }
        int step = result[num - 1] - result[num - 2];
        for (int i = num - 3; i > 0; i--)
        {
            result[i] = result[i + 1] - step;
        }
        for (int i = 0; i < num; i++)
        {
            result[i] = version * 4 + 16 - result[i];
        }
        return result;
    }
    private int[] getAlignmentPoints()
    {
        int[] AlignmentCoords=getAlignmentCoords();
        if (AlignmentCoords.Length == 0)
            return new int[0];
        int[] ans = new int[2*AlignmentCoords.Length * AlignmentCoords.Length - 6];
        int nr = 0;
        for (int i = 0; i < AlignmentCoords.Length; i++)
        {
            for (int j = 0; j < AlignmentCoords.Length; j++)
            {
                if (i == AlignmentCoords.Length - 1 && j == AlignmentCoords.Length - 1)
                    continue;
                if (i == 0 && j == AlignmentCoords.Length - 1)
                    continue;
                if (j == 0 && i == AlignmentCoords.Length - 1)
                    continue;
             
                ans[nr++]=AlignmentCoords[i];
                ans[nr++]=AlignmentCoords[j];
            }
        }

        return ans;
    }
    public bool IsData(int x,int y)
    {
        int max_width= sizeForVersion(version);
        int[] alligmentPoints = getAlignmentPoints();
        if (x < 9 && y < 9) return false;
        if (x < 9 && y > max_width-9) return false;
        if (x > max_width-9 && y < 9) return false;
        if (getVersion() >= 7 && x < 7 && y > max_width - 12) return false;
        if (getVersion() >= 7 && y < 7 && x > max_width - 12) return false;

        if (x==6||y==6) return false;
        for(int i=0;i < alligmentPoints.Length; i += 2)
        {
            if (Math.Abs(alligmentPoints[i] - x) < 3 &&
                Math.Abs(alligmentPoints[i + 1] - y) < 3)
                return false;
        }
        return true;
    }
    public QrCodeData(int[][] mat)
    {
        qrCodeArray = mat;
        if ((mat.GetLength(0) - 17) % 4 != 0)
        {
            Console.Error.WriteLine("Mat Lenght is not Correct");
        }
        version = (mat.GetLength(0) - 17)/4;
        if (version < 1 || version > 40)
        {
            Console.Error.WriteLine("Mat Lenght is not Correct");
        }
        
        for (int i = 0; i < mat.Length; i++)
        {
            for (int j = 0; j < mat.Length ; j++)
            {
                Console.Write(IsData(i,j)?"#":"_");
            }
            Console.WriteLine();
        }
        List<int> x = getData();
        foreach (var indexer in x) {
            Console.Write(indexer);
        }
    }
    public List<int> getData()
    {
        List<int> ans = new List<int>();
        for (int j= qrCodeArray.Length - 1; j >= 0; j -= 2)
        {
            int nj=j-(j>6?1:0);
            if (nj % 4!= 1)
            {
                for (int i = qrCodeArray.Length - 1; i >= 0; i--)
                {

                    if (IsData(i, nj+1))
                        ans.Add(qrCodeArray[i][nj+1]);
                    if (IsData(i, nj ))
                        ans.Add(qrCodeArray[i][nj]);
                }
            }
            else
            {
                for (int i =0; i <qrCodeArray.Length; i++)
                {
                    if (IsData(i, nj+1))
                        ans.Add(qrCodeArray[i][ nj+1]);
                    if (IsData(i, nj ))
                        ans.Add(qrCodeArray[i][ nj ]);
                }

            }
        }
        Datatype = ((datatype)((ans[0]) << 3 | ans[1] << 2 | ans[2] << 1 | ans[3]));
        len = ans[4] << 7 + ans[5] << 6 + ans[6] << 5 + ans[7] << 4 + ans[8] << 3 + ans[9] << 2 + ans[10] << 1 + ans[11];
        return ans;
    }
}
