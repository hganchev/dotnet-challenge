namespace OOP.StaticMembers;

// Extension methods must live in a static class.
// The first parameter (prefixed with 'this') is the type being extended.
public static class StringExtensions
{
    // Palindrome check: ignores case and non-alphanumeric characters
    public static bool IsPalindrome(this string s)
    {
        var cleaned = s.ToLower().Where(char.IsLetterOrDigit).ToArray();
        return cleaned.SequenceEqual(cleaned.Reverse());
    }

    public static string Truncate(this string s, int maxLength, string ellipsis = "…")
    {
        if (s.Length <= maxLength) return s;
        return s[..(maxLength - ellipsis.Length)] + ellipsis;
    }

    public static string ToTitleCase(this string s) =>
        string.Join(' ',
            s.Split(' ', StringSplitOptions.RemoveEmptyEntries)
             .Select(w => char.ToUpper(w[0]) + w[1..].ToLower()));
}
