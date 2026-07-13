namespace ScalabilityAndCaching.LoadBalancing;

/// <summary>Routes to whichever server currently has the fewest active connections. Adapts to uneven
/// load but requires the load balancer to track per-server state.</summary>
public class LeastConnectionsStrategy : ILoadBalancingStrategy
{
    public string Name => "Least Connections";

    public BackendServer SelectServer(IReadOnlyList<BackendServer> servers) =>
        servers.OrderBy(s => s.ActiveConnections).First();
}
