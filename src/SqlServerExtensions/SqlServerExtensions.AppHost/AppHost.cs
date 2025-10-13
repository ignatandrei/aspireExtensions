
using DataProject;
using SqlExtensionsAspire;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.SqlServerExtensions_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

//builder.AddProject<Projects.SqlServerExtensions_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithHttpHealthCheck("/health")
//    .WithReference(apiService)
//    .WaitFor(apiService);


var paramPass = builder.AddParameter("password", "myP@ssW0rd");

//string str = string.Join("\r\nGO\r\n", DBFiles.FilesToCreate);
var sqlserver = builder.AddSqlServer("sqlserver", paramPass, 1433)
    //.WithDbGate()
    ;

var db = sqlserver.AddDatabase("DepEmp")
    .WithSqlPadViewerForDB(sqlserver)    
    .WithSqlCommand("delete","delete from Employee")
    .ExecuteSqlServerScriptsAtStartup(DBFiles.FilesToCreate.ToArray())
    //.DropCreateDBCommand()
    //.ExecScripts(DBFiles.FilesToCreate)
    //.RecreateWithScripts(DBFiles.FilesToCreate)
    ;

builder.Build().Run();
