# EF Core LEFT/RIGHT JOIN: EF Core ≤ 9 (LINQ workaround) vs EF Core 10 (`LeftJoin`/`RightJoin`)

## What changed in EF Core 10

.NET 10 adds first-class LINQ standard query operators `LeftJoin` and `RightJoin`
(`System.Linq.Enumerable`/`Queryable`). EF Core 10 recognizes both and translates
them directly to SQL `LEFT JOIN` / `RIGHT JOIN` — no more hand-rolling the
`SelectMany` + `GroupJoin` + `DefaultIfEmpty` dance to express an outer join.

```csharp
// EF Core ≤ 9 — the classic "left outer join" LINQ pattern
var query =
    from customer in context.Customers
    join order in context.Orders
        on customer.Id equals order.CustomerId into customerOrders
    from order in customerOrders.DefaultIfEmpty()   // ← makes it a LEFT JOIN
    select new
    {
        customer.Name,
        OrderTotal = order != null ? order.Total : 0m
    };

// EF Core 10 — same result, method-syntax, no GroupJoin/DefaultIfEmpty needed
var query = context.Customers
    .LeftJoin(
        context.Orders,
        customer => customer.Id,
        order => order.CustomerId,
        (customer, order) => new
        {
            customer.Name,
            OrderTotal = order != null ? order.Total : 0m
        });
```

---

## Side-by-side comparison

| Capability | EF Core ≤ 9 / .NET 9 | EF Core 10 / .NET 10 |
|---|---|---|
| **Inner join** | ✔ `join ... on ... equals ...` / `.Join(...)` | ✔ unchanged — still the right tool for inner joins |
| **Left outer join** | ✘ no dedicated operator — requires `GroupJoin` into a temp collection, then `SelectMany` + `.DefaultIfEmpty()` | ✔ `.LeftJoin(...)` — one method call |
| **Right outer join** | ✘ not directly expressible — must swap the two sequences and write it as a left join, then reorder the projection | ✔ `.RightJoin(...)` — dedicated operator, keeps all rows from the *second* sequence |
| **Query syntax (`from x in ... select`)** | Supports left join via `into` + `DefaultIfEmpty()` | Still no query-syntax equivalent for `LeftJoin`/`RightJoin` — method syntax only |
| **Readability** | Easy to get wrong (forgetting `DefaultIfEmpty()` silently turns it into an inner join) | Explicit, self-documenting method name |
| **SQL generated** | `LEFT JOIN` (via query-syntax pattern) | `LEFT JOIN` / `RIGHT JOIN` (both natively supported) |

---

## Code examples

### Left join

```csharp
// EF Core ≤ 9 — must use GroupJoin + SelectMany + DefaultIfEmpty
var customersWithOrders =
    from customer in context.Customers
    join order in context.Orders
        on customer.Id equals order.CustomerId into customerOrders
    from order in customerOrders.DefaultIfEmpty()
    select new
    {
        customer.Name,
        OrderId = order == null ? (int?)null : order.Id
    };

// EF Core 10 — dedicated LeftJoin operator
var customersWithOrders = context.Customers
    .LeftJoin(
        context.Orders,
        customer => customer.Id,
        order => order.CustomerId,
        (customer, order) => new
        {
            customer.Name,
            OrderId = order == null ? (int?)null : order.Id
        });
```

### Right join

```csharp
// EF Core ≤ 9 — no RightJoin exists; simulate by swapping sides and
// writing it as a left join from Orders back to Customers
var ordersWithCustomers =
    from order in context.Orders
    join customer in context.Customers
        on order.CustomerId equals customer.Id into orderCustomers
    from customer in orderCustomers.DefaultIfEmpty()
    select new
    {
        order.Id,
        CustomerName = customer == null ? "Unknown" : customer.Name
    };

// EF Core 10 — RightJoin keeps all rows from the second sequence (Orders),
// matching rows from the first (Customers) — no need to swap anything
var ordersWithCustomers = context.Customers
    .RightJoin(
        context.Orders,
        customer => customer.Id,
        order => order.CustomerId,
        (customer, order) => new
        {
            order.Id,
            CustomerName = customer == null ? "Unknown" : customer.Name
        });
```

---

## Backwards compatibility

The `join ... into ... DefaultIfEmpty()` pattern still compiles and translates to
SQL exactly as before on EF Core 10 — nothing is removed. `LeftJoin`/`RightJoin`
are purely additive and only available in method syntax
(`from x in ... select` query syntax has no keyword for them).

> See `src/Week6-EFCore/QueryOptimization/` (net10.0, EF Core 10.0.9, `Customer`/
> `Order`/`OrderItem` entities) for a project already on the EF Core 10 baseline
> where these operators can be tried directly.
