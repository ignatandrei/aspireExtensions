
namespace AspireResourceExtensionsAspire;

public class AspireResource : Resource, IResourceWithEnvironment, IResourceWithEndpoints, IResourceWithServiceDiscovery
{
    internal AspireResource() : base("AspireResource")
    {

    }
    private List<IResourceBuilder<IResourceWithEnvironment>> resources = new();
    public void AddEnvironmentVariablesTo(params IResourceBuilder<IResourceWithEnvironment>[] resources)
    {
        foreach (var resource in resources)
        {
            this.resources.Add(resource);
            resource.WithEnvironment("ASPIRE_LOGIN_URL", LoginUrl);
            resource.WithEnvironment("ASPIRE_BASE_URL", BaseUrl);
        }
    }

    private string? _loginUrl;
    public string LoginUrl() => (_loginUrl ?? "Notyet");

    private string? _baseUrl;
    public string BaseUrl() => (_baseUrl ?? "");
    public async Task<string> StartParsing(DistributedApplication da, IDistributedApplicationBuilder builder)
    {
        MyAppResource myApp = MyAppResource.Construct(da, builder);


        var ret = await AddAspire.ViewData(da);
        _loginUrl = ret??"";
        if (ret == null)
        {
            return _loginUrl;
        }
        string webServer = await StartWebServerAsync(myApp, da.ResourceCommands, ret)??"";        
        //var env = await this.GetEnvironmentVariableValuesAsync();
        await da.ResourceNotifications.PublishUpdateAsync(this, mainState =>
        {
            EnvironmentVariableSnapshot login = new("ASPIRE_LOGIN_URL", ret, true);

            var urlSha = new UrlSnapshot("ASPIRE_LOGIN_URL", ret, false)
            {
                DisplayProperties = new("LoginUrl")
            };

            _baseUrl = ret;
            if (ret.IndexOf("login?") > 0)
            {
                _baseUrl = ret.Substring(0, ret.IndexOf("login?"));
            }

            UrlSnapshot baseUrlSnap = new UrlSnapshot("ASPIRE_BASE_URL", _baseUrl, false)
            {
                DisplayProperties = new("BaseUrl")
            };
            UrlSnapshot webServerSnap = new UrlSnapshot("ASPIRE_NEW_ASPIRE_URL", webServer ?? "", false)
            {
                DisplayProperties = new("NewAspireUrl")
            };
            var urls = mainState.Urls.AddRange(urlSha, webServerSnap);

            EnvironmentVariableSnapshot baseEnv = new EnvironmentVariableSnapshot("ASPIRE_BASE_URL", _baseUrl, true);
            EnvironmentVariableSnapshot newAspire = new EnvironmentVariableSnapshot("ASPIRE_NEW_ASPIRE_URL", webServer ?? "", true);

            var env = mainState.EnvironmentVariables.AddRange(login, baseEnv, newAspire);

            foreach (var r in resources)
            {
                da.ResourceNotifications.PublishUpdateAsync(r.Resource, s =>
                {

                    var envRes = s.EnvironmentVariables.AddRange(

                        new EnvironmentVariableSnapshot("ASPIRE_LOGIN_URL", ret, true),
                        new EnvironmentVariableSnapshot("ASPIRE_BASE_URL", _baseUrl, true),
                        new EnvironmentVariableSnapshot("ASPIRE_NEW_ASPIRE_URL", webServer ?? "", true)

                        );
                    return s with
                    {
                        EnvironmentVariables = envRes
                    };

                });
            }
            ;



            return mainState with
            {
                State = KnownResourceStates.Running,
                EnvironmentVariables = env,
                Urls = urls,
            };
        });

        return ret;
    }

    internal async Task<string?> StartWebServerAsync(MyAppResource myApp, ResourceCommandService resourceCommands, string aspireUrl)
    {
        var port = new Uri(aspireUrl).Port;
        var newPort = port + 1;

        var builder = WebApplication.CreateBuilder();
        //builder.WebHost.UseUrls($"http://*:{port}");
        builder.Services.AddOpenApi();
        builder.Services.AddCors();

        var p = builder.Environment.WebRootFileProvider;
        builder.Environment.WebRootFileProvider= MapFileProvider.Manifest(builder);
        var app = builder.Build();
        //MapFileProvider.mapFile("wwwroot", prov, app);
        //app.Urls.Add("http://127.0.0.1:0");
        app.UseCors(it =>
        {
            it
              .AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(it => true)
              .AllowCredentials();
        });
        
        app.MapOpenApi();
        app.MapOpenApi("/openapi/{documentName}.yaml");

        app.Urls.Add($"http://127.0.0.1:{newPort}");
        // Serve static files from a "wwwroot" directory
        
        app.UseDefaultFiles();
        app.UseStaticFiles();
        
        
        app.MapGet("/api/aspire/resources/export/mermaid", ()=>myApp.ExportToMermaid());
        app.MapGet("/api/aspire/resources/export/csv", () => myApp.ExportToCSV());

        app.MapGet("/api/aspire/resources/", () =>
        {
            return myApp.MyResources();
        });
        app.MapGet("/api/aspire/resources/{name}", (string name) =>
        {
            return myApp.DetailsResource(name);
        });
            
        app.MapPost("/api/aspire/resources/{name}/execute/{command}", async Task<Results<Ok<ExecuteCommandResult>, NotFound<string>>> (string name,string command) =>
        {
            if (!myApp.ExistResource(name))
                return TypedResults.NotFound($"cannot find command {command} on {name}");

            var res = await resourceCommands.ExecuteCommandAsync(name, command);  
            return TypedResults.Ok(res);
        });


        await app.StartAsync();
        var addresses = app.Services.GetRequiredService<IServer>().Features.GetRequiredFeature<IServerAddressesFeature>().Addresses;

        var first=addresses.FirstOrDefault();
        return first;
        


    }
}
