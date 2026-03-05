# How to run ExtensionMethods

The project multi-targets **net9.0** and **net10.0**.  
Use `-f` / `--framework` to choose which runtime to use.

## From the repository root

```bash
# .NET 9 — classic extension methods only
dotnet run -f net9.0 --project src/Week1-CSharpFundamentals/ExtensionMethods

# .NET 10 — classic section + new C# 14 extension member blocks
dotnet run -f net10.0 --project src/Week1-CSharpFundamentals/ExtensionMethods
```

## From inside the project folder

```bash
cd src/Week1-CSharpFundamentals/ExtensionMethods

dotnet run -f net9.0
dotnet run -f net10.0
```

## Build both targets at once (no run)

```bash
dotnet build src/Week1-CSharpFundamentals/ExtensionMethods
```

## Expected output

### net9.0

```
=== Extension Methods: .NET 9 (C# 13) vs .NET 10 (C# 14) ===
  Runtime: .NET 9  — only classic section applies
  Tip: run with -f net10.0 to see the .NET 10 section too

┌─────────────────────────────────────────────────────────┐
│  .NET 9 / C# ≤ 13 — Classic extension methods           │
└─────────────────────────────────────────────────────────┘
  "racecar".IsPalindrome()  → True
  "hello".IsPalindrome()    → False
  "the quick brown fox".WordCount()   → 4   ← must use ()
  "Hello, World!".Truncate(8)    → Hello, …
  "the quick brown fox".ToTitleCase() → The Quick Brown Fox

  Limitations in C# ≤ 13:
    ✘ No extension properties   — s.WordCount()  not  s.WordCount
    ✘ No extension static members — can't add  string.Separator
    ✘ No extension operators    — can't write  "ha" * 3
    ✘ Extensions for one type scattered across unrelated static classes
```

### net10.0

```
=== Extension Methods: .NET 9 (C# 13) vs .NET 10 (C# 14) ===
  Runtime: .NET 10 — both sections shown

┌─────────────────────────────────────────────────────────┐
│  .NET 9 / C# ≤ 13 — Classic extension methods           │
└─────────────────────────────────────────────────────────┘
  ... (same as above) ...

┌─────────────────────────────────────────────────────────┐
│  .NET 10 / C# 14 — New extension member blocks          │
└─────────────────────────────────────────────────────────┘
  "".IsEmpty              → True   ← property, no ()
  "hi".IsEmpty            → False
  "racecar".IsPalindrome  → True   ← property, no ()
  "hello".IsPalindrome    → False
  "the quick brown fox".WordCount   → 4       ← property, no ()
  "Hello, World!".Truncate(8)   → Hello, …
  "the quick brown fox".ToTitleCase() → The Quick Brown Fox

  string.Separator        → ────────────────────────────────────────
  string.Repeat("ab", 4)  → abababab

  "ha" * 3               → hahaha
  "-" * 20               → --------------------

  New capabilities in C# 14:
    ✔ Extension properties    — .WordCount, .IsEmpty, .IsPalindrome
    ✔ Extension static members — string.Separator, string.Repeat(...)
    ✔ Extension operators      — "ha" * 3
    ✔ All extensions for a type grouped in one extension(...) block
```

## Project file reference

```xml
<TargetFrameworks>net9.0;net10.0</TargetFrameworks>
```

The C# 14-only code is guarded with `#if NET10_0_OR_GREATER` so the same
source tree compiles cleanly on both runtimes.
