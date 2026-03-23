namespace SpanAndMemory.Examples;

/// <summary>
/// Demonstrates zero-allocation parsing of text using <see cref="ReadOnlySpan{char}"/>.
/// Uses <c>int.TryParse(ReadOnlySpan&lt;char&gt;)</c> and <c>double.TryParse</c>
/// overloads that accept spans — no <c>Substring</c>, no intermediate string allocations.
/// </summary>
public static class ParsingDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Zero-Allocation Parsing with ReadOnlySpan<char> ===");

        // 1. Parse a CSV row of integers
        Console.WriteLine("\n--- CSV integer parsing ---");
        ParseCsvIntegers("10,20,30,40,50");
        ParseCsvIntegers("100,-5,0,999");

        // 2. Parse key=value pairs (e.g., query string)
        Console.WriteLine("\n--- Key=value parsing ---");
        ParseQueryString("name=Alice&age=30&city=Sofia");

        // 3. Parse a simple HTTP-style status line
        Console.WriteLine("\n--- HTTP status line parsing ---");
        ParseStatusLine("HTTP/1.1 200 OK");
        ParseStatusLine("HTTP/2.0 404 Not Found");

        // 4. Parse an IPv4 address
        Console.WriteLine("\n--- IPv4 address parsing ---");
        ParseIpv4("192.168.1.100");
        ParseIpv4("10.0.0.1");

        Console.WriteLine("\n  Key insight: every tokenization step uses ReadOnlySpan<char>");
        Console.WriteLine("  slicing — no string.Split(), no Substring(), zero allocations.");
    }

    private static void ParseCsvIntegers(string csv)
    {
        Console.Write($"  '{csv}' → [");
        ReadOnlySpan<char> remaining = csv.AsSpan();
        bool first = true;
        while (!remaining.IsEmpty)
        {
            int comma = remaining.IndexOf(',');
            ReadOnlySpan<char> token = comma >= 0 ? remaining[..comma] : remaining;

            if (int.TryParse(token, out int value))
            {
                if (!first) Console.Write(", ");
                Console.Write(value);
                first = false;
            }

            remaining = comma >= 0 ? remaining[(comma + 1)..] : ReadOnlySpan<char>.Empty;
        }
        Console.WriteLine("]");
    }

    private static void ParseQueryString(string queryString)
    {
        Console.WriteLine($"  '{queryString}':");
        ReadOnlySpan<char> remaining = queryString.AsSpan();
        while (!remaining.IsEmpty)
        {
            int amp = remaining.IndexOf('&');
            ReadOnlySpan<char> pair = amp >= 0 ? remaining[..amp] : remaining;

            int eq = pair.IndexOf('=');
            if (eq >= 0)
            {
                ReadOnlySpan<char> key = pair[..eq];
                ReadOnlySpan<char> val = pair[(eq + 1)..];
                Console.WriteLine($"    {key} = {val}");
            }

            remaining = amp >= 0 ? remaining[(amp + 1)..] : ReadOnlySpan<char>.Empty;
        }
    }

    private static void ParseStatusLine(string line)
    {
        ReadOnlySpan<char> span = line.AsSpan();

        int sp1 = span.IndexOf(' ');
        if (sp1 < 0) return;
        ReadOnlySpan<char> version = span[..sp1];

        ReadOnlySpan<char> rest = span[(sp1 + 1)..];
        int sp2 = rest.IndexOf(' ');
        if (sp2 < 0) return;
        ReadOnlySpan<char> statusCode = rest[..sp2];
        ReadOnlySpan<char> reason = rest[(sp2 + 1)..];

        int.TryParse(statusCode, out int code);
        Console.WriteLine($"  '{line}' → version={version}, code={code}, reason={reason}");
    }

    private static void ParseIpv4(string address)
    {
        ReadOnlySpan<char> span = address.AsSpan();
        Span<int> octets = stackalloc int[4];
        int octetIndex = 0;

        while (!span.IsEmpty && octetIndex < 4)
        {
            int dot = span.IndexOf('.');
            ReadOnlySpan<char> token = dot >= 0 ? span[..dot] : span;

            if (int.TryParse(token, out int octet))
                octets[octetIndex++] = octet;

            span = dot >= 0 ? span[(dot + 1)..] : ReadOnlySpan<char>.Empty;
        }

        Console.WriteLine($"  '{address}' → [{octets[0]}.{octets[1]}.{octets[2]}.{octets[3]}]  " +
                          "(parsed stackalloc int[4] — zero heap allocation)");
    }
}
