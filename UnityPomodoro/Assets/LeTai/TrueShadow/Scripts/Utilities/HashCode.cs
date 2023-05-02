namespace LeTai
{
// Extended from https://referencesource.microsoft.com/#mscorlib/system/tuple.cs,52
public static class HashUtils
{
    public static int CombineHashCodes(int h1, int h2)
    {
        return ((h1 << 5) + h1) ^ h2;
    }

    public static int CombineHashCodes(int h1, int h2, int h3)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2), h3);
    }

    public static int CombineHashCodes(int h1, int h2, int h3, int h4)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2), CombineHashCodes(h3, h4));
    }

    public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), h5);
    }

    public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6));
    }

    public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6, h7));
    }

    public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6, h7, h8));
    }

    public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8, int h9)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7, h8), h9);
    }

    public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8, int h9,
                                       int h10)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7, h8), CombineHashCodes(h9, h10));
    }

    public static int CombineHashCodes(int h1,  int h2, int h3, int h4, int h5, int h6, int h7, int h8, int h9,
                                       int h10, int h11)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7, h8), CombineHashCodes(h9, h10, h11));
    }

    public static int CombineHashCodes(int h1,  int h2,  int h3, int h4, int h5, int h6, int h7, int h8, int h9,
                                       int h10, int h11, int h12)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7, h8), CombineHashCodes(h9, h10, h11, h12));
    }

    public static int CombineHashCodes(int h1,  int h2,  int h3,  int h4, int h5, int h6, int h7, int h8, int h9,
                                       int h10, int h11, int h12, int h13)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2,  h3,  h4,  h5, h6, h7, h8),
                                CombineHashCodes(h9, h10, h11, h12, h13));
    }

    public static int CombineHashCodes(int h1,  int h2,  int h3,  int h4,  int h5, int h6, int h7, int h8, int h9,
                                       int h10, int h11, int h12, int h13, int h14)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2,  h3,  h4,  h5,  h6, h7, h8),
                                CombineHashCodes(h9, h10, h11, h12, h13, h14));
    }

    public static int CombineHashCodes(int h1,  int h2,  int h3,  int h4,  int h5,  int h6, int h7, int h8, int h9,
                                       int h10, int h11, int h12, int h13, int h14, int h15)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2,  h3,  h4,  h5,  h6,  h7, h8),
                                CombineHashCodes(h9, h10, h11, h12, h13, h14, h15));
    }
}
}
