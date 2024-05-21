using System;
public class StringDistance
{
    public static int HammingDistance(string a, string b)
    {
        if (a.Length != b.Length)
        {
            Console.WriteLine("Strings must be of equal length in calculating the hamming distance");
            Environment.Exit(0);
        }
        int count = 0;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i])
            {
                count++;
            }
        }
        return count;
    }

    public static int LongestCommonSubsequence(string a, string b)
    {
        int[,] dp = new int[a.Length + 1, b.Length + 1];
        for (int i = 1; i <= a.Length; ++i)
        {
            for (int j = 1; j <= b.Length; ++j)
            {
                if (a[i - 1] == b[j - 1])
                {
                    dp[i, j] = 1 + dp[i - 1, j - 1];
                }
                else
                {
                    dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }
        }
        return dp[a.Length, b.Length];
    }

    public static int LevenshteinDistance(string a, string b)
    {
        int[,] dp = new int[a.Length + 1, b.Length + 1];

        for (int i = 1; i <= a.Length; i++)
        {
            dp[i, 0] = i;
        }

        for (int j = 1; j <= b.Length; j++)
        {
            dp[0, j] = j;
        }

        for (int i = 1; i <= a.Length; i++)
        {
            for (int j = 1; j <= b.Length; j++)
            {
                int cost;
                if (a[i - 1] == b[j - 1]) cost = 0;
                else cost = 1;

                dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), dp[i - 1, j - 1] + cost);
            }
        }
        return dp[a.Length, b.Length];
    }

    public static double CalculateSimilarityPercentage(string a, string b)
    {
        int distance = LevenshteinDistance(a, b);
        int maxLen = Math.Max(a.Length, b.Length);
        return maxLen == 0 ? 100.0 : (1.0 - (double)distance / maxLen) * 100.0;
    }
}


