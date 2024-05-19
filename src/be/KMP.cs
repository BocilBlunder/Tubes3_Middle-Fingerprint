using System;

public class KMP
{
    public static int Search(string text, string pattern)
    {
        int n = text.Length;
        int m = pattern.Length;
        int[] lps = new int[m];
        int j = 0; // Length of previous longest prefix suffix

        // Preprocess the pattern to fill lps array
        ComputeLPSArray(pattern, m, lps);

        int i = 0; // index for text
        while (i < n)
        {
            if (pattern[j] == text[i])
            {
                j++;
                i++;
            }
            if (j == m)
            {
                Console.WriteLine("Found pattern at index " + (i - j));
                j = lps[j - 1];
                return i-j;
            }
            else if (i < n && pattern[j] != text[i])
            {
                if (j != 0)
                    j = lps[j - 1];
                else
                    i = i + 1;
            }
        }

        return -1; // if the pattern is not found
    }

    private static void ComputeLPSArray(string pattern, int m, int[] lps)
    {
        int len = 0;
        int i = 1;
        lps[0] = 0; // lps[0] is always 0

        while (i < m)
        {
            if (pattern[i] == pattern[len])
            {
                len++;
                lps[i] = len;
                i++;
            }
            else
            {
                if (len != 0)
                {
                    len = lps[len - 1];
                }
                else
                {
                    lps[i] = 0;
                    i++;
                }
            }
        }
    }
}
