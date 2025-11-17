namespace AspireResourceExtensionsAspire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;

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
    public string LoginUrl() => (_loginUrl ?? "");

    private string? _baseUrl;
    public string BaseUrl() => (_baseUrl ?? "");
    public async Task<string> StartParsing(DistributedApplication da, IDistributedApplicationBuilder builder)
    {
        MyAppResource myApp = MyAppResource.Construct(da, builder);
        
        var webServer = await StartWebServerAsync(myApp);

        var ret = await AddAspire.ViewData(da);
        if (ret != null)
        {
            var env = await this.GetEnvironmentVariableValuesAsync();
            await da.ResourceNotifications.PublishUpdateAsync(this, mainState => {
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
                UrlSnapshot webServerSnap=new UrlSnapshot("ASPIRE_NEW_ASPIRE_URL", webServer ?? "", false)
                {
                    DisplayProperties = new("NewAspireUrl")
                };
                var urls = mainState.Urls.AddRange(baseUrlSnap, urlSha, webServerSnap);
                
                EnvironmentVariableSnapshot baseEnv = new EnvironmentVariableSnapshot("ASPIRE_BASE_URL", _baseUrl, true);
                EnvironmentVariableSnapshot newAspire = new EnvironmentVariableSnapshot("ASPIRE_NEW_ASPIRE_URL", webServer ?? "", true);

                var env = mainState.EnvironmentVariables.AddRange(login, baseEnv,newAspire);

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
        }
        _loginUrl = ret;
        return _loginUrl ?? "";
    }

    internal async Task<string?> StartWebServerAsync(MyAppResource myApp)
    {
        var builder = WebApplication.CreateBuilder();
        //builder.WebHost.UseUrls($"http://*:{port}");

        var app = builder.Build();
        app.Urls.Add("http://127.0.0.1:0");
        // Serve static files from a "wwwroot" directory
        app.UseDefaultFiles();
        app.UseStaticFiles();

        // If wwwroot/index.html does not exist, create a simple one
        var wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var indexPath = Path.Combine(wwwroot, "index.html");
        if (!Directory.Exists(wwwroot))
            Directory.CreateDirectory(wwwroot);
        if (!File.Exists(indexPath))
            await File.WriteAllTextAsync(indexPath, "<!DOCTYPE html><html><body><h1>Hello from AspireResource!</h1></body></html>");

        app.MapGet("/aspire/resources/export/mermaid", ()=>myApp.ExportToMermaid());
        await app.StartAsync();
        var addresses = app.Services.GetRequiredService<IServer>().Features.GetRequiredFeature<IServerAddressesFeature>().Addresses;

        var first=addresses.FirstOrDefault();
        return first;
        


    }
}
