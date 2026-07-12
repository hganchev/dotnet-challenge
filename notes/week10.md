# Week 10 - Advanced System Design

## Goal
Go beyond the Week 9 foundations into the topics that separate a passing system design discussion from a strong one: distributed data & consensus, service-to-service communication, database design at scale, security across service boundaries, and real-time/stream processing — culminating in a larger end-to-end capstone.

---

## 1. Distributed Data, Consistency & Consensus

### Topics to Cover
| Topic | Key Points |
|---|---|
| CAP theorem | Consistency, Availability, Partition tolerance — pick two under a partition |
| Partitioning/sharding | Range vs hash partitioning; hot-partition avoidance |
| Consistency models | Strong vs eventual consistency; read-your-writes; causal consistency |
| Consensus (basics) | Leader election concept; why distributed consensus is hard (conceptual Raft/Paxos, not a full implementation) |

### Expected Outcomes
- Explain a concrete scenario where an eventually-consistent design is an acceptable trade-off
- Design a hash-based sharding scheme for a dataset and identify its hot-partition risk
- Articulate what breaks (C, A, or P) in two contrasting system designs
- Simulate a basic leader-election flow and explain why split-brain is a risk without consensus

---

## 2. API Gateway & Service Communication

### Topics to Cover
| Topic | Key Points |
|---|---|
| REST vs gRPC | Payload/protocol trade-offs, streaming support, contract-first (protobuf) vs flexible JSON |
| API Gateway / BFF | Centralized routing, auth, rate limiting, request aggregation at the edge |
| Service discovery | Static registry vs dynamic lookup; health-aware routing |
| Synchronous vs asynchronous communication | When to call directly vs communicate via events (ties back to Week 9 messaging) |

### Expected Outcomes
- Compare a REST and gRPC contract for the same operation and justify a choice
- Implement a minimal API Gateway that routes to two downstream services and centralizes one cross-cutting concern (e.g. auth)
- Explain how service discovery avoids hard-coded downstream addresses

---

## 3. Database Design & Replication

### Topics to Cover
| Topic | Key Points |
|---|---|
| SQL vs NoSQL | Consistency/schema trade-offs; when a document/key-value/columnar store beats a relational DB |
| Indexing | How indexes speed up reads and slow down writes; composite index ordering |
| Replication | Primary/read-replica topology, replication lag, failover basics |
| Consistent hashing | Minimizing remapped keys on node add/remove; used for sharding and cache-node selection |

### Expected Outcomes
- Justify a SQL vs NoSQL choice for two contrasting workloads
- Explain how an index changes a query plan and its write-amplification cost
- Implement consistent hashing from scratch and demonstrate reduced key remapping vs modulo hashing when a node is added/removed

---

## 4. Security at Scale

### Topics to Cover
| Topic | Key Points |
|---|---|
| Cross-service authn/authz | Token propagation (e.g. JWT passed/validated across service boundaries), zero-trust basics |
| Encryption | In-transit (TLS) vs at-rest; where each is enforced in a distributed system |
| Secrets management | Never hard-code secrets; centralized secret stores, rotation |

### Expected Outcomes
- Demonstrate a token propagated from an edge gateway to a downstream service and validated there
- Explain where TLS termination happens in a typical gateway + service topology
- Show a secrets-management approach that avoids secrets in source control or config files

---

## 5. Real-Time & Stream Processing

### Topics to Cover
| Topic | Key Points |
|---|---|
| WebSockets / SSE / long polling | Trade-offs for pushing real-time updates to clients |
| Batch vs stream processing | ETL/batch windows vs continuous stream processing; latency vs throughput trade-offs |

### Expected Outcomes
- Implement a simple real-time push demo and explain why WebSockets/SSE beat polling for that scenario
- Contrast a batch pipeline and a streaming pipeline solving the same problem, and explain the latency/complexity trade-off

---

## 6. Capstone Advanced System Design

### Topics to Cover
| Concept | Key Points |
|---|---|
| Requirements gathering | Functional + non-functional requirements, capacity estimation (QPS, storage, bandwidth) |
| High-level design | API design, data model, component diagram spanning multiple services |
| Trade-off analysis | Justify choices tying together Week 9 foundations and Week 10 advanced topics |
| Case studies | Pick one large-scale system: ride-sharing dispatch, social media feed, or multi-region e-commerce checkout |

### Expected Outcomes
- Produce a written design doc (requirements → capacity estimate → high-level architecture → trade-offs) for one case study
- Implement a representative slice of the chosen design (core API + two cross-cutting concerns spanning both weeks)
- Present the design trade-offs as if defending them in a system design interview

---

## Coverage Audit

### Distributed Data, Consistency & Consensus
| Topic | Covered? | File |
|---|---|---|
| Sharding scheme design | ⬜ | `DistributedDataAndConsistency/` – not yet implemented |
| Consistency model comparison | ⬜ | `DistributedDataAndConsistency/` – not yet implemented |
| Leader-election/consensus simulation | ⬜ | `DistributedDataAndConsistency/` – not yet implemented |

### API Gateway & Service Communication
| Topic | Covered? | File |
|---|---|---|
| REST vs gRPC comparison | ⬜ | `ApiGatewayAndServiceCommunication/` – not yet implemented |
| Minimal API Gateway routing | ⬜ | `ApiGatewayAndServiceCommunication/` – not yet implemented |
| Service discovery basics | ⬜ | `ApiGatewayAndServiceCommunication/` – not yet implemented |

### Database Design & Replication
| Topic | Covered? | File |
|---|---|---|
| SQL vs NoSQL trade-off analysis | ⬜ | `DatabaseDesignAndReplication/` – not yet implemented |
| Indexing fundamentals | ⬜ | `DatabaseDesignAndReplication/` – not yet implemented |
| Replication / failover basics | ⬜ | `DatabaseDesignAndReplication/` – not yet implemented |
| Consistent hashing implementation | ⬜ | `DatabaseDesignAndReplication/` – not yet implemented |

### Security at Scale
| Topic | Covered? | File |
|---|---|---|
| Cross-service token propagation | ⬜ | `SecurityAtScale/` – not yet implemented |
| Encryption in-transit vs at-rest | ⬜ | `SecurityAtScale/` – not yet implemented |
| Secrets management | ⬜ | `SecurityAtScale/` – not yet implemented |

### Real-Time & Stream Processing
| Topic | Covered? | File |
|---|---|---|
| WebSockets/SSE real-time push demo | ⬜ | `RealTimeAndStreamProcessing/` – not yet implemented |
| Batch vs stream processing comparison | ⬜ | `RealTimeAndStreamProcessing/` – not yet implemented |

### Capstone Advanced System Design
| Topic | Covered? | File |
|---|---|---|
| Design doc for chosen case study | ⬜ | `CapstoneAdvancedSystemDesign/` – not yet implemented |
| Partial implementation of chosen design | ⬜ | `CapstoneAdvancedSystemDesign/` – not yet implemented |

### Gaps to Address
- Implement `DistributedDataAndConsistency`, `ApiGatewayAndServiceCommunication`, `DatabaseDesignAndReplication`, `SecurityAtScale`, `RealTimeAndStreamProcessing`, `CapstoneAdvancedSystemDesign`
