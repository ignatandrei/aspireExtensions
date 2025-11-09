using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Dutchskull.Aspire.PolyRepo;

namespace DocumentorDatabaseExtensionsAspire;
public static class DocumentorDatabaseExtensions
{
    static string NormalizePathForCurrentPlatform(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return path;
        }

        // Fix slashes
        path = path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

        return Path.GetFullPath(path);
    }
    public static IResourceBuilder<ProjectResource> AddDocumentationOnFolder(this IResourceBuilder<SqlServerDatabaseResource> db, string folder)
    {
        var name = db.Resource.Name;
        var builder= db.ApplicationBuilder;
        var fullPath = NormalizePathForCurrentPlatform(folder);
        fullPath= Path.Combine(fullPath,name);
        var folderRepo = Path.Combine(fullPath, "repos");
        var folderDocusaurus = Path.Combine(fullPath, "docudb");
        var repository = builder
                .AddRepository(
            "repository" + name,
            "https://github.com/ignatandrei/aspireExtensions",
            c => c.WithDefaultBranch("EFCore9.0.10")
                .WithTargetPath(folderRepo)

                //.KeepUpToDate()
                )
               .WithParentRelationship(db)
               ;

        //db.ApplicationBuilder.
        var dotnetProject = builder
    .AddProjectFromRepository("docuDB" + name, repository,
        "generators/ShowDiagram/ShowDiagram.csproj")
        .WaitFor(db)
        .WithReference(db)
        .WithEnvironment("DocusaurusFolder", folderDocusaurus)
        .WithParentRelationship(db)
        ;
        return dotnetProject;
    }   
}
