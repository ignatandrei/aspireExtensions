using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace DocumentorDatabaseExtensionsAspire;

public class DocumentationResource: ExecutableResource,IResourceWithServiceDiscovery
{
    public DocumentationResource(string name,string workingDir) : base(name, "dotnet run", workingDir)
    {
    }
}
