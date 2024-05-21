public class BooyerMoore
{

    public static int[,] BadCharacterTable(string pattern)
    {
        int[,] table = new int[26, pattern.Length];
        int[] alphabet = new int[26];
        for (int i = 0; i < alphabet.Length; i++) alphabet[i] = -1;

        for (int i = 0; i < pattern.Length; i++)
        {
            int c = pattern[i] - 'a';
            alphabet[c] = i;
            for (int j = 0; j < 26; j++) table[j, i] = alphabet[j];
        }
        return table;
    }


    // Bisa dalam linier waktu, tapi kuadratik dulu aja. Kalau perlu, nanti dibuat yang linier.
    public static int[] GoodFoundSuffixTable(string pattern)
    {
        int[] longestMatchedSuffix = new int[pattern.Length];
        for (int i = 0; i < pattern.Length; i++)
        {
            int j = i;
            while (j >= 0 && pattern[pattern.Length - 1 - (i - j)] == pattern[j]) j--;
            longestMatchedSuffix[i] = i - j;
        }
        int[] suffixTable = new int[pattern.Length];
        for (int i = pattern.Length - 1; i >= 0; i--)
        {
            int j = pattern.Length - 2, suffixLength = pattern.Length - i;
            while (j >= 0 && longestMatchedSuffix[j] < suffixLength) j--;
            if (j < 0) suffixTable[i] = -1;
            else suffixTable[i] = j;
        }
        return suffixTable;
    }

    public static bool Search(string pattern, string text)
    {
        if (text.Length < pattern.Length || text.Length == 0 || pattern.Length == 0)
        {
            return false;
        }

        int[,] badCharacterTable = BadCharacterTable(pattern);
        int[] goodFoundSuffixTable = GoodFoundSuffixTable(pattern);

        int currentPattern = 0;
        while (currentPattern + pattern.Length - 1 < text.Length)
        {
            int currentIndex = currentPattern + pattern.Length - 1;
            while (currentIndex >= currentPattern && pattern[currentIndex - currentPattern] == text[currentIndex]) currentIndex--;
            if (currentIndex < currentPattern) return true;
            int badShift;
            if (badCharacterTable[text[currentIndex] - 'a', currentIndex - currentPattern] == -1) badShift = pattern.Length;
            else badShift = (currentIndex - currentPattern) - badCharacterTable[text[currentIndex] - 'a', currentIndex - currentPattern];

            int goodShift;
            if (currentPattern + pattern.Length - 1 == currentIndex) goodShift = badShift;
            else if (goodFoundSuffixTable[(currentIndex - currentPattern) + 1] == -1) goodShift = 1;
            else goodShift = pattern.Length - goodFoundSuffixTable[(currentIndex - currentPattern) + 1] - 1;

            int maxShift = Math.Max(badShift, goodShift);
            currentPattern += maxShift;
        }
        return false;
    }

}


