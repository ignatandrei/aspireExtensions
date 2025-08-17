using k8s.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Blazor.Extension;
public static class BlazorWebAssemblyProjectExtensions
{


    public static IResourceBuilder<TRes> AddPathToEnvironmment<TProject, TRes>(
            this IResourceBuilder<TRes> builder, TProject p, string name)
            where TProject : IProjectMetadata, new()
            where TRes : IResourceWithEnvironment
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

    public static IResourceBuilder<ProjectResource> AddWebAssemblyProject<TProject>(
        this IDistributedApplicationBuilder builder, string name,
        IResourceBuilder<ProjectResource> api)
        where TProject : Aspire.Hosting.IProjectMetadata, new()
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
