using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Dutchskull.Aspire.PolyRepo;
using FileEmbed;
namespace DocumentorDatabaseExtensionsAspire;
public static partial class DocumentorDatabaseExtensions
{
    [FileEmbed(@"docudb.zip")]
    private static partial ReadOnlySpan<byte> BytesDocuDB();
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
        fullPath = Path.Combine(fullPath,name);
        string zipPath = Path.Combine(fullPath, "docudb.zip");
        string docuDBFolder = Path.Combine(fullPath, "docudb");
        if (!File.Exists(zipPath))
        {
            Directory.CreateDirectory(fullPath);
            File.WriteAllBytes(zipPath, BytesDocuDB().ToArray());
        }
        if(!Directory.Exists(docuDBFolder))
        {
            System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, docuDBFolder);
        }        
        var buildFolder= Path.Combine(docuDBFolder, "build");
        if(!Directory.Exists(buildFolder))
        {
            Directory.CreateDirectory(buildFolder);
            File.WriteAllText(Path.Combine(buildFolder, "index.html"), @$"""
<h1>Generate the documentation for {name}</h1>
""");
        }
        var folderRepo = Path.Combine(fullPath, "repos");
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
        .WithEnvironment("DocusaurusFolder", docuDBFolder)
        .WithParentRelationship(db)
        ;
        return dotnetProject;
    }   
}
