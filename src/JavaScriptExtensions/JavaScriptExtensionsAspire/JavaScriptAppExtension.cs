using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.JavaScript;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace JavaScriptExtensionsAspire;


public static class JavaScriptAppExtension
{
    extension(IResourceBuilder<JavaScriptAppResource> appResource)
    {
        public IResourceBuilder<JavaScriptAppResource> AddNpmCommandsFromPackage()
        {
            var builder = appResource.ApplicationBuilder;
            var wd = appResource.Resource.WorkingDirectory;
            var npm = appResource.Resource.Command;
            var packageJsonPath = Path.Combine(wd, "package.json");
            if (!File.Exists(packageJsonPath))
            {
                throw new FileNotFoundException($"Could not find package.json at {packageJsonPath}");
            }
            var packageJson = System.Text.Json.JsonDocument.Parse(File.ReadAllText(packageJsonPath));
            if (!packageJson.RootElement.TryGetProperty("scripts", out var scripts))
            {
                throw new Exception($"No scripts section found in package.json at {packageJsonPath}");
            }
            foreach (var script in scripts.EnumerateObject())
            {
                var commandName = script.Name;
                var commandValue = script.Value.GetString() ?? "";
                appResource = appResource.WithCommand(commandName, commandName, async (ecc) =>
                {
                    var loggerService = ecc.ServiceProvider.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
                    var logger = loggerService?.GetLogger(appResource.Resource);
                    if (logger == null)
                    {
                        Console.WriteLine($"Logger not found for {appResource.Resource.Name}");
                        return new ExecuteCommandResult() { Success = false, ErrorMessage = "Logger not found" };
                    }
                    var npmPath = "npm";

                    if (OperatingSystem.IsWindows())
                        npmPath = "npm.cmd";
                    npmPath = FullExeName(npmPath);
                    logger.LogInformation($"Exec {npmPath} run {commandName} in folder: {wd}");
                    var exportStartInfo = new ProcessStartInfo
                    {
                        FileName = npmPath,
                        Arguments = "run " + commandName,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WorkingDirectory = wd,
                        WindowStyle = ProcessWindowStyle.Hidden,
                    };

                    if (appResource.Resource.TryGetEnvironmentVariables(out var envCallback))
                    {
                        Dictionary<string, object> envDict = new();
                        EnvironmentCallbackContext environmentCallbackContext = new(builder.ExecutionContext, envDict);
                        foreach (var env in envCallback)
                        {
                            await env.Callback(environmentCallbackContext);

                        }
                        var envs = environmentCallbackContext.EnvironmentVariables;


                        foreach (var kvp in envs)
                        {
                            exportStartInfo.Environment[kvp.Key] = kvp.Value?.ToString() ?? "";
                        }
                    }
                    var result = await ExecuteProcess(exportStartInfo);
                    if (result.Success)
                    {
                        logger.LogInformation($"Executing {commandName} result: {result.Output}");
                        return new ExecuteCommandResult() { Success = true };
                    }
                    logger.LogError($"Executing {commandName} failed: {result.ErrorMessage}");
                    return new ExecuteCommandResult() { Success = false, ErrorMessage = result.ErrorMessage };
                });
            }
            return appResource;
        }
    }
    private static string FullExeName(string command)
    {
        var paths = Environment.GetEnvironmentVariable("PATH");
        if (paths == null) return command;
        var where = paths.Split(new char[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => Path.Combine(p, command))
            .FirstOrDefault(p => File.Exists(p));

        return where ?? command;
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
    private static async Task<ExecuteProcessResult> ExecuteProcess(ProcessStartInfo exportStartInfo)
    {
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
            if (string.IsNullOrWhiteSpace(resultError))
            {
                resultError = "No error message provided.See previous messages";

            }
            return new ExecuteProcessResult(resultStandard, resultError, exportProcess.ExitCode);
        }
        finally
        {
            await Task.WhenAll(stdOutTask ?? Task.CompletedTask, stdErrTask ?? Task.CompletedTask);
        }

    }
}

record ExecuteProcessResult(string Output, string Error, int ExitCode)
{
    /// <summary>
    /// Creates a successful execution result with the specified output.
    /// </summary>
    /// <param name="output">The standard output from the successful process execution.</param>
    /// <returns>An <see cref="ExecuteProcessResult"/> indicating success.</returns>
    public static ExecuteProcessResult SuccessResult(string output) => new ExecuteProcessResult(output, "", 0);

    /// <summary>
    /// Gets a value indicating whether the process execution was successful (exit code is 0).
    /// </summary>
    public bool Success => ExitCode == 0;

    /// <summary>
    /// Gets the error message if the process execution failed, or null if it was successful.
    /// </summary>
    public string? ErrorMessage => Success ? null : Error;
}
