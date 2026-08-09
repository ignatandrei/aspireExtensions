using Aspire.Hosting.ApplicationModel;

namespace PortExtensionAspire;

public class PortResource() : Resource("PortResource")
    , IResourceWithParameters
    , IResourceWithEnvironment
{
    private MSPC mSPC = new();
    public IDictionary<string, object?> Parameters { get; } = new Dictionary<string, object?>();
    public UInt16 GetDeterministicPort(string name)
    {
        if (Parameters.TryGetValue(name, out var existing))
        {
            return (UInt16)existing!;
        }
        var deterministicPort = mSPC.GetDeterministicPort(name);
        Parameters[name] = deterministicPort;
        return deterministicPort;
    }
    internal EnvironmentVariableSnapshot[] environmentVariables()
    {
        return this.Parameters.Select(kvp => new EnvironmentVariableSnapshot($"PORT_{kvp.Key}", kvp.Value?.ToString() ?? "", true)).ToArray();
    }
}
