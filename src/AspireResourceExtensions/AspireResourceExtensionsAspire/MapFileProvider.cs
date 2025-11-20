namespace AspireResourceExtensionsAspire;
class ReplaceFileProvider : IFileProvider
{
    private readonly IFileProvider _innerProvider;
    public ReplaceFileProvider(IFileProvider innerProvider)
    {
        _innerProvider = innerProvider;
    }
    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        var ret = _innerProvider.GetDirectoryContents(subpath);
        return ret;
    }
    public IFileInfo GetFileInfo(string subpath)
    {
        var ret= _innerProvider.GetFileInfo(subpath);
        return ret;
    }
    public Microsoft.Extensions.Primitives.IChangeToken Watch(string filter)
    {
        return _innerProvider.Watch(filter);
    }
}
internal static class MapFileProvider
{
    public static IFileProvider Manifest(this WebApplicationBuilder appBuilder)
    {
        var provManifest = new ManifestEmbeddedFileProvider(typeof(MapFileProvider).Assembly,"wwwroot");
        return new ReplaceFileProvider(provManifest);
    }
    
}
