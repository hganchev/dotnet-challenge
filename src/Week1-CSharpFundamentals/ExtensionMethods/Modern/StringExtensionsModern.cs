#if NET10_0_OR_GREATER
namespace ExtensionMethods.Modern;

/// <summary>
/// C# 14 (.NET 10) extension member block syntax.
/// <para>
/// All extensions for a type are grouped inside an <c>extension(T receiver)</c>
/// block inside a regular <c>static class</c>.  The block can contain:
/// methods, properties, static members, <em>and operators</em> — none of which
/// were possible in C# ≤ 13.
/// </para>
/// </summary>
public static class StringExtensionsModern
{
    // ── Instance extensions ───────────────────────────────────────────────────
    // The receiver is named 's' and is available in every member below.
    extension(string s)
    {
        // ✔ Extension *property* — accessed without parentheses: "hi".IsEmpty
        /// <summary>Returns <c>true</c> when the string has zero characters.</summary>
        public bool IsEmpty => s.Length == 0;

        // ✔ Extension *property* with block body
        /// <summary>Returns <c>true</c> if the string is a palindrome (case-insensitive, alphanumeric only).</summary>
        public bool IsPalindrome
        {
            get
            {
                var cleaned = s.ToLower().Where(char.IsLetterOrDigit).ToArray();
                return cleaned.SequenceEqual(cleaned.Reverse());
            }
        }

        // ✔ Extension *property* using LINQ — no () at the call site
        /// <summary>Number of whitespace-separated words.</summary>
        public int WordCount => s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

        // ✔ Extension method (same as before, but co-located with the properties above)
        /// <summary>Truncates to <paramref name="maxLength"/> chars, appending <paramref name="ellipsis"/> when cut.</summary>
        public string Truncate(int maxLength, string ellipsis = "…")
        {
            if (s.Length <= maxLength) return s;
            return s[..(maxLength - ellipsis.Length)] + ellipsis;
        }

        /// <summary>Converts each word to Title Case.</summary>
        public string ToTitleCase() =>
            string.Join(' ',
                s.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                 .Select(w => char.ToUpper(w[0]) + w[1..].ToLower()));

        // ✔ Extension *operator* — completely new in C# 14, impossible before
        // Repeats the string n times:  "ha" * 3  →  "hahaha"
        /// <summary>Repeats the string <paramref name="count"/> times.</summary>
        public static string operator *(string value, int count) =>
            string.Concat(Enumerable.Repeat(value, count));
    }

    // ── Static extensions ─────────────────────────────────────────────────────
    // No receiver parameter — adds static members directly to the type.
    extension(string)
    {
        // ✔ Extension *static property* — called as string.Separator
        /// <summary>A 40-character separator line.</summary>
        public static string Separator => new string('─', 40);

        // ✔ Extension *static method* — called as string.Repeat(value, n)
        /// <summary>Returns <paramref name="value"/> repeated <paramref name="count"/> times.</summary>
        public static string Repeat(string value, int count) =>
            string.Concat(Enumerable.Repeat(value, count));
    }
}
#endif
