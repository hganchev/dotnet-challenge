namespace ScalabilityAndCaching.LoadBalancing;

/// <summary>Represents a single backend instance behind a load balancer.</summary>
public class BackendServer
{
    public string Name { get; }
    public int ActiveConnections { get; private set; }

    public BackendServer(string name) => Name = name;

    public void IncrementConnections() => ActiveConnections++;

    public void DecrementConnections() => ActiveConnections = Math.Max(0, ActiveConnections - 1);
}
