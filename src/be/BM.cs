using System;
using System.Collections.Generic;

public class BM
{
    public static int Search(string text, string pattern)
    {
        int n = text.Length;
        int m = pattern.Length;
        if (m == 0) return -1; // edge case: empty pattern

        Dictionary<char, int> badChar = new Dictionary<char, int>();

        // Building the bad character heuristic
        for (int i = 0; i < m; i++)
            badChar[pattern[i]] = i;

        int s = 0; // s is the shift of the pattern with respect to text
        while (s <= (n - m))
        {
            int j = m - 1;

            while (j >= 0 && pattern[j] == text[s + j])
                j--;

            if (j < 0)
            {
                Console.WriteLine("Pattern occurs at shift = " + s);
                s += (s + m < n) ? m - (badChar.ContainsKey(text[s + m]) ? badChar[text[s + m]] : -1) : 1;
                return s;
            }
            else
            {
                s += Math.Max(1, j - (badChar.ContainsKey(text[s + j]) ? badChar[text[s + j]] : -1));
            }
        }

        return -1; // if the pattern is not found
    }
}