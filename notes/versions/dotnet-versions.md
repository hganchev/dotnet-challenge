# .NET / C# Version Comparison

| .NET Version | C# Version | Release | Support | Key Features |
|---|---|---|---|---|
| .NET Core 1.0 | C# 6.0 | Jun 2016 | EOL | String interpolation, null-conditional operators, `nameof`, expression-bodied members, auto-property initializers |
| .NET Core 2.0 | C# 7.1 | Aug 2017 | EOL | `ValueTask<T>`, Span precursor APIs, default literal, async Main |
| .NET Core 2.1 | C# 7.2 | May 2018 | EOL | `Span<T>` / `Memory<T>`, `in` parameters, `ref struct`, `private protected` |
| .NET Core 3.0 | C# 8.0 | Sep 2019 | EOL | Nullable reference types, async streams (`IAsyncEnumerable`), default interface members, switch expressions, ranges & indices |
| .NET Core 3.1 | C# 8.0 | Dec 2019 | EOL (LTS) | Same as 3.0, Blazor Server GA, gRPC |
| .NET 5 | C# 9.0 | Nov 2020 | EOL | Records, top-level statements, `init` setters, pattern matching improvements, `nint`/`nuint`, covariant returns |
| .NET 6 | C# 10.0 | Nov 2021 | EOL (LTS) | Global usings, file-scoped namespaces, `record struct`, `CallerArgumentExpression`, Minimal APIs, `DateOnly`/`TimeOnly` |
| .NET 7 | C# 11.0 | Nov 2022 | EOL | Required members, generic attributes, raw string literals, `list patterns`, `static abstract` interface members, UTF-8 string literals |
| .NET 8 | C# 12.0 | Nov 2023 | **LTS** (EOL Nov 2026) | Primary constructors, collection expressions, `ref readonly` params, `[AllowedValues]`, Frozen collections, AOT improvements, Keyed DI |
| .NET 9 | C# 13.0 | Nov 2024 | STS (EOL May 2026) | `params` collections, `field` keyword (preview), `lock` object, `allows ref struct` constraint, `SearchValues<string>`, `HybridCache`, `Task.WhenEach` |
| .NET 10 | C# 14.0 | Nov 2025 | **LTS** | Built-in endpoint validation (`AddValidation()`), `field` keyword stable, `params` in lambdas, extensions (preview), `OverloadResolutionPriority`, `BinaryFormatter` removed |

---

## Support lifecycle quick reference

| Type | Duration | Example |
|---|---|---|
| **LTS** (Long-Term Support) | 3 years patches | .NET 8, .NET 10 |
| **STS** (Standard-Term Support) | 18 months patches | .NET 9 |
| EOL | No more patches — upgrade required | .NET 7 and earlier |

> **Rule of thumb**: even-numbered releases are LTS (.NET 6, 8, 10). Odd are STS (.NET 7, 9).
