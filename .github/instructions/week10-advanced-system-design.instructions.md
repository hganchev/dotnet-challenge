---
applyTo: "src/Week10-AdvancedSystemDesign/**"
---
# Week 10 — Advanced System Design

## Distributed Data, Consistency & Consensus
- Keep CAP theorem and consistency-model discussions grounded in a concrete scenario (e.g. "if the partition heals, what value wins?") rather than purely abstract explanation.
- For sharding demos, make the hot-partition risk visible (e.g. skewed key distribution) rather than only showing the happy path.
- Leader-election/consensus demos should stay conceptual (illustrate the split-brain problem) — do not attempt a full Raft/Paxos implementation.

## API Gateway & Service Communication
- When comparing REST vs gRPC, show the same operation modeled both ways rather than describing the trade-off only in prose.
- A gateway demo must centralize at least one real cross-cutting concern (auth, rate limiting, or request aggregation) — a pure pass-through proxy doesn't demonstrate the pattern.
- Service discovery demos should show what breaks when a downstream address is hard-coded vs looked up dynamically.

## Database Design & Replication
- Justify SQL vs NoSQL choices against a concrete workload (e.g. "many small writes with flexible schema" vs "relational integrity with joins") — never present it as a universal ranking.
- Consistent hashing must be implemented from scratch (a virtual-node ring), not simulated with a comment — the goal is to show reduced key remapping vs modulo hashing.
- Replication demos should show replication lag as an explicit, visible effect (e.g. a stale read from a replica), not just describe it.

## Security at Scale
- Never hard-code secrets, tokens, or connection strings in demo code — use configuration/environment variables even in a console sample, and call out where a real secrets manager would sit.
- Token-propagation demos must show the token being validated at the downstream service, not just passed through unchecked.
- Be explicit about where TLS termination happens in the demoed topology (gateway vs service).

## Real-Time & Stream Processing
- Real-time push demos should contrast against a polling baseline so the trade-off is visible, not just implement WebSockets/SSE in isolation.
- Batch vs stream processing demos should use the same sample dataset/problem for both approaches so the latency/complexity trade-off is directly comparable.

## Capstone Advanced System Design
- Every case study must include: functional + non-functional requirements, a capacity estimate (QPS/storage/bandwidth), a component diagram, and explicit trade-off justifications tying back to both Week 9 and Week 10 topics.
- Implement only a representative slice of the design (core API + two cross-cutting concerns) — the capstone is not meant to be a production-complete system.
