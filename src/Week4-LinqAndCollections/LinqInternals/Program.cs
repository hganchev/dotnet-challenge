using LinqInternals.Concepts;
using LinqInternals.Examples;

Console.WriteLine("=== Week 4 — LINQ Internals ===\n");

// Concepts
DeferredExecutionExample.Run();
ExpressionTreeExample.Run();
IQueryableExample.Run();
YieldReturnExample.Run();

// Examples
QueryVsMethodSyntaxExample.Run();
StreamingOperatorsExample.Run();
MaterialisingOperatorsExample.Run();
AggregatorsExample.Run();

Console.WriteLine("\n=== Completed ===");
