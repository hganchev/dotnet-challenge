namespace ScalabilityAndCaching.LoadBalancing;

/// <summary>Cycles through servers in a fixed order. Simple and stateless, but ignores actual server load.</summary>
public class RoundRobinStrategy : ILoadBalancingStrategy
{
    private int _next = -1;

    public string Name => "Round Robin";

    public BackendServer SelectServer(IReadOnlyList<BackendServer> servers)
    {
        var index = Interlocked.Increment(ref _next) % servers.Count;
        return servers[index];
    }
}
