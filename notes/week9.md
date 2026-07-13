# Week 9 - System Design (Foundations)

## Goal
Learn how to reason about the foundational building blocks of large-scale distributed systems: scaling strategies, caching, asynchronous messaging, resilience/rate limiting, and observability. Advanced topics (distributed data/consensus, API gateways, database replication, security at scale, real-time/stream processing, and the end-to-end capstone) live in [week10.md](week10.md).

---

## 1. Scalability & Caching

### Topics to Cover
| Topic | Key Points |
|---|---|
| Horizontal vs vertical scaling | Stateless services, session affinity pitfalls, scale-out vs scale-up trade-offs |
| Load balancing | Round-robin, least-connections, weighted, health-check-aware routing |
| In-process caching | `IMemoryCache`, expiration policies (absolute/sliding), eviction callbacks |
| Distributed caching | `IDistributedCache`, Redis-backed cache, cache-aside vs write-through vs write-behind |
| Cache correctness | Invalidation strategies, cache stampede/thundering herd prevention, stale-while-revalidate |

### Expected Outcomes
- Simulate a load balancer choosing between multiple backend instances using at least two algorithms
- Implement cache-aside with `IMemoryCache` and explain when a distributed cache is required instead
- Demonstrate a cache invalidation strategy and explain how it avoids stampedes under concurrent load

---

## 2. Messaging & Event-Driven Architecture

### Topics to Cover
| Topic | Key Points |
|---|---|
| Pub/Sub | Decoupling producers from consumers; topics vs queues |
| Message brokers | Conceptual overview of Kafka / RabbitMQ / Azure Service Bus semantics |
| Outbox pattern | Guaranteeing at-least-once publish alongside a local DB transaction |
| Event sourcing (basics) | Storing state as a sequence of events; rebuilding state via replay |
| Delivery guarantees | At-most-once vs at-least-once vs exactly-once; idempotent consumers |

### Expected Outcomes
- Implement an in-memory pub/sub demo (e.g. via `System.Threading.Channels`) with multiple subscribers
- Implement a simplified outbox pattern that persists an event alongside a business change
- Explain why "exactly-once" delivery is effectively achieved via idempotent consumers, not the transport

---

## 3. Resilience & Rate Limiting

### Topics to Cover
| Topic | Key Points |
|---|---|
| Retry | Exponential backoff with jitter; when retries make things worse |
| Circuit breaker | Open/half-open/closed states; failing fast to protect downstream systems |
| Bulkhead & timeout | Isolating failures; bounding concurrent calls per dependency |
| Rate limiting | Token bucket, sliding window, fixed window algorithms |
| Idempotency | Idempotency keys for safe retries of non-idempotent operations (e.g. payments) |

### Expected Outcomes
- Wire up Polly policies (retry, circuit breaker, timeout) around a simulated flaky dependency
- Implement a token-bucket rate limiter from scratch and explain its trade-offs vs sliding window
- Design an idempotency-key mechanism for a "create order" style endpoint

---

## 4. Observability & Tracing

### Topics to Cover
| Topic | Key Points |
|---|---|
| Structured logging | Serilog, log enrichment, correlation IDs across service boundaries |
| Distributed tracing | OpenTelemetry spans/traces, propagating trace context across async/HTTP calls |
| Metrics | Counters, histograms, dashboards for latency/error-rate/throughput (RED/USE methods) |
| Health checks | Liveness vs readiness probes; `IHealthCheck` in ASP.NET Core |

### Expected Outcomes
- Instrument a small multi-step flow with OpenTelemetry and view the resulting trace
- Add structured logging with a correlation ID propagated across an async call chain
- Implement liveness/readiness health checks for a minimal API

---

## Coverage Audit

### Scalability & Caching
| Topic | Covered? | File |
|---|---|---|
| Load balancing simulation (round-robin, least-connections) | ✅ | `ScalabilityAndCaching/LoadBalancing/`, `Examples/LoadBalancingDemo.cs` |
| In-process caching (`IMemoryCache`) | ✅ | `ScalabilityAndCaching/Caching/CacheAsideProductService.cs` |
| Distributed caching (`IDistributedCache`) | ✅ | `ScalabilityAndCaching/Caching/DistributedCacheProductService.cs` |
| Cache invalidation / stampede prevention | ✅ | `ScalabilityAndCaching/Caching/StampedePreventionCacheService.cs` |

### Messaging & Event-Driven Architecture
| Topic | Covered? | File |
|---|---|---|
| Pub/Sub demo | ✅ | `MessagingAndEventDrivenArchitecture/PubSub/`, `Examples/PubSubDemo.cs` |
| Outbox pattern | ✅ | `MessagingAndEventDrivenArchitecture/Outbox/`, `Examples/OutboxDemo.cs` |
| Event sourcing basics | ✅ | `MessagingAndEventDrivenArchitecture/EventSourcing/`, `Examples/EventSourcingDemo.cs` |
| Delivery guarantees / idempotent consumer | ✅ | `MessagingAndEventDrivenArchitecture/DeliveryGuarantees/`, `Examples/DeliveryGuaranteesDemo.cs` |

### Resilience & Rate Limiting
| Topic | Covered? | File |
|---|---|---|
| Polly retry/circuit-breaker/timeout | ✅ | `ResilienceAndRateLimiting/Resilience/ResiliencePolicies.cs`, `Examples/PollyResilienceDemo.cs` |
| Rate limiter (token bucket / sliding window) | ✅ | `ResilienceAndRateLimiting/RateLimiting/`, `Examples/RateLimitingDemo.cs` |
| Idempotency keys | ✅ | `ResilienceAndRateLimiting/Idempotency/`, `Examples/IdempotencyDemo.cs` |

### Observability & Tracing
| Topic | Covered? | File |
|---|---|---|
| Structured logging + correlation IDs | ✅ | `ObservabilityAndTracing/Logging/OrderRequestFlow.cs`, `Examples/StructuredLoggingDemo.cs` |
| OpenTelemetry tracing | ✅ | `ObservabilityAndTracing/Tracing/`, `Examples/DistributedTracingDemo.cs` |
| Health checks (liveness vs readiness) | ✅ | `ObservabilityAndTracing/HealthChecks/`, `Examples/HealthChecksDemo.cs` |

### Gaps to Address
- None — all four Week 9 projects are fully implemented and smoke-tested (`dotnet build DotNetChallenge.sln`: 0 warnings/errors).
- See [week10.md](week10.md) for advanced/distributed-data topics, the API gateway/database/security/real-time projects, and the capstone
