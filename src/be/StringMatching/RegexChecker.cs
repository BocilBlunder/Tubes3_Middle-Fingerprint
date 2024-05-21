using System;
using System.Text.RegularExpressions;

public class RegexChecker
{
    private static readonly Dictionary<char, string> numberSubstitutions = new Dictionary<char, string>
    {
        {'a', "(a|4)"},
        {'e', "(e|3)"},
        {'i', "(i|1)"},
        {'o', "(o|0)"},
        {'t', "(t|7)"},
        {'s', "(s|5)"},
        {'g', "(g|6|9)"},
        {'r', "(r|2)"},
        {'b', "(b|8)"},
    };

    public static bool IsValidWord(string original, string alayOption)
    {
        // Normalize to lowercase
        original = original.ToLower();
        alayOption = alayOption.ToLower();

        // Generate regex pattern from original
        string pattern = GenerateRegexPattern(original);

        // Match pattern against alayOption
        return Regex.IsMatch(alayOption, "^" + pattern + "$");
    }

    private static string GenerateRegexPattern(string original)
    {
        // Replace characters based on numberSubstitutions and allow vowel removal
        string regexPattern = "";
        foreach (char c in original)
        {
            if (numberSubstitutions.ContainsKey(c))
            {
                regexPattern += numberSubstitutions[c]; // Substitute with regex pattern
            }
            else
            {
                regexPattern += c; // Add consonants as they are
            }

            if ("aeiou".Contains(c))
            {
                regexPattern += "?"; // Make vowels optional
            }
        }
        return regexPattern;
    }
}
