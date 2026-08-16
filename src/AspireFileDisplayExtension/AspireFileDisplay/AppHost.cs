using AspireFileDisplayExtension;

var builder = DistributedApplication.CreateBuilder(args);
var resDisplayFiles = builder.CreateFileDisplay(port: 55987);
resDisplayFiles.AddFile(relativePath: "AppHost.cs");
builder.Build().Run();