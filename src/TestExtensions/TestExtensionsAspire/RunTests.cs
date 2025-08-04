using System.Diagnostics;
using Microsoft.Extensions.Logging;
namespace TestExtensionsAspire;

public static class RunTests
{
    public static IResourceBuilder<ProjectResource> AddTestProject<TProject>(this IDistributedApplicationBuilder builder, string name, params string[] arguments)
       where TProject : IProjectMetadata, new()
    {
        var testProject = builder
            .AddProject<TProject>(name)
            .WithExplicitStart()
            ;
        var pathProject = new TProject().ProjectPath;

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
                var loggerService = ctx.ServiceProvider.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
                var logger = loggerService?.GetLogger(testProject.Resource);
                if(logger == null)
                {
                    Console.WriteLine($"Logger not found for {testProject.Resource.Name}");
                    return new ExecuteCommandResult() { Success = false, ErrorMessage = "Logger not found" };
                }
                logger.LogInformation($"Start Running {testCommand} for  {pathProject}");
                var data = await RunTestsForProject(pathProject, filter);
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
    static async Task<ExecuteProcessResult> RunTestsForProject(string pathProject, string filter)
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


