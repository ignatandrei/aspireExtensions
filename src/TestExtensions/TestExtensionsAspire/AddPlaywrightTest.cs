using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;


namespace TestExtensionsAspire;

public class AspireResource: Resource, IResourceWithEnvironment, IResourceWithEndpoints,IResourceWithServiceDiscovery
{
    internal AspireResource() : base("AspireResource")
    {

    }
    private List<IResourceBuilder<IResourceWithEnvironment>> resources = new();
    public void AddEnvironmentVariablesTo(IResourceBuilder<IResourceWithEnvironment> resource)
    {
        resources.Add(resource);
        resource.WithEnvironment("ASPIRE_LOGIN_URL",LoginUrl);
        resource.WithEnvironment("ASPIRE_BASE_URL",BaseUrl);
    }

    private string? _loginUrl;
    public string LoginUrl()=> (_loginUrl ??"");
    
    private string? _baseUrl;
    public string BaseUrl()=> (_baseUrl ??"");
    public async Task<string> StartParsing(DistributedApplication da)
    {
        var ret= await AddTest.ViewData(da);
        if (ret != null)
        {
            var env = await this.GetEnvironmentVariableValuesAsync();
            await da.ResourceNotifications.PublishUpdateAsync(this, mainState => {
                EnvironmentVariableSnapshot login=new("ASPIRE_LOGIN_URL", ret,true);
                
                var urlSha = new UrlSnapshot("ASPIRE_LOGIN_URL", ret, false)
                {
                    DisplayProperties=new("LoginUrl")
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
                var urls = mainState.Urls.AddRange(baseUrlSnap,urlSha);

                EnvironmentVariableSnapshot baseEnv = new EnvironmentVariableSnapshot("ASPIRE_BASE_URL", _baseUrl, true);
                var env = mainState.EnvironmentVariables.AddRange(login,baseEnv);
                
                foreach (var r in resources)
                {
                    da.ResourceNotifications.PublishUpdateAsync(r.Resource, s =>
                    {

                        var envRes = s.EnvironmentVariables.AddRange(
                            new EnvironmentVariableSnapshot("ASPIRE_LOGIN_URL", ret, true),
                            new EnvironmentVariableSnapshot("ASPIRE_BASE_URL", _baseUrl, true)

                            );
                        return s with
                        {
                            EnvironmentVariables = envRes
                        };

                    });
                };

                
                
                return mainState with
                {
                    State = KnownResourceStates.Running,
                    EnvironmentVariables = env,
                    Urls = urls,
                };
            });
        }
        _loginUrl = ret;
        return _loginUrl??"";
    }
}

public record DataForPlayWrightTest(Func<DistributedApplication,Task<string?>> ParseLoginUrl,Func<string> GetLoginUrl,Func<string> GetUrl );
public static class AddTest
{
    static AspireResource resource = new();
    static bool added = false;
    public static IResourceBuilder<AspireResource>? AddAspireResource(this IDistributedApplicationBuilder builder)
    {
        if(added)
        {
            return null;
        }
        added = true;
        var res = builder.AddResource(resource);
        builder.Services.AddFakeLogging();
        return res;
    }
    public static DataForPlayWrightTest ParseLogsForAspireUrl(this IDistributedApplicationBuilder builder)
    {
        builder.Services.AddFakeLogging();
        return new (ViewData,()=> UrlFunc(true),()=>UrlFunc(false));
    }
    static string? LoginUrl = null;
    static string UrlFunc(bool login)
    {
        var loginUrl = LoginUrl ?? "";
        if (login)
        {
            return loginUrl;
        }
        if(loginUrl.IndexOf("login?")>0)
        {
            return loginUrl.Substring(0, loginUrl.IndexOf("login?"));
        }
        return loginUrl;
    }

    internal static async Task<string?> ViewData(DistributedApplication da)
    {
        await Task.Delay(1000);
        var fake = typeof(FakeLoggerProvider);
        var logger= da.Services.GetServices<ILoggerProvider>();
        FakeLoggerProvider? fakeLogger=null;
        foreach (var service in logger)
        {
            if(service.GetType() == fake)
            {
                fakeLogger = service as FakeLoggerProvider;
            }
        }
        ArgumentNullException.ThrowIfNull(fakeLogger);
        var tsWait = TimeSpan.FromSeconds(5);
        string? url = null;
        while (url == null )
        {

            var messages = fakeLogger.Collector.GetSnapshot().Select(x => x.Message).ToArray();
            //Console.WriteLine("wating"+string.Join(',',messages));
            await Task.Delay(tsWait);
            url = FindUrl(messages);
        }
        string? login = null;
        var nrRetry = 10;
        while (url != null && login == null && nrRetry > 0)
        {
            nrRetry--;
            var messages = fakeLogger.Collector.GetSnapshot().Select(x => x.Message).ToArray();
            login = FindLogin(messages);
            await Task.Delay(tsWait);
        }
        LoginUrl = login ?? url;
        return LoginUrl;
    }
    private static string? FindLogin(string[] messages)
    {
        var mes = messages.FirstOrDefault(it => it.Contains("Login to the dashboard at"));
        if (mes == null) return null;
        var url = mes.Replace("Login to the dashboard at", "");
        return url.Trim();
    }
    private static string? FindUrl(string[] messages)
    {
        var mes = messages.FirstOrDefault(it => it.Contains("Now listening on:"));
        if (mes == null) return null;
        var url = mes.Replace("Now listening on:", "");
        return url.Trim();

    }
}
