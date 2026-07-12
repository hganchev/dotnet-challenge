# Week 4 - LINQ and Collections

## Goal
Master LINQ query patterns and the .NET Collections Framework: understand deferred execution and expression trees, select the right collection for each scenario, and write high-performance queries that scale to large datasets without unnecessary allocations.

---

## 1. LINQ Internals

### Topics to Cover
| Topic | Key Points |
|---|---|
| `IEnumerable<T>` | Deferred (lazy) execution; `GetEnumerator()` and `MoveNext()` loop |
| `IEnumerable` vs `IQueryable<T>` | `IEnumerable` = in-memory delegates; `IQueryable` = expression trees translated to SQL |
| Deferred execution | Query operators do not run immediately; they run on iteration (`foreach`, `ToList`, etc.) |
| Expression trees | Abstract syntax trees; captured at compile time; translatable to SQL (EF Core), dynamically compilable |
| `Func<T, R>` vs `Expression<Func<T, R>>` | Delegate (execute immediately) vs expression (tree, can be translated) |
| Query vs method syntax | Both compile to the same delegates; choose for readability |
| Streaming operators | `Select`, `Where` — push elements through one at a time |
| Materialising operators | `ToList()`, `ToArray()`, `ToDictionary()` — force eager evaluation |
| Aggregators | `First()`, `Single()`, `Count()`, `Sum()` — materialise and reduce |

### Expected Outcomes
- Explain why `query.Where(...).Select(...).Where(...).Count()` only iterates the collection once
- Distinguish between a query that hasn't run yet and one that has been materialised
- Write an `IEnumerable<T>` extension method using `yield return` and demonstrate lazy evaluation
- Compare `IEnumerable<T>` filtering with `IQueryable<T>` translation to SQL

---

## 2. Collections

### Topics to Cover
| Collection | Time Complexity | Space | Use Case |
|---|---|---|---|
| `List<T>` | O(1) get/set, O(n) insert/remove | Contiguous | Ordered, frequent random access, occasional mutations |
| `Dictionary<K,V>` | O(1) avg lookup | Hash table | Fast key lookups; unordered |
| `SortedDictionary<K,V>` | O(log n) lookup | Balanced tree | Sorted by key; iterator traversal |
| `HashSet<T>` | O(1) avg add/remove | Hash table | Membership tests, set operations (union, intersect) |
| `SortedSet<T>` | O(log n) add/remove | Balanced tree | Sorted membership; range queries |
| `LinkedList<T>` | O(1) insert/remove (if you have node), O(n) search | Doubly-linked | Frequent mid-list insertion/removal; rare without node reference |
| `Stack<T>` | O(1) push/pop | Array-backed | LIFO; parse expressions, DFS |
| `Queue<T>` | O(1) enqueue/dequeue | Circular buffer | FIFO; BFS, task scheduling |
| `ArrayPool<T>` | O(1) rent/return | Pooled heap | Large temporary buffers; reusable allocation |
| `ImmutableArray<T>` | O(1) get, O(n) update | Immutable tree | Thread-safe sharing; structural sharing |

### Expected Outcomes
- Choose the best collection for a specific scenario (e.g., membership test, sorted iteration, LIFO)
- Explain why `List<T>.Contains()` is O(n) and use `HashSet<T>` instead for repeated lookups
- Initialise collections with capacity hints to avoid repeated internal resizing
- Use `ImmutableArray<T>` / `ImmutableDictionary<K,V>` for safe concurrent access without locks
- Understand when `LinkedList<T>` is (rarely) faster than `List<T>` for mid-list mutations

---

## 3. LINQ Performance

### Topics to Cover
| Technique | Benefit | Caveat |
|---|---|---|
| `Any()` vs `Count() > 0` | `Any()` short-circuits after 1st match | Use `Any()` always for boolean checks |
| `FirstOrDefault()` vs `SingleOrDefault()` | `First` stops at 1st; `Single` scans all | Don't use `Single` unless uniqueness is guaranteed |
| `Where` then `Select` | Filters before projection | Reduces allocations for discarded items |
| Streaming vs materialising | One-pass with `foreach`; multiple with `ToList()` | Cache if iterating 2+ times |
| `Count` property vs `.Count()` | Direct property is O(1); LINQ method enumerates | Use `.Count` on `ICollection<T>` directly |
| Lazy composition | Chain filters/projections without `ToList()` | Preserve streaming until final materialisation |
| Avoid `.OrderBy` in loops | Sort once, cache result | Sorting inside a loop multiplies work |
| `CollectionsMarshal.AsSpan` | Zero-copy list iteration as `Span<T>` | Fastest loop over `List<T>` internals |
| PLINQ `.AsParallel()` | CPU-bound queries across multiple cores | Overhead for small datasets; profile first |

### Expected Outcomes
- Identify and fix multiple enumeration bugs in LINQ chains
- Choose between streaming and materialisation based on usage patterns
- Rewrite a hot-path loop using `Span<T>` / `CollectionsMarshal` for performance
- Explain why `Enumerable.Range(0, 1_000_000).Where(...).Count()` is efficient (streaming)
- Profile and optimise a LINQ query by eliminating redundant materialisation or sorting

---

## Implementation Plan

### Implementation Order
| Step | Project | Why |
|---|---|---|
| 1 | LinqInternals | Foundation — understand deferred execution and expression trees |
| 2 | Collections | Choose the right collection for each scenario |
| 3 | LinqPerformance | Apply performance best practices to real queries |

### 1. LinqInternals

**Folder structure:**
```
LinqInternals/
  Program.cs                             # Orchestrator — calls all demos
  Concepts/
    DeferredExecutionDemo.cs             # Query doesn't run until iteration
    ExpressionTreeDemo.cs                # Expression<Func<T,R>> vs Func<T,R>
    IQueryableDemo.cs                    # Translation to SQL (EF Core mock)
    YieldReturnDemo.cs                   # Custom IEnumerable with yield
  Examples/
    QueryVsMethodSyntaxDemo.cs           # Both compile to same delegates
    StreamingOperatorsDemo.cs            # Select, Where, TakeWhile — one-pass
    MaterialisingOperatorsDemo.cs        # ToList, ToArray, ToDictionary
    AggregatorsDemo.cs                   # First, Single, Count, Sum — materialise
```

**Key demos:**
- Deferred execution: build a query, show it hasn't run, then iterate and show it runs
- Expression trees: compile and evaluate `Expression<Func<int, bool>>` vs plain `Func`
- `IQueryable` mock: show expression tree translation to SQL-like string
- `yield return`: implement custom `Fibonacci()` generator, show memory efficiency
- Streaming vs materialising: `Count()` after `Where().Select()` vs `ToList().Count()`

### 2. Collections

**Folder structure:**
```
Collections/
  Program.cs                             # Orchestrator — calls all demos
  Concepts/
    CollectionComparison.cs              # O(n) vs O(1) table; choose by scenario
    ListVsDictionary.cs                  # Random access vs key lookup
    HashSetVsList.cs                     # O(1) membership vs O(n)
    StackQueueLinkedList.cs              # LIFO vs FIFO vs mid-list insert
  Examples/
    ListDemo.cs                          # Ordered, resizable, index access
    DictionaryDemo.cs                    # Fast key lookups
    HashSetDemo.cs                       # Membership, set operations
    StackQueueDemo.cs                    # LIFO/FIFO semantics
    SortedCollectionsDemo.cs             # SortedDictionary, SortedSet
    LinkedListDemo.cs                    # Mid-list mutations (rare use case)
    ImmutableCollectionsDemo.cs          # ImmutableArray, ImmutableDictionary
    ArrayPoolDemo.cs                     # Reusable large buffers
    CapacityHintDemo.cs                  # Pre-sizing to avoid resizing
```

**Key demos:**
- List: ordered append, index access, resizing overhead
- Dictionary: O(1) lookups, hash collisions, enumeration order is undefined
- HashSet: membership test, union/intersect, faster than List.Contains
- Stack/Queue: LIFO/FIFO, practical parsing/scheduling
- SortedDictionary/SortedSet: ordered traversal, higher lookup cost
- LinkedList: mid-list node operations (but rarely faster without node reference)
- Immutability: share `ImmutableArray` across threads, structural sharing
- ArrayPool: allocate large temp buffer, reuse across iterations
- Capacity hints: initialise `List<T>(capacity)` to avoid repeated doubling

### 3. LinqPerformance

**Folder structure:**
```
LinqPerformance/
  Program.cs                             # Orchestrator — calls all demos
  Concepts/
    MultipleEnumerationBug.cs            # Iterating same IEnumerable 2+ times
    StreamingVsMaterialising.cs          # Memory vs speed trade-off
    CountVsAny.cs                        # O(n) vs O(1) with early termination
  Examples/
    OptimisedQueryDemo.cs                # Streaming chain, final materialisation
    MissingCountProperty.cs              # .Count extension vs .Count property
    OrderByInLoopDemo.cs                 # Sort once, cache result
    SpanIteration.cs                     # CollectionsMarshal.AsSpan for hot paths
    FirstVsSingleDemo.cs                 # Early termination vs full scan
    ParallelLinqDemo.cs                  # PLINQ .AsParallel() profiling
    RealWorldOptimisationDemo.cs         # Combine techniques (streaming, Any, Span)
```

**Key demos:**
- Multiple enumeration: build a chain, show it's evaluated once per iteration
- Streaming vs materialising: `Where().Take(10)` vs `Where().ToList().Take(10)`
- `Any()` short-circuits after 1st match; `Count() > 0` doesn't
- `FirstOrDefault()` vs `SingleOrDefault()` — use `First` unless uniqueness guaranteed
- `OrderBy` in loop: sort 100 times vs sort once
- `Span<T>` iteration: `CollectionsMarshal.AsSpan(list)` for the fastest loop
- PLINQ `.AsParallel()`: show speedup on CPU-bound queries; warn about overhead
- Real-world: combine streaming + `Any()` + `Span` to minimise allocations

---

## Notes

### LINQ Internals
- **Deferred execution** is the key insight: LINQ operators return an `IEnumerable<T>` that doesn't run until you iterate.
- Each operator wraps the previous one; iteration triggers a chain of calls through the wrappers.
- `IEnumerable<T>` uses `Func<T, R>` delegates — instantiated at runtime, not translatable.
- `IQueryable<T>` uses `Expression<Func<T, R>>` — captured as a syntax tree, translatable to SQL or other backends.
- `yield return` in a method makes it an iterator — returns an enumerator on-demand, allocates minimally.
- Streaming operators (`Select`, `Where`) yield one item at a time; materialising operators (`ToList`, `Count`) force full evaluation.
- Query syntax (`from x in coll select y`) and method syntax (`coll.Select(y)`) compile identically; choose for readability.
- `GroupBy`, `Join`, `Except`, `Intersect`, `Union` express relational/set-style operations declaratively instead of nested loops — see `LinqInternals/Examples/SetAndJoinOperatorsExample.cs`.
- A custom `IEqualityComparer<T>` / `IComparer<T>` lets `HashSet<T>`/`Dictionary<K,V>`/`Sort`/`OrderBy` use domain-specific equality/ordering (e.g. case-insensitive keys, sort by length-then-alphabetical) — see `LinqInternals/Examples/CustomComparerExample.cs`.

### Collections
- Use `List<T>` for ordered, random-access collections; resizing is O(1) amortised.
- Use `Dictionary<K, V>` for O(1) key lookups; iteration order is undefined (linked hash maps in other languages preserve insertion order).
- Use `HashSet<T>` for membership tests and set operations; **never** use `List<T>.Contains()` in a loop.
- Use `Stack<T>` / `Queue<T>` for LIFO/FIFO; prefer them over `List<T>.Insert(0, ...)` / `RemoveAt(0)`.
- Use `SortedDictionary<K, V>` or `SortedSet<T>` only when you need sorted iteration; the O(log n) lookup cost is significant for large datasets.
- Use `LinkedList<T>` only if you already have a `LinkedListNode<T>` reference for O(1) mid-list insertion; otherwise `List<T>` is faster.
- Use `ImmutableArray<T>` / `ImmutableDictionary<K, V>` for safe sharing across threads without locks; structural sharing avoids full copies.
- Use `ArrayPool<T>.Shared.Rent()` / `Return()` to reuse large temporary buffers; critical for LOH scenarios (buffers ≥ 85KB).
- Initialise collections with a capacity hint (`new List<T>(capacity)`) to avoid repeated internal resizing during bulk operations.

### LINQ Performance
- **Always use `Any()` instead of `Count() > 0`** — `Any()` short-circuits after the first match.
- **Use `FirstOrDefault()` unless uniqueness is guaranteed** — `SingleOrDefault()` scans the entire sequence.
- **Cache materialisations** — if you iterate the same `IEnumerable<T>` multiple times, call `ToList()` once.
- **Prefer streaming chains** — `data.Where(...).Select(...).Where(...).Take(10)` is lazy and efficient; avoid `ToList()` in the middle.
- **Use the `.Count` property on `ICollection<T>` directly** — it's O(1); the `.Count()` LINQ extension enumerates.
- **Sort once, cache the result** — don't call `OrderBy` inside a loop; sort outside, reuse the result.
- **Use `CollectionsMarshal.AsSpan(list)`** for the fastest iteration over `List<T>` internals — zero allocations, full CPU optimisation.
- **Profile before using PLINQ** — `Parallel.AsParallel()` has overhead; only beneficial for large CPU-bound queries.
- **Combine techniques** — streaming + `Any()` + early termination + `Span<T>` = minimal allocations and maximum speed.
