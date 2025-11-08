using DiagramDocusaurusGenerator.Templates;

namespace DiagramDocusaurusGenerator;

public class DataDocusaurus(string folderDocusaurus)
{
    public async Task CreateDocumentation(string folderWithFilesGenerated,string nameDB, ILogger logger)
    {
        var docsDatabasesFolder = Path.Combine(folderDocusaurus, "docs", "databases");
        if (!Directory.Exists(docsDatabasesFolder))
        {
            Directory.CreateDirectory(docsDatabasesFolder);
        }
        var docsDatabase = Path.Combine(docsDatabasesFolder, nameDB);
        if (!Directory.Exists(docsDatabase))
        {
            Directory.CreateDirectory(docsDatabase);
        }
        string newFile = "";
        string fileCopy = "";
        DatabaseModelReact databaseModelReact = new() { nameDB = nameDB };
        var user=new DatabaseUser(databaseModelReact);
        newFile = Path.Combine(docsDatabase, databaseModelReact.partialDocumentFileUser);
        await File.WriteAllTextAsync(newFile, await user.RenderAsync());
        newFile = Path.Combine(docsDatabase, databaseModelReact.partialDocumentFile);
        fileCopy = Path.Combine(folderWithFilesGenerated, $"{nameDB}Context.generated.mdx");
        logger.LogInformation($"Creating mermaid file for database {nameDB} in {newFile} from {fileCopy}");
        File.Copy(fileCopy, newFile, true);
        newFile= Path.Combine(docsDatabase, "index.mdx");
        var docDatabase = new DatabaseFull(databaseModelReact);
        await File.WriteAllTextAsync(newFile, await docDatabase.RenderAsync());
    }
}
