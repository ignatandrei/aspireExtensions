
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace WebAPIDocsExtensions.AppHost.sdk;
internal static class WebAPIDocsExtensions
{
    public static IResourceBuilder<ContainerResource> AddSDKGeneration(this IDistributedApplicationBuilder builder, params IResourceBuilder<IResourceWithServiceDiscovery>[] projects)
    {
        var name = "sdkgen";
        var resource = builder
            .AddContainer(name, "openapitools/openapi-generator-online");
        resource
            .WithHttpEndpoint(port: 8888, targetPort: 8080)
            .WithHttpCommand("api/gen/clients", "Clients",
            commandOptions: new HttpCommandOptions()
            {
                Method = HttpMethod.Get,
                GetCommandResult = async (ctx) =>
                {
                    var log = ctx.ServiceProvider.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
                    var logger = log?.GetLogger(resource.Resource);
                    try
                    {
                        var response = await ctx.Response.Content.ReadAsStringAsync();
                        logger?.LogInformation($"Response: {response}");
                    }
                    catch (Exception ex)
                    {
                        return new ExecuteCommandResult() { Success = false, ErrorMessage = ex.Message };
                    }
                    return new ExecuteCommandResult() { Success = true };
                },
            })
            .WithHttpCommand("api/gen/clients/html2", "Html2",
            commandOptions: new HttpCommandOptions()
            {
                Method = HttpMethod.Post,
                PrepareRequest = (context) =>
                {
                    //var container = context.ServiceProvider.GetService(typeof(ContainerIDs)) as ContainerIDs;
                    //ArgumentNullException.ThrowIfNull(container);
                    //var containerId = container.GetContainerID(context.ResourceName);
                    //var http = container.GetHttpUrl(context.ResourceName, nameAPI);
                    //Console.WriteLine(containerId);
                    var endPoints = projects[0].Resource.GetEndpoints()?.ToArray() ?? [];
                    var first = endPoints.First(it => it.Url.Contains("http://")).Url.Replace("localhost", "host.docker.internal");
                    var http = first.EndsWith("/") ? first : first + "/";

                    var data = new
                    {
                        openAPIUrl = $"{http}openapi/v1.json"
                        //openapiNormalizer = [],
                        //options= { },
                        //spec= { }
                    };
                    context.Request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                    return Task.CompletedTask;
                },
                GetCommandResult = async (res) =>
                {
                    var log = res.ServiceProvider.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
                    var logger = log?.GetLogger(resource.Resource);
                    res.Response.EnsureSuccessStatusCode();
                    var text = await res.Response.Content.ReadAsStringAsync();

                    var resDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(await res.Response.Content.ReadAsStringAsync());
                    ArgumentNullException.ThrowIfNull(resDict);
                    if (!resDict.TryGetValue("link", out var url))
                    {

                        return new ExecuteCommandResult() { Success = false, ErrorMessage = "no link" };
                    }
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true // Ensures the default browser is used
                    });
                    return new ExecuteCommandResult() { Success = false, ErrorMessage = "test" };
                }
            });
        
        foreach (var proj in projects)
        {
            resource.WithReference(proj);
            resource.WaitFor(proj);
        };
        //var url = projects[0].Resource.GetEndpoints().FirstOrDefault();
        //resource.OnResourceEndpointsAllocated((res, evt, ct) =>
        //{

        //    var endPoints = projects[0].Resource.GetEndpoints()?.ToArray() ?? [];
        //    var first = endPoints.FirstOrDefault().Url;
        //    resource.WithCommand("ep" + first, "ep" + first, async (ecc) =>
        //    {
        //        await Task.Delay(1000);
        //        return new ExecuteCommandResult() { Success = true };
        //    }, null);
        //    return Task.CompletedTask;
        //});
        
            
            //var dir = projects[0].Name.Replace("-", "_");
            //var script = $"""
            //    #!/bin/sh
            //    set -e
            //    echo Generating SDK for {projects[0].Name} from ${{OPENAPI_URL:-{url}/swagger/v1/swagger.json}} into {dir}
            //    mkdir -p {dir}
            //    wget -q -O {dir}/openapi.json ${{OPENAPI_URL:-{url}/swagger/v1/swagger.json}}
            //    openapi-generator-cli generate -i {dir}/openapi.json -g csharp -o {dir} --additional-properties=packageName={projects[0].Name.Replace("-", "")}.Client,packageVersion=1.0.0,nunitVersion=3.12.0,netCoreProjectFile=true
            //    echo SDK generation completed.
            //    """;
            
            //var sb = new System.Text.StringBuilder();
            //sb.AppendLine("#!/bin/sh");
            //sb.AppendLine("set -e");
            //foreach (var proj in projects)
            //{
            //    var svc = proj.GetServiceDiscovery();
            //    var url = svc.GetHttpEndpoint().Url;
            //    var dir = proj.Name.Replace("-", "_");
            //    sb.AppendLine($"echo Generating SDK for {proj.Name} from ${{OPENAPI_URL:-{url}/swagger/v1/swagger.json}} into {dir}");
            //    sb.AppendLine($"mkdir -p {dir}");
            //    sb.AppendLine($"wget -q -O {dir}/openapi.json ${{OPENAPI_URL:-{url}/swagger/v1/swagger.json}}");
            //    sb.AppendLine($"openapi-generator-cli generate -i {dir}/openapi.json -g csharp -o {dir} --additional-properties=packageName={proj.Name.Replace("-", "")}.Client,packageVersion=1.0.0,nunitVersion=3.12.0,netCoreProjectFile=true");
            //}
            //sb.AppendLine("echo SDK generation completed.");
            //var script = sb.ToString();
            //Console.WriteLine($"Generated script:\n{script}");
            //res.ExecuteCommand(new[] { "sh", "-c", script });
        //});
        return resource;
    }
}    