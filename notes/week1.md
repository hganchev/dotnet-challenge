# Week 1 - C# Fundamentals

## Goal
Build a solid foundation in the core C# language concepts that underpin all .NET development: type system, object-oriented design, functional patterns via delegates, and reusable code via generics.

---

## 1. Value vs Reference Types

### Topics to Cover
| Topic | Key Points |
|---|---|
| Stack vs heap | Value types live on the stack; reference types on the heap |
| Value types | `struct`, `enum`, primitives (`int`, `bool`, `double`, etc.) |
| Reference types | `class`, `interface`, `delegate`, `array`, `string` |
| Boxing / unboxing | Implicit boxing, explicit unboxing, performance cost |
| `ref`, `out`, `in` | Pass value types by reference; `in` for read-only ref |
| Nullable value types | `int?` / `Nullable<T>`, null coalescing `??`, null checks |
| Equality semantics | `==` vs `Equals()` vs `ReferenceEquals()`; struct equality |
| `record` types | Value-based equality for reference types; `record struct` |

### Expected Outcomes
- Explain why `struct` mutations in methods require `ref`
- Describe the boxing overhead and when it matters
- Choose correctly between `class` and `struct` for a given scenario

---

## 2. OOP

### Topics to Cover
| Topic | Key Points |
|---|---|
| Classes and objects | Fields, properties, methods, constructors |
| Access modifiers | `public`, `private`, `protected`, `internal`, `private protected` |
| Properties | Auto-properties, expression-bodied, `init`-only setters |
| Encapsulation | Hide internals, expose minimal surface area |
| Inheritance | `base()` calls, constructor chaining, `sealed` |
| Abstract classes | Cannot be instantiated; define partial contracts |
| Interfaces | Default interface methods (C# 8+), explicit implementation |
| Polymorphism | `virtual`/`override`, method hiding with `new`, `is`/`as` |
| Static members | Static classes, singleton pattern, extension methods |
| Operator overloading | `==`, `+`, implicit/explicit conversion operators |
| Object / collection initializers | Syntactic sugar for construction |
| `record` types | Immutability, `with` expressions, positional records |

### Expected Outcomes
- Design a class hierarchy with correct use of abstract vs interface
- Explain when to prefer composition over inheritance
- Implement `IEquatable<T>` and `IComparable<T>` correctly

---

## 3. Delegates and Events

### Topics to Cover
| Topic | Key Points |
|---|---|
| Delegate declaration | `delegate` keyword, signature matching |
| Instantiation & invocation | Assign a method, call via delegate variable |
| Multicast delegates | `+=` / `-=` operators, `Delegate.Combine`, invocation order |
| Anonymous methods | `delegate(...)` inline syntax (pre-lambda pattern) |
| Lambda expressions | `=>` syntax, closures, captured variables |
| Built-in delegates | `Action<>`, `Func<>`, `Predicate<>` |
| Events | `event` keyword, publisher/subscriber pattern |
| `EventHandler` pattern | `EventHandler` and `EventHandler<TEventArgs>` |
| Custom `EventArgs` | Derive from `EventArgs`, carry event data |
| Event memory leaks | Strong reference from publisher to subscriber; when to unsubscribe |

### Expected Outcomes
- Explain the difference between a delegate and an event
- Convert a method-group delegate to a lambda and back
- Implement a safe event pattern that avoids memory leaks

---

## 4. Generics

### Topics to Cover
| Topic | Key Points |
|---|---|
| Generic classes | `class Foo<T>`, type parameter usage |
| Generic methods | Method-level type parameters, type inference |
| Type constraints | `where T : class`, `where T : struct`, `where T : new()`, interface constraints |
| Multiple constraints | Combining constraints with multiple `where` clauses |
| Generic interfaces | `IEnumerable<T>`, `IComparable<T>`, custom generic interfaces |
| Generic collections | `List<T>`, `Dictionary<TKey,TValue>`, `HashSet<T>` |
| `default` keyword | `default(T)` for zero-value initialization |
| Covariance (`out`) | Read-only generic type parameter; `IEnumerable<out T>` |
| Contravariance (`in`) | Write-only generic type parameter; `IComparer<in T>` |
| Generic delegates | `Func<>` / `Action<>` as generic delegates |

### Expected Outcomes
- Write a generic repository that works with any entity type
- Explain why `List<string>` cannot be assigned to `List<object>` (invariance)
- Use covariant/contravariant interfaces correctly

---

## Coverage Audit

### ✅ Value vs Reference Types
- [ ] File exists: `ValueVsReference/Program.cs`
- [ ] Stack/heap demonstration
- [ ] Boxing/unboxing example
- [ ] `ref` / `out` / `in` examples
- [ ] Nullable types

### ✅ OOP
- [ ] File exists: `OOP/Program.cs`
- [ ] Inheritance hierarchy
- [ ] Abstract class + interface
- [ ] Polymorphism demo
- [ ] Encapsulation with access modifiers

### Delegates and Events — see detailed audit below

### ✅ Generics
- [ ] File exists: `Generics/Program.cs`
- [ ] Generic class + method
- [ ] Constraints
- [ ] Covariance/contravariance

---

## Delegates and Events — Detailed Coverage Audit

| Topic | Covered? | File |
|---|---|---|
| Delegate declaration | ✅ | `Delegates/DelegateBasics.cs` |
| Instantiation & invocation | ✅ | `Delegates/DelegateBasics.cs` |
| Multicast (`Delegate.Combine`) | ✅ | `Delegates/DelegateBasics.cs` |
| Multicast (`+=` / `-=`) | ✅ | `Events/EventBasics.cs` |
| Lambda expressions | ✅ | `Delegates/BuildInDelegates.cs` |
| `Action<>` | ✅ | `Delegates/BuildInDelegates.cs` |
| `Func<>` | ✅ | `Delegates/BuildInDelegates.cs` |
| `Predicate<>` | ✅ | `Delegates/BuildInDelegates.cs` |
| Events / publisher-subscriber | ✅ | `Events/EventBasics.cs` |
| Custom `EventArgs` | ✅ | `Events/CustomEventArgs.cs` |
| Anonymous methods (`delegate` syntax) | ✅ | `Delegates/DelegateBasics.cs` |
| `EventHandler<TEventArgs>` pattern | ✅ | `Events/CustomEventArgs.cs` |
| Event memory leak awareness | ✅ | `Events/EventBasics.cs` |

### Gaps to Address
All gaps resolved. Delegates & Events topic is fully covered.
