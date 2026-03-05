namespace ExtensionMethods.Classic;

/// <summary>
/// Classic C# extension method style (C# ≤ 13 / .NET ≤ 9).
/// All extensions live in a <c>static class</c>; the receiver is the first
/// parameter prefixed with <c>this</c>.  Works on every supported TFM.
/// </summary>
/// <remarks>
/// LIMITATIONS of this style:
///   ✘ No extension *properties*   — must use method syntax:  s.WordCount()
///   ✘ No extension *static* members — cannot add e.g. string.Separator
///   ✘ No extension *operators*    — cannot overload * / + on string
///   ✘ Related helpers are scattered; callers see no grouping
/// </remarks>
public static class StringExtensionsClassic
{
    /// <summary>Returns <c>true</c> if <paramref name="s"/> reads the same forwards and backwards (ignoring case and non-alphanumeric characters).</summary>
    public static bool IsPalindrome(this string s)
    {
        var cleaned = s.ToLower().Where(char.IsLetterOrDigit).ToArray();
        return cleaned.SequenceEqual(cleaned.Reverse());
    }

    /// <summary>Counts words split by spaces — must be called as a method: <c>s.WordCount()</c>.</summary>
    public static int WordCount(this string s) =>
        s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

    /// <summary>Truncates <paramref name="s"/> to <paramref name="maxLength"/> characters, appending <paramref name="ellipsis"/> if cut.</summary>
    public static string Truncate(this string s, int maxLength, string ellipsis = "…")
    {
        if (s.Length <= maxLength) return s;
        return s[..(maxLength - ellipsis.Length)] + ellipsis;
    }

    /// <summary>Converts each word to Title Case.</summary>
    public static string ToTitleCase(this string s) =>
        string.Join(' ',
            s.Split(' ', StringSplitOptions.RemoveEmptyEntries)
             .Select(w => char.ToUpper(w[0]) + w[1..].ToLower()));
}
