---
applyTo: "src/Week4-LinqAndCollections/**"
---
# Week 4 — LINQ and Collections

## LINQ Internals & Deferred Execution
- Remember that LINQ operators on `IEnumerable<T>` are lazily evaluated — the query runs only when iterated.
- Materialise with `ToList()` / `ToArray()` when you need to iterate multiple times, cache results, or return a stable snapshot.
- Avoid multiple enumeration of the same `IEnumerable<T>` — enumerate once and store the result.
- Use `IQueryable<T>` (e.g., EF Core) when the query must be translated to SQL; use `IEnumerable<T>` for in-memory processing.
- Understand the difference between `expression trees` (IQueryable) and `delegates` (IEnumerable) for filtering.
- Implement custom sequences with `yield return` iterator methods rather than hand-writing `IEnumerator<T>`.
- Prefer `GroupBy`, `Join`, `Except`, `Intersect`, `Union` for set/relational-style operations instead of nested loops.

## Collection Choice
- Use `List<T>` for ordered, index-accessible, mutable sequences.
- Use `Dictionary<TKey, TValue>` for O(1) key lookups; use `SortedDictionary<K,V>` only when order matters.
- Use `HashSet<T>` for membership tests and set operations — do not use `List<T>.Contains` for this.
- Use `Queue<T>` / `Stack<T>` for FIFO / LIFO semantics; use `LinkedList<T>` only when frequent mid-list insertion/removal is needed.
- Use `ImmutableArray<T>` / `ImmutableDictionary<K,V>` when sharing read-only collections across threads.
- Initialise collections with a capacity hint when the size is known to avoid repeated internal resizing.
- Provide a custom `IEqualityComparer<T>` / `IComparer<T>` when default equality/ordering doesn't match domain semantics (e.g. case-insensitive keys).

## LINQ Performance
- Prefer `Any()` over `Count() > 0` — `Any()` short-circuits after the first element.
- Prefer `FirstOrDefault()` over `SingleOrDefault()` when uniqueness is guaranteed — `Single` scans the whole sequence.
- Use `Select` + `Where` chaining rather than nested loops for clarity; profile before switching to imperative loops for performance.
- Avoid `OrderBy` on large sequences inside tight loops — sort once, cache the result.
- Prefer `Span<T>`-based APIs (e.g., `CollectionsMarshal.AsSpan`) for high-performance iteration over `List<T>` contents.
- Use `Count` property on `ICollection<T>` directly instead of calling `.Count()` LINQ extension (avoids full enumeration).
