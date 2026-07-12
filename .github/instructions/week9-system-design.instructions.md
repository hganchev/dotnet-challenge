---
applyTo: "src/Week9-SystemDesign/**"
---
# Week 9 — System Design (Foundations)

## Scalability & Caching
- Prefer stateless service design in examples; call out explicitly where state would force session affinity.
- Use `IMemoryCache` for in-process caching demos; use `IDistributedCache` (Redis) for distributed caching demos — don't conflate the two.
- Always demonstrate an explicit cache invalidation strategy (TTL, explicit eviction, or versioned keys) — never leave a demo with unbounded cache growth.
- When simulating load balancing algorithms, model at least two (e.g. round-robin and least-connections) so trade-offs are visible side by side.

## Messaging & Event-Driven Architecture
- Use `System.Threading.Channels` for in-process pub/sub demos rather than pulling in a real broker dependency.
- Model the outbox pattern with an explicit "pending events" store that a separate dispatcher drains — don't publish directly inside the business transaction.
- Be explicit in comments about which delivery guarantee a demo provides (at-least-once vs at-most-once) and why exactly-once is achieved via idempotency, not the transport.

## Resilience & Rate Limiting
- Use `Polly` for retry/circuit-breaker/timeout policies; always pair retry with exponential backoff + jitter, never a fixed-delay retry loop.
- When implementing rate limiters from scratch, isolate the algorithm (token bucket, sliding window) behind an interface so it can be swapped and compared.
- Demonstrate idempotency keys with an explicit dictionary/store keyed by client-supplied key — show what happens on a duplicate request.

## Observability & Tracing
- Use `Serilog` for structured logging demos; always include a correlation ID enriched onto every log line in a request/flow.
- Use `OpenTelemetry` for tracing demos; propagate trace context across at least one async boundary (e.g. a background channel consumer) to show context flows beyond a single method call.
- Implement health checks via ASP.NET Core's `IHealthCheck`, distinguishing liveness (is the process up) from readiness (can it serve traffic).

## See also
- Distributed data/consensus, API gateway, database replication, security-at-scale, real-time/stream processing, and the capstone now live under `src/Week10-AdvancedSystemDesign/` — see `.github/instructions/week10-advanced-system-design.instructions.md`.
