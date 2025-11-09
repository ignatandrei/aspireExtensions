using DiagramDocusaurusGenerator;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateSlimBuilder(args);


builder.Services.AddDirectoryBrowser();
var app = builder.Build();
app.UseStaticFiles();
app.UseFileServer(enableDirectoryBrowsing: true);
var docusaurusPath = Environment.GetEnvironmentVariable("DocusaurusFolder") ?? string.Empty;
if(docusaurusPath != string.Empty)
{
    app.UseFileServer(new FileServerOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
            Path.Combine(docusaurusPath,"build")),
        RequestPath = "/docudb",
        EnableDirectoryBrowsing = true
    });
}
var logger = app.Services.GetRequiredService<ILogger<Program>>();

await foreach (var item in Task.WhenEach(app.RunAsync(), DoWork(logger)))
{
    logger.LogInformation($"Task completed with result: {item}");
}

static async Task<int> DoWork(ILogger<Program> logger)
{
    string? connectionString = null;
    var envs = Environment.GetEnvironmentVariables();
    string docuSaurusFolder = string.Empty;
    foreach (var key in envs.Keys)
    {
        if (key is string k && envs[key] is string v)
        {
            if(k == "DocusaurusFolder")
            {
                docuSaurusFolder = v;
                logger.LogInformation($"Docusaurus folder from env var: {docuSaurusFolder}");
            }
            if (k.StartsWith("ConnectionStrings"))
            {
                logger.LogInformation($"Env var: {k}={v}");
                connectionString = v;
            }
        }
    }
    if (connectionString is null)
    {
        logger.LogError("No connection string found in environment variables");
        return 10;
    }

    //dotnet new tool-manifest
    //dotnet tool install dotnet-ef
    //dotnet tool restore
    var folder = Environment.CurrentDirectory;
    logger.LogInformation($"Doing work in {folder}");
    var mermaidProjectFolder = Path.GetFullPath(Path.Combine(folder, "..", "Mermaid"));
    logger.LogInformation($"Looking for Mermaid project in {mermaidProjectFolder}");
    bool ok = false;
    ok = await LaunchProgram(mermaidProjectFolder, "dotnet", "build", logger);
    if (!ok)
    {
        return 20;
    }
    ok = await LaunchProgram(mermaidProjectFolder, "dotnet", "tool restore", logger);
    logger.LogInformation($"restore {ok}");
    if (!ok)
    {
        return 30;
    }
    string scaffold = $$"""
ef dbcontext scaffold "{{connectionString}}" Microsoft.EntityFrameworkCore.SqlServer --force
""";
    logger.LogInformation($"scaffold {scaffold}");
    ok = await LaunchProgram(mermaidProjectFolder, "dotnet", scaffold, logger);
    logger.LogInformation($"scaffold {ok}");
    if (!ok)
    {
        return 40;
    }
    string endContext = "Context.database.generated.mdx";
    var contextFiles = Directory.GetFiles(mermaidProjectFolder, $"*{endContext}", SearchOption.AllDirectories);
    
    foreach (var file in contextFiles)
    {
        string nameDB = Path.GetFileName(file).Replace($"{endContext}", "");
        logger.LogInformation($"Found database {nameDB} from context file: {file}");
        await CreateDocumentation(file, nameDB,docuSaurusFolder, logger);
    }
    logger.LogInformation($"Installing Docusaurus dependencies in {docuSaurusFolder}");
    await LaunchProgram(docuSaurusFolder,"npm","install", logger);
    logger.LogInformation($"Building Docusaurus site in {docuSaurusFolder}");
    await LaunchProgram(docuSaurusFolder, "npm", "run build", logger);
    logger.LogInformation($"DONE documentation");
    return 42;
}

static async Task CreateDocumentation(string file, string nameDB,string pathDocusaurus , ILogger<Program> logger)
{
    nameDB = nameDB.ToUpper();
    var folder = Path.GetDirectoryName(file);
    //var pathDocusaurus = Path.Combine(folder,  "..", "..", "..", "docudb");
    pathDocusaurus = Path.GetFullPath(pathDocusaurus);
    if (!Directory.Exists(pathDocusaurus))
    {
        logger.LogError($"Docusaurus folder not found: {pathDocusaurus}");
        return;
    }
    DataDocusaurus dataDocusaurus = new(pathDocusaurus);

    await dataDocusaurus.CreateDocumentation(folder,nameDB,  logger);

//    var markdownFile = Path.Combine(docsFolder, $"{nameDB}.mdx");
    
//    string partialExampleFile = $"_markdown-{nameDB}-database.mdx";
//    File.Copy(file, Path.Combine(docsFolder, partialExampleFile), true);
//    string partialExampleFileUser = $"_markdown-{nameDB}-database-user.mdx";
//    await File.WriteAllTextAsync(Path.Combine(docsFolder, partialExampleFileUser), );
    
//    using (var writer = new StreamWriter(markdownFile))
//    { 
//        var str = $$"""

//""";
//await writer.WriteAsync(str);
//    } 
    //here we will add tables

}


static async Task<bool> LaunchProgram(string folder, string exe, string args, ILogger logger)
{
    try
    {
        var resolvedExe = ResolveExecutable(exe, logger);

        ProcessStartInfo psi = new()
        {
            FileName = resolvedExe,
            Arguments = args,
            WorkingDirectory = folder,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        Process p = new Process();
        p.StartInfo = psi;
        StringBuilder output = new();
        StringBuilder error = new();

        p.OutputDataReceived += LogData(output, "OUTPUT");

        p.ErrorDataReceived += LogData(error, "ERROR");


        p.Start();
        p.BeginOutputReadLine();
        p.BeginErrorReadLine();
        await p.WaitForExitAsync();
        var ok = (p.ExitCode == 0) || (string.IsNullOrWhiteSpace(error.ToString()));
        if (!ok)
        {
            logger.LogError("OUTPUT:" + output.ToString());
            logger.LogError("----------------");
            logger.LogError("ERROR:" + error.ToString());
            logger.LogError("----------------");
        }
        return ok;
    }
    catch (Exception ex)
    {
        logger.LogError($"Error launching program {exe} {args} in folder {folder}: {ex.Message}");
        return false;
    }
}
static string? ResolveExecutable(string exe, ILogger logger)
{
    // Normalize for npm on Windows
    if (OperatingSystem.IsWindows())
    {
        if (exe.Equals("npm", StringComparison.OrdinalIgnoreCase))
        {
            exe = "npm.cmd";
        }
        if (exe.Equals("dotnet", StringComparison.OrdinalIgnoreCase))
        {
            exe = "dotnet.exe";
        }
    }
    // If already an absolute path and exists
    if (Path.IsPathRooted(exe) && File.Exists(exe))
        return exe;

    // Search PATH
    var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
    foreach (var segment in pathEnv.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    {
        try
        {
            var candidate = Path.Combine(segment, exe);
            if (File.Exists(candidate))
                return candidate;
        }
        catch { /* ignore invalid path segments */ }
    }

    logger.LogWarning($"Could not resolve full path for executable '{exe}'.");
    return exe;
}

static DataReceivedEventHandler LogData(StringBuilder output, string type)
{
    return (sender, e) =>
    {
        if (string.IsNullOrEmpty(e.Data))
        {
            return;
        }

        output.AppendLine(e.Data);

    };
}
