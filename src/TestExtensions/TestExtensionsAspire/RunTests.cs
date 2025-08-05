using Aspire.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
namespace TestExtensionsAspire;

/// <summary>
/// Extension methods for adding test projects to .NET Aspire distributed applications.
/// </summary>
public static class RunTests
{
    /// <summary>
    /// Adds a test project as a resource to the distributed application builder with custom test commands.
    /// The test project is configured with explicit start, meaning it won't run automatically when the application starts.
    /// </summary>
    /// <typeparam name="TProject">The project metadata type that implements <see cref="IProjectMetadata"/>.</typeparam>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="name">The name for the test resource.</param>
    /// <param name="arguments">Array of test command arguments (e.g., "run --filter-trait 'Category=UnitTest'").</param>
    /// <returns>An <see cref="IResourceBuilder{ProjectResource}"/> that can be further configured.</returns>
    /// <example>
    /// <code>
    /// builder.AddTestProject&lt;Projects.MyApp_Tests&gt;("MyTests",
    ///     "run --filter-trait 'Category=UnitTest'",
    ///     "run --filter-trait 'Category=Integration'");
    /// </code>
    /// </example>
    public static IResourceBuilder<ProjectResource> AddTestProject<TProject>(this IDistributedApplicationBuilder builder, string name, params string[] arguments)
       where TProject : IProjectMetadata, new()
    {
        var testProject = builder
            .AddProject<TProject>(name, c =>
            {
                c.ExcludeLaunchProfile = true;
            })
            .WithExplicitStart() 
            ;
        
        var pathProject = new TProject().ProjectPath;

        //var y = ExecutableResourceBuilderExtensions.AddExecutable(builder, "andrei", "dotnet", Path.GetDirectoryName(pathProject)!, ["run" ,"--filter-trait","'Category=UnitTest'"]);
        //y.WithParentRelationship(testProject);
        //y.WithEnvironment("ASD", "ASDSSDA").WithExplicitStart();
        //y.OnBeforeResourceStarted(async (res, bef,ct) => 
        //{
        //    await Task.Delay(100);
        //    testProject.Resource.TryGetEnvironmentVariables

        //});
        for (var i = 0; i < arguments.Length; i++)
        {
            var filter = arguments[i];
            var testName = $"Test_{i}";
            var testFilter = $"\"{filter}\"";
            var testCommand = $"dotnet {testFilter}";
            testProject
                .WithCommand(
            name: testName,
            displayName: testCommand,
            executeCommand: (async (ctx) =>
            {
                var b= testProject.Resource.TryGetEnvironmentVariables(out var envCallback);
                envCallback ??= [];
                Dictionary<string, object> envDict = new();
                EnvironmentCallbackContext environmentCallbackContext = new(builder.ExecutionContext, envDict);
                foreach (var env in envCallback)
                {
                    await env.Callback(environmentCallbackContext);

                }
                var envs = environmentCallbackContext.EnvironmentVariables;
                var loggerService = ctx.ServiceProvider.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
                var logger = loggerService?.GetLogger(testProject.Resource);
                
                if (logger == null)
                {
                    Console.WriteLine($"Logger not found for {testProject.Resource.Name}");
                    return new ExecuteCommandResult() { Success = false, ErrorMessage = "Logger not found" };
                }
                logger.LogInformation($"Start Running {testCommand} for  {pathProject}");
                var data = await RunTestsForProject(pathProject, filter,envs);
                logger.LogInformation(data.Output);
                if (data.Success)
                {
                    logger.LogInformation($"Test with {filter} finished with success");

                    return new ExecuteCommandResult() { Success = true };
                }
                logger.LogError($"Test with {filter} finished with error {data.ErrorMessage}");
                return new ExecuteCommandResult() { Success = false, ErrorMessage = data.ErrorMessage };
            }
            ),
            commandOptions: new CommandOptions()
            {
                IconName = "PersonRunningFilled",
                IconVariant = IconVariant.Filled,
            }
        );

        }


        return testProject;
    }
    
    /// <summary>
    /// Executes tests for a specific project with the given filter.
    /// </summary>
    /// <param name="pathProject">The path to the project file.</param>
    /// <param name="filter">The test filter arguments to pass to dotnet test.</param>
    /// <returns>A task that represents the asynchronous test execution operation. The task result contains the execution result.</returns>
    static async Task<ExecuteProcessResult> RunTestsForProject(string pathProject, string filter, Dictionary<string, object> envs)
    {
        var folder = Path.GetDirectoryName(pathProject);
        var exportStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"{filter}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            WorkingDirectory = folder,
            
        };
        foreach (var env in envs)
        {
            if (env.Value is EndpointReference ref1)
            {
                exportStartInfo.Environment.Add(env.Key, ref1.Url);
                continue;
            }
            exportStartInfo.Environment.Add(env.Key, env.Value?.ToString());
        }
        var exportProcess = new Process { StartInfo = exportStartInfo };

        Task? stdOutTask = null;
        Task? stdErrTask = null;
        string resultStandard = "";
        string resultError = "";
        try
        {
            try
            {
                if (exportProcess.Start())
                {
                    stdOutTask = ConsumeOutput(exportProcess.StandardOutput, msg => resultStandard += msg);
                    stdErrTask = ConsumeOutput(exportProcess.StandardError, msg => resultError += msg);
                }
            }
            catch (Exception ex)
            {
                return new ExecuteProcessResult("", ex.Message, int.MinValue);
            }

            var timeout = TimeSpan.FromMinutes(5);
            var exited = exportProcess.WaitForExit(timeout);

            if (exportProcess.HasExited && exportProcess.ExitCode == 0 && string.IsNullOrWhiteSpace(resultError))
            {
                return ExecuteProcessResult.SuccessResult(resultStandard);
            }
            if (!exportProcess.HasExited)
            {
                exportProcess.Kill(true);
            }
            int nr = exportProcess.ExitCode;
            if (nr == 0) nr = int.MinValue;
            if(string.IsNullOrWhiteSpace(resultError))
            {
                resultError = "No error message provided.See previous messages";
            }
            return new ExecuteProcessResult(resultStandard, resultError, exportProcess.ExitCode);
        }
        finally
        {
            await Task.WhenAll(stdOutTask ?? Task.CompletedTask, stdErrTask ?? Task.CompletedTask);
        }

        static async Task ConsumeOutput(TextReader reader, Action<string> callback)
        {
            char[] buffer = new char[256];
            int charsRead;

            while ((charsRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                callback(new string(buffer, 0, charsRead));
            }
        }
    }
}


