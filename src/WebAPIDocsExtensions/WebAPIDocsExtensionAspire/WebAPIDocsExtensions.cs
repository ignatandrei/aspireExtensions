using ConsoleOpinionated;

namespace WebAPIDocsExtensionAspire;

public static class WebAPIDocsExtensions
{
    public static IResourceBuilder<ContainerResource> AddSDKGeneration_openapitools(this IResourceBuilder<ProjectResource> project, string downloadFolder = "wwwroot/docs", string jsonPath= "openapi/v1.json")
    {
        if (!Path.IsPathRooted(downloadFolder))
        {
            var pathProject = project.Resource.GetProjectMetadata()?.ProjectPath ?? "";
            var dir = Path.GetDirectoryName(pathProject) ?? ".";
            downloadFolder = Path.GetFullPath(Path.Combine(dir, downloadFolder));
        }
        var builder = project.ApplicationBuilder;
        var name = "sdkgen-" + project.Resource.Name;
        var resource = builder
            .AddContainer(name, "openapitools/openapi-generator-online")
            .WithReference(project)
            .WithAnnotation(new AnnotationClients($"Clients{name}"))
            .WaitFor(project)
            .ExcludeFromManifest();
        resource.WithParentRelationship(project);
        resource.OnResourceReady(async (res, evt, ct) =>
        {
            //#pragma warning disable ASPIREINTERACTION001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            //var interaction= evt.Services.GetService(typeof(IInteractionService)) as IInteractionService;
            var log = evt.Services.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
            var logger = log?.GetLogger(res);
            logger?.LogInformation($"!!!!!!!!Container {name} is ready.");
            await Task.Delay(15000);
            var endpoints = res.GetEndpoints()?.ToArray();
            endpoints?.ToList().ForEach(ep => logger?.LogInformation($"!!!Endpoint: {ep.Url}"));
            if (endpoints == null || endpoints.Length == 0)
            {
                logger?.LogError($"!!!!!!!!Container {name} has no endpoints.");
                return;
            }
            var first = endpoints.First().Url;
            logger?.LogInformation($"!!!!!!!!Container {name} first endpoint: {first}");
            HttpClient client = new();
            client.BaseAddress = new Uri(first);
            try
            {
                var response = await client.GetAsync("api/gen/clients");
                response.EnsureSuccessStatusCode();
                var text = await response.Content.ReadAsStringAsync();
                logger?.LogInformation($"!!!Clients: {text}");
                var ann = res.Annotations.FirstOrDefault(it => it is AnnotationClients) as AnnotationClients;
                if (ann == null)
                {
                    logger?.LogError($"!!!!!!!!Container {name} has no annotation.");
                    return;
                }
                ann.Url = first;
                ann.Data = text
                    .Replace("[", "")
                    .Replace("]", "")
                    .Replace("\"", "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries);

                //if(interaction?.IsAvailable??false)
                //    interaction?.PromptMessageBoxAsync($"SDK Generation Service is ready at {first}"
                //        , $"# Clients {Environment.NewLine} {text}"
                //        ,new MessageBoxInteractionOptions()
                //        {
                //             EnableMessageMarkdown = true,

                //        }
                //        );
                //#pragma warning restore ASPIREINTERACTION001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            }
            catch (Exception ex)
            {
                logger?.LogError($"Health check failed: {ex.Message}");
            }

        });

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
            .WithCommand("opinionated", "opinionated", async (ctx) =>
            {
                var log = ctx.ServiceProvider.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
                var logger = log?.GetLogger(resource.Resource);

                var commandService = ctx.ServiceProvider.GetService(typeof(ResourceCommandService))as ResourceCommandService;
                if(commandService==null)
                {
                    return CommandResults.Failure("No command service");
                }
                var result = await commandService.ExecuteCommandAsync(resource.Resource, "genClients");
                if(!result.Success)
                {
                    return CommandResults.Failure(result.ErrorMessage);
                }

                var pathSave = downloadFolder;
                await (new ConsoleOpinionated.ExportOpinionated()).Generate(pathSave!);
                var index=Path.Combine(pathSave!, "index.html");
                logger?.LogInformation($"Opinionated docs generated in {index} ");
                return CommandResults.Success();

            })
            .WithCommand("genClients", "genClients", async (ctx) =>
            {
                var endPoints = project.Resource.GetEndpoints()?.ToArray() ?? [];
                var first = endPoints.First(it => it.Url.Contains("http://")).Url.Replace("localhost", "host.docker.internal");
                var http = first.EndsWith("/") ? first : first + "/";

                var log = ctx.ServiceProvider.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
                var logger = log?.GetLogger(resource.Resource);
                var ann = resource.Resource.Annotations.FirstOrDefault(it => it is AnnotationClients) as AnnotationClients;
                if (ann == null)
                {
                    return new ExecuteCommandResult() { Success = false, ErrorMessage = "No annotation found" };
                }
                if (ann.Data.Length == 0)
                {
                    return new ExecuteCommandResult() { Success = false, ErrorMessage = "No clients available" };
                }
                logger?.LogInformation("Available clients:");
                HttpClient httpClient = new();
                httpClient.BaseAddress = new Uri(ann.Url);
                foreach (var client in ann.Data)
                {
                    logger?.LogInformation($"- {client}");
                    byte[] data = await GetClientZip(httpClient, client, $"{http}{jsonPath}");
                    if (!Directory.Exists(downloadFolder))
                    {
                        Directory.CreateDirectory(downloadFolder);
                    }
                    if (data.Length > 0)
                    {
                        var file = Path.Combine(downloadFolder, $"{client}.zip");
                        await File.WriteAllBytesAsync(file, data);
                        logger?.LogInformation($"  - saved to {file}");
                    }
                    else
                    {
                        logger?.LogInformation($"  - failed to get client {client}");
                    }
                }
                return new ExecuteCommandResult() { Success = true };
            }, new CommandOptions()
            {
                Description = "List available SDK clients",
            })
            .WithHttpCommand("api/gen/clients", "choose",
            commandOptions: new HttpCommandOptions()
            {
                Method = HttpMethod.Post,
                PrepareRequest = async (context) =>
                {

                    var endPoints = project.Resource.GetEndpoints()?.ToArray() ?? [];
                    var first = endPoints.First(it => it.Url.Contains("http://")).Url.Replace("localhost", "host.docker.internal");
                    var http = first.EndsWith("/") ? first : first + "/";
                    #pragma warning disable ASPIREINTERACTION001 
                    var interaction= context.ServiceProvider.GetService(typeof(IInteractionService)) as IInteractionService;
                    var responseUser = await interaction.PromptInputAsync("API Export", "What do you want to export?( html2 , csharp ...) ", "Client", "html2");    
                    if(responseUser.Canceled)
                    {
                        return ;
                    }
                    var what= responseUser.Data.Value?.Trim()??"html2";
                    var data = new
                    {
                        openAPIUrl = $"{http}{jsonPath}"
                        //openapiNormalizer = [],
                        //options= { },
                        //spec= { }
                    };
                    context.Request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                    var uri = context!.Request!.RequestUri!.ToString();
                    if(!uri.EndsWith("/"))
                        uri += "/";
                    uri= uri+what;
                    context.Request.RequestUri = new Uri(uri);
                    return ;
                    #pragma warning restore ASPIREINTERACTION001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

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

                        return new ExecuteCommandResult() { Success = false, ErrorMessage = $"no link in {text}" };
                    }
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true // Ensures the default browser is used
                    });
                    return new ExecuteCommandResult() { Success = false, ErrorMessage = "test" };
                }
            });

        //foreach (var proj in projects)
        //{
        //    resource.WithReference(proj);
        //    resource.WaitFor(proj);
        //};
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

    private static async Task<byte[]> GetClientZip(HttpClient client, string typeClient, string json)
    {
        try
        {
            var content = JsonContent.Create(

            new
            {
                openAPIUrl = json
                //openapiNormalizer = [],
                //options= { },
                //spec= { }
            }
            );

            var response = await client.PostAsync($"api/gen/clients/{typeClient}", content);
            response.EnsureSuccessStatusCode();
            var text = await response.Content.ReadAsStringAsync();

            var resDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(text);
            ArgumentNullException.ThrowIfNull(resDict);
            if (!resDict.TryGetValue("link", out var url))
            {

                return [];
            }
            return await client.GetByteArrayAsync(url);
        }
        catch
        {
            //maybe log the error
            return [];
        }
    }

}