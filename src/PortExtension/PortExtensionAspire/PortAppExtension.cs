using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace PortExtensionAspire;

public static class PortAppExtension
{
    static PortResource instance = new PortResource();
    public static IResourceBuilder<PortResource> AddPort(this IDistributedApplicationBuilder builder)
    {

        return builder.AddResource<PortResource>(instance);

    }
    public static IResourceBuilder<PortResource> Construct(
        this IResourceBuilder<PortResource> builder)
    {

        var state = new CustomResourceSnapshot()
        {
            State = new(KnownResourceStates.Running, KnownResourceStateStyles.Success),
            ResourceType = "PortResource",
            Properties = [],
            EnvironmentVariables = [.. builder.Resource.environmentVariables()],

        };
        return builder.WithInitialState(state);
    }
    public static IResourceBuilder<PortResource> WithDeterministicPortEnvironment(
        this IResourceBuilder<PortResource> builder, params string[] names)
    {
        foreach (var name in names)
        {
            builder.Resource.GetDeterministicPort(name);
        }
        return builder;
    }

    /// <summary>
    /// Injects every deterministic port currently registered on <paramref name="portResource"/> as
    /// environment variables ("PORT_{name}") into <paramref name="builder"/>. Because this overload takes
    /// <see cref="IResourceBuilder{PortResource}"/> specifically (rather than the built-in generic
    /// <c>WithReference(object, ...)</c> overload), it is selected automatically by overload resolution
    /// whenever you call <c>.WithReference(portResource)</c> on another resource.
    /// The values are resolved lazily via a callback, after resource allocation, picking up any ports
    /// registered on <paramref name="portResource"/> up to that point.
    /// </summary>
    public static IResourceBuilder<TDestination> WithPortReference<TDestination>(
        this IResourceBuilder<TDestination> builder, IResourceBuilder<PortResource> portResource)
        where TDestination : IResourceWithEnvironment
    {
        return builder.WithEnvironment(ctx =>
        {
            foreach (var (name, value) in portResource.Resource.Parameters)
            {
                if (value is UInt16 port)
                {
                    ctx.EnvironmentVariables[$"PORT_{name}"] = port.ToString();
                }
            }
        });
    }
}
