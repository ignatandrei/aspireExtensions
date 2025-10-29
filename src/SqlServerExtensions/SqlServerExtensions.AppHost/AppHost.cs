
using DataProject;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlExtensionsAspire;

var builder = DistributedApplication.CreateBuilder(args);

//var apiService = builder.AddProject<Projects.SqlServerExtensions_ApiService>("apiservice")
//    .WithHttpHealthCheck("/health");

//builder.AddProject<Projects.SqlServerExtensions_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithHttpHealthCheck("/health")
//    .WithReference(apiService)
//    .WaitFor(apiService);

builder.Services.AddFakeLogging();
var paramPass = builder.AddParameter("password", "myP@ssW0rd");

//string str = string.Join("\r\nGO\r\n", DBFiles.FilesToCreate);
var sqlserver = builder.AddSqlServer("sqlserver", paramPass, 1433)
    .WithLifetime(ContainerLifetime.Persistent)
    //.WithDbGate()
    ;

var db = sqlserver.AddDatabase("DepEmp")
    .WithSqlPadViewerForDB(sqlserver)    
    .WithSqlCommand("deleteEmployee","delete from Employee", ExecCommandEnum.NonQuery)
    .WithSqlCommand("selectEmployeeCount", "select count(*) from Employee", ExecCommandEnum.Scalar)
    .ExecuteSqlServerScriptsAtStartup(DBFiles.FilesToCreate.ToArray())
    //.DropCreateDBCommand()
    //.ExecScripts(DBFiles.FilesToCreate)
    //.RecreateWithScripts(DBFiles.FilesToCreate)
    ;

var da = builder.Build();

await Task.WhenAll(da.RunAsync(), Data(da));

static async Task<bool> Data(DistributedApplication da)
{
    Console.WriteLine("start");
    var services = da.Services.GetServices<ILoggerProvider>().ToArray();
    await Task.Delay(5000);
    Console.WriteLine("end");
    return true;
}
