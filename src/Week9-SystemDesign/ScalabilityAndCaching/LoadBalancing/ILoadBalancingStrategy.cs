namespace ScalabilityAndCaching.LoadBalancing;

/// <summary>A pluggable algorithm for choosing which backend server should handle the next request.</summary>
public interface ILoadBalancingStrategy
{
    string Name { get; }

    BackendServer SelectServer(IReadOnlyList<BackendServer> servers);
}
