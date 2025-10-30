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
    public string LoginUrl() => (_loginUrl ?? "");

    private string? _baseUrl;
    public string BaseUrl() => (_baseUrl ?? "");
    public async Task<string> StartParsing(DistributedApplication da)
    {
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
                var urls = mainState.Urls.AddRange(baseUrlSnap, urlSha);

                EnvironmentVariableSnapshot baseEnv = new EnvironmentVariableSnapshot("ASPIRE_BASE_URL", _baseUrl, true);
                var env = mainState.EnvironmentVariables.AddRange(login, baseEnv);

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
}
