using ScalabilityAndCaching.LoadBalancing;

namespace ScalabilityAndCaching.Examples;

/// <summary>Demonstrates round-robin vs least-connections load balancing side by side.</summary>
public static class LoadBalancingDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Load Balancing Simulation ===");

        Console.WriteLine("\n-- Round Robin --");
        var roundRobinServers = CreateServers();
        var roundRobin = new RoundRobinStrategy();
        for (var i = 1; i <= 6; i++)
        {
            var server = roundRobin.SelectServer(roundRobinServers);
            Console.WriteLine($"  Request {i} -> {server.Name}");
        }

        Console.WriteLine("\n-- Least Connections --");
        var leastConnectionsServers = CreateServers();
        // Simulate Server-A already handling 4 long-running connections before new traffic arrives.
        for (var i = 0; i < 4; i++)
        {
            leastConnectionsServers[0].IncrementConnections();
        }

        var leastConnections = new LeastConnectionsStrategy();
        for (var i = 1; i <= 6; i++)
        {
            var server = leastConnections.SelectServer(leastConnectionsServers);
            server.IncrementConnections();
            Console.WriteLine($"  Request {i} -> {server.Name} (now {server.ActiveConnections} active)");
        }

        Console.WriteLine();
        Console.WriteLine("Trade-off: round-robin is simple and stateless but ignores real load;");
        Console.WriteLine("least-connections adapts to uneven load but requires tracking per-server state.");
    }

    private static List<BackendServer> CreateServers() =>
    [
        new BackendServer("Server-A"),
        new BackendServer("Server-B"),
        new BackendServer("Server-C"),
    ];
}
