using k8s.KubeConfigModels;
using Microsoft.Extensions.Logging;
using NetEscapades.EnumGenerators;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Xml.Linq;

namespace DotnetGlobalToolsExtensionAspire;

public class DotnetGlobalToolResource: ExecutableResource
{
    public DotnetGlobalToolResource(): base("DotnetGlobalTool","dotnet","")
    {
        
        
    }
    
}

[Flags]
[EnumExtensions]
public enum ToolName: UInt64
{
    None = 0,
    [Display(Name = "programmerall")]
    programmerall = 1 << 0,
    [Display(Name ="dotnet-depends")]
    dotnet_depends  =1 << 1,
    [Display(Name = "dotnet-ef")]
    dotnet_ef = 1 << 2,
    [Display(Name = "dotnet-outdated")]
    dotnet_outdated = 1 << 3,
    [Display(Name = "dotnet-project-licenses")]
    dotnet_project_licenses =1 << 4,
    [Display(Name = "dotnet-property")]
    dotnet_property = 1 << 5,
    [Display(Name = "dotnet-repl")]
    dotnet_repl = 1 << 6,
    [Display(Name = "dotnetthx")]
    dotnetthx = 1 << 7,
    [Display(Name = "httprepl")]
    httprepl=1 << 8,
    [Display(Name = "netpackageanalyzerconsole")]
    netpackageanalyzerconsole=1 << 9,
    [Display(Name = "powershell")]
    powershell = 1 << 10,
    [Display(Name = "run-script")]
    run_script = 1 << 11,
    [Display(Name = "watch2")]
    watch2=1 << 12,
    
}
public static class DotnetGlobalToolResourceBuilderExtensions
{
    public static ToolName All()
    {
        ulong result = 0;
        foreach(var item in ToolNameExtensions.GetValuesAsUnderlyingType())
        {
            result += item;
        }
        return (ToolName)result;
    }
    public static IResourceBuilder<DotnetGlobalToolResource> AddDotnetGlobalTools(
        this IDistributedApplicationBuilder builder,
        ToolName toolName = ToolName.None
        )
    {
        List<string> toolNames = new();
        if (toolName != ToolName.None)
        {

            foreach (var item in ToolNameExtensions.GetValues())
            {
                if(item == ToolName.None)
                    continue;

                if (!toolName.HasFlagFast(item))
                    continue;

                var name = item.ToStringFast(true);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    toolNames.Add(name);
                }

            }
        }
        

        return AddDotnetGlobalTools(builder,toolNames.ToArray()) ;

    }
    private static async Task<ExecuteCommandResult> FromTool(string toolName,ILogger logger)
    {
        var exportStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = CommandLineUnInstall(toolName),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,


        };
        var result = await ExecuteProcess(exportStartInfo);
        logger.LogInformation($"Uninstalling {toolName} result: {result.Output}");

        exportStartInfo.Arguments = CommandLineInstall(toolName);
        result = await ExecuteProcess(exportStartInfo);
        if (result.Success)
        {
            logger.LogInformation($"Installing {toolName} result: {result.Output}");
            return new ExecuteCommandResult() { Success = true };
        }
        logger.LogError($"Installing {toolName} failed: {result.ErrorMessage}");
        return new ExecuteCommandResult() { Success = false, ErrorMessage = result.ErrorMessage };

    }
    public static IResourceBuilder<DotnetGlobalToolResource> AddDotnetGlobalTools(
        this IDistributedApplicationBuilder builder,
        params string[] arr
        )
    {
        ArgumentNullException.ThrowIfNull(builder);
        var resource = new DotnetGlobalToolResource();
        var res = builder.AddResource(resource).WithArgs(new string[]
        {
            "tool",
            "list",
            "-g"
        })
            .ExcludeFromManifest()            
            ;

        if ((arr?.Length ?? 0) == 0)
            return res;
        
        
        
        arr = arr!.Order().ToArray();

        res.WithCommand("All", "!All!", async ecc =>
        {
            var loggerService = ecc.ServiceProvider.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
            var logger = loggerService?.GetLogger(resource);

            if (logger == null)
            {
                Console.WriteLine($"Logger not found for {resource.Name}");
                return new ExecuteCommandResult() { Success = false, ErrorMessage = "Logger not found" };
            }
            var commandService = ecc.ServiceProvider.GetService(typeof(ResourceCommandService)) as ResourceCommandService;
            if (commandService == null)
            {
                Console.WriteLine($"command service not found for {resource.Name}");
                return new ExecuteCommandResult() { Success = false, ErrorMessage = "Logger not found" };
            }
            logger.LogInformation("Starting full install...");
            var nrSuccess = 0;
            foreach (var toolName in arr)
            {
                var resTool = await commandService.ExecuteCommandAsync(resource, toolName);
                if (resTool.Success)
                {
                    nrSuccess++;
                    logger.LogInformation($"Installing {toolName} succeeded.");
                }
                else
                {
                    logger.LogError($"Installing {toolName} failed: {resTool.ErrorMessage}");
                }
            }
            if (nrSuccess == arr.Length)
            {
                logger.LogInformation("All tools installed successfully.");
                return new ExecuteCommandResult() { Success = true };
            }
            else
            {
                //logger.LogWarning($"Only {nrSuccess} out of {arr.Length} tools were installed successfully.");
                return new ExecuteCommandResult() { Success = false, ErrorMessage = $"Only {nrSuccess} out of {arr.Length} tools were installed successfully." };
            }

        });



        foreach (var toolName in arr)
        {
            res.WithCommand(toolName, toolName,async (ExecuteCommandContext ecc)=>
            {
                var loggerService = ecc.ServiceProvider.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
                var logger = loggerService?.GetLogger(resource);

                if (logger == null)
                {
                    Console.WriteLine($"Logger not found for {resource.Name}");
                    return new ExecuteCommandResult() { Success = false, ErrorMessage = "Logger not found" };
                }
                
                return await FromTool(toolName, logger);
            });
        }
        return res;
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
    private static string CommandLineInstall(string name)
    {
        return $"tool install -g {name.Trim()} --verbosity normal --allow-roll-forward ";
    }
    private static string CommandLineUnInstall(string name)
    {
        return $"tool uninstall -g {name.Trim()}";
    }



}
