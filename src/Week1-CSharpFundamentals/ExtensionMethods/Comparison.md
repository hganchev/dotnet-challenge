# Extension Methods: C# ≤ 13 (.NET 9) vs C# 14 (.NET 10)

## What changed in C# 14

C# 14 introduces **extension member blocks** — a new syntax that lets you add
properties, static members, and operators to an existing type, not just methods.

```csharp
// C# 14 — everything for the type lives in one block
public static class StringExtensionsModern
{
    extension(string s)                     // receiver named 's'
    {
        public bool   IsEmpty    => s.Length == 0;          // ← property
        public int    WordCount  => s.Split(' ').Length;    // ← property
        public bool   IsPalindrome { get { … } }            // ← property (body)
        public string Truncate(int n) { … }                 // ← method (same as before)
        public static string operator *(string v, int n)    // ← operator
            => string.Concat(Enumerable.Repeat(v, n));
    }

    extension(string)                       // no receiver = static context
    {
        public static string Separator => new string('─', 40);   // ← static property
        public static string Repeat(string v, int n) => …;       // ← static method
    }
}
```

---

## Side-by-side comparison

| Capability | C# ≤ 13 / .NET 9 | C# 14 / .NET 10 |
|---|---|---|
| **Extension methods** | ✔ `s.WordCount()` | ✔ `s.WordCount()` |
| **Extension properties** | ✘ must use `()` | ✔ `s.WordCount` — no `()` |
| **Extension static members** | ✘ not possible | ✔ `string.Separator` |
| **Extension operators** | ✘ not possible | ✔ `"ha" * 3` |
| **Grouping** | ✘ one `static class` per type | ✔ `extension(T)` block groups all members |
| **Syntax** | `public static R Foo(this T x)` | `extension(T x) { public R Foo() }` |

---

## Code examples

### Extension property

```csharp
// .NET 9 — must wrap in a method
public static int WordCount(this string s)          // call site: s.WordCount()
    => s.Split(' ').Length;

// .NET 10 — true property
extension(string s)
{
    public int WordCount => s.Split(' ').Length;     // call site: s.WordCount
}
```

### Extension static member

```csharp
// .NET 9 — impossible; you can only target instances
//          the closest workaround is a plain static helper class

// .NET 10
extension(string)
{
    public static string Separator => new string('─', 40);  // call site: string.Separator
}
```

### Extension operator

```csharp
// .NET 9 — impossible

// .NET 10
extension(string s)
{
    public static string operator *(string value, int count)   // call site: "ha" * 3
        => string.Concat(Enumerable.Repeat(value, count));
}
```

---

## Backwards compatibility

The classic `this`-parameter syntax still compiles unchanged on .NET 10 / C# 14.
Nothing is removed — you only get new capabilities.
