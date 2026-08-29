using Aspire.Hosting.ApplicationModel;

namespace PortExtensionAspire;

public class PortResource() : Resource("PortResource")
    , IResourceWithParameters
    , IResourceWithEnvironment
{
    private MSPC mSPC = new();
    public IDictionary<string, object?> Parameters { get; } = new Dictionary<string, object?>();

    /// <summary>
    /// Returns a deterministic (repeatable) port for <paramref name="name"/>, caching it on first use.
    /// WARNING: uniqueness across different names is not guaranteed — hash collisions can assign the
    /// same port to two differently named resources. See <see cref="MSPC.GetDeterministicPort(string)"/>.
    /// </summary>
    public UInt16 GetDeterministicPort(string name)
    {
        if (Parameters.TryGetValue(name, out var existing))
        {
            return (UInt16)existing!;
        }
        var deterministicPort = mSPC.GetDeterministicPort(name);
        return SetDeterministicPort(name, deterministicPort);
    }
    public UInt16 SetDeterministicPort(string name, UInt16 port)
    {
        if (Parameters.TryGetValue(name, out var existing))
        {
            
            throw new InvalidOperationException($"Port for {name} is already set to {existing}, cannot set to {port}");
        }

        Parameters[name] = port;
        return port;
    }
    internal EnvironmentVariableSnapshot[] environmentVariables()
    {
        return this.Parameters.Select(kvp => new EnvironmentVariableSnapshot($"PORT_{kvp.Key}", kvp.Value?.ToString() ?? "", true)).ToArray();
    }
}
