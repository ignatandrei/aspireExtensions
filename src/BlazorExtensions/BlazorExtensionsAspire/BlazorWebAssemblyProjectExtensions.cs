using k8s.KubeConfigModels;
using k8s.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Blazor.Extension;

public static class BlazorWebAssemblyProjectExtensions
{

    private static string ReplaceEnvironmentVariable(string fileContent, string newEnvironmentValue)
    {
        var pattern = @"environment:\s*""[^""]*""";
        var replacement = $@"environment: ""{newEnvironmentValue}""";
        return Regex.Replace(fileContent, pattern, replacement);
    }


    extension<TRes>(IResourceBuilder<TRes> builder)
                where TRes : IResourceWithEnvironment, IProjectMetadata, new()
    {
        public IResourceBuilder<TRes> AddPathToEnvironmment<TProject>(
                TProject p, string name)
                where TProject : IProjectMetadata, new()
        {
            //var p = new TProject();        
            string pathPrj = p.ProjectPath;
            var fi = new FileInfo(pathPrj);
            string dirName = fi?.DirectoryName ?? "";
            var projectBuilder = builder
                .WithEnvironment(ctx =>
                {
                    ctx.EnvironmentVariables[name] = dirName;
                    ctx.EnvironmentVariables[$"{name}csproj"] = pathPrj;
                });

            return projectBuilder;
        }
    }


    public static IResourceBuilder<P> AddCommandsToModifyEnvName<P, TProject>(
        this IResourceBuilder<P> builder,
            TProject prj, params string[] nameOfEnvName)
        where TProject : IProjectMetadata, new()
        where P : ProjectResource
    {
        if (nameOfEnvName == null || nameOfEnvName.Length == 0)
            return builder;

        string pathPrj = prj.ProjectPath;
        foreach (var name in nameOfEnvName)
        {
            builder = builder.WithCommand(name, name, async context =>
            {
                var dist = context.ServiceProvider.GetService(typeof(DistributedApplicationModel)) as DistributedApplicationModel;
                var res = dist!.Resources.First(r => r is P);
                var loggerRes = context.ServiceProvider.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
                var logger = loggerRes?.GetLogger(context.ResourceName);
                var commandService = context.ServiceProvider.GetService(typeof(ResourceCommandService)) as ResourceCommandService; logger?.LogDebug($"Modifying environment variable {name} for project {pathPrj}");
                logger?.LogInformation($"Setting environment variable {name} to {pathPrj}");
                await commandService!.ExecuteCommandAsync(res, KnownResourceCommands.StopCommand);
                //AddWasmApplicationEnvironmentName(pathPrj, name);
                var folder= Path.GetDirectoryName(pathPrj);
                var wwwroot = Path.Combine(folder!, "wwwroot");
                var file = Path.Combine(wwwroot, "index.html");
                var fileContent = File.ReadAllText(file);

                fileContent = ReplaceEnvironmentVariable(fileContent, name);
                File.WriteAllText(file, fileContent);

                await commandService!.ExecuteCommandAsync(res, KnownResourceCommands.StartCommand);

                return CommandResults.Success();
            });
        }

        return builder;

    }
    extension(IDistributedApplicationBuilder builder)
    {

        public IResourceBuilder<ProjectResource> AddWebAssemblyProject<TProject>(
            string name,
            IResourceBuilder<ProjectResource> api)
            where TProject : IProjectMetadata, new()
        {

            var nameOfParameter = api.Resource.Name + "_host";
            var projectBuilder = builder.AddProject<TProject>(name);
            var p = new TProject();
            string hostApi = p.ProjectPath;
            var dir = Path.GetDirectoryName(hostApi);
            ArgumentNullException.ThrowIfNull(dir);
            var wwwroot = Path.Combine(dir, "wwwroot");
            if (!Directory.Exists(wwwroot))
            {
                Directory.CreateDirectory(wwwroot);
            }
            var file = Path.Combine(wwwroot, "appsettings.json");
            if (!File.Exists(file))
                File.WriteAllText(file, "{}");
            projectBuilder = projectBuilder.WithEnvironment(ctx =>
            {

                //var loggerService = ctx.ServiceProvider.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
                //var logger = loggerService?.GetLogger(testProject.Resource);
                if (!api.Resource.TryGetEndpoints(out var end))
                    return;
                if (!end.Any())
                    return;


                var fileContent = File.ReadAllText(file);

                Dictionary<string, object>? dict;
                if (string.IsNullOrWhiteSpace(fileContent))
                    dict = new Dictionary<string, object>();
                else
                    dict = JsonSerializer.Deserialize<Dictionary<string, object>>(fileContent!);

                ArgumentNullException.ThrowIfNull(dict);
                var val = end.FirstOrDefault()?.AllocatedEndpoint?.UriString ?? "";
                if (!val.EndsWith("/"))
                    val += "/";
                if (dict.ContainsKey(nameOfParameter))
                {
                    // If the value is already set and matches, we can skip writing it again


                    if (dict[nameOfParameter]?.ToString() == val)
                    {
                        ctx.Logger?.LogInformation($"Skipping writing {nameOfParameter} as it is already set to {val}");
                        return;
                    }
                }
                dict[nameOfParameter] = val;
                JsonSerializerOptions opt = new JsonSerializerOptions(JsonSerializerOptions.Default)
                { WriteIndented = true };
                File.WriteAllText(file, JsonSerializer.Serialize(dict, opt));
                ctx.Logger?.LogInformation($"Successfully writing {nameOfParameter} as it is already set to {val}");
                ctx.EnvironmentVariables[nameOfParameter] = val;

            });
            return projectBuilder
                .WithReference(api)
                .WaitFor(api);

        }
    }
}