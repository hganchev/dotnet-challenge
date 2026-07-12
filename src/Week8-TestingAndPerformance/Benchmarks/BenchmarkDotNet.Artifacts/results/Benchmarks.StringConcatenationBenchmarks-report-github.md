```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core Ultra 9 185H 0.40GHz, 1 CPU, 22 logical and 16 physical cores
.NET SDK 10.0.109
  [Host] : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Dry    : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method              | Iterations | Mean       | Error | Ratio | Gen0      | Allocated   | Alloc Ratio |
|-------------------- |----------- |-----------:|------:|------:|----------:|------------:|------------:|
| **StringConcatenation** | **100**        |   **253.5 μs** |    **NA** |  **1.00** |         **-** |     **12576 B** |        **1.00** |
| StringBuilderAppend | 100        |   337.9 μs |    NA |  1.33 |         - |       768 B |        0.06 |
|                     |            |            |       |       |           |             |             |
| **StringConcatenation** | **1000**       |   **604.1 μs** |    **NA** |  **1.00** |         **-** |   **1025976 B** |       **1.000** |
| StringBuilderAppend | 1000       |   323.0 μs |    NA |  0.53 |         - |      4576 B |       0.004 |
|                     |            |            |       |       |           |             |             |
| **StringConcatenation** | **10000**      | **8,721.3 μs** |    **NA** |  **1.00** | **7000.0000** | **100259976 B** |       **1.000** |
| StringBuilderAppend | 10000      |   692.1 μs |    NA |  0.08 |         - |     53200 B |       0.001 |
