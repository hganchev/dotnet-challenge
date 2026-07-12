using LinqPerformance.Concepts;
using LinqPerformance.Examples;

Console.WriteLine("=== Week 4 — LINQ Performance ===\n");

// Concepts
MultipleEnumerationBugExample.Run();
StreamingVsMaterialisingExample.Run();
CountVsAnyExample.Run();

// Examples
OptimisedQueryExample.Run();
MissingCountPropertyExample.Run();
OrderByInLoopExample.Run();
SpanIterationExample.Run();
FirstVsSingleExample.Run();
ParallelLinqExample.Run();
RealWorldOptimisationExample.Run();

Console.WriteLine("\n=== Completed ===");
